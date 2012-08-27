using System;
using System.Collections.Generic;
using System.Linq;
using CommonDX;
using SharpDX.Direct2D1;

namespace MetroRetro.Games
{
    class Tigers : BaseGame
    {
        private const float PlayerSpd = 0.8f;
        private const float EnemySpd = 0.15f;
        private const float BulletSpd = 0.9f;

        private readonly Point _enemyDir = new Point(0.0f, 1.0f);
        private readonly Point _bulletDir = new Point(0.0f, -1.0f);

        private List<Point> _enemyPos;
        private List<Point> _bulletPos; 

        private Point _playerPos;
        private Point _playerDir;

        private readonly Point _playerSize = new Point(0.05f, 0.05f);
        private readonly Point _bulletSize = new Point(0.01f, 0.01f);
        private readonly Point _enemySize = new Point(0.05f, 0.05f);

        public Tigers(GameManager gameManager, float maxTime)
            : base(gameManager, maxTime)
        {
        }

        public override void SetArrows()
        {
            _gameManager.Page.SetArrowButtons(true, false, true, true);
        }

        public override void SetArrowsLabels()
        {
            _gameManager.Page.SetArrowButtonsLabels(up: "≜");
        }

        readonly Random _r = new Random();
        private bool _isShootButtonPressed;
        private float _timeToNextPossibleShoot;
        private const float ShootInterval = 0.3f;

        private void AddEnemy(int id = -1)
        {
            var p = new Point(
                (float) _r.NextDouble() * (GamesParams.MarginX1 - GamesParams.MarginX0 - _enemySize.X) + GamesParams.MarginX0 + _enemySize.X / 2, 
                (float) _r.NextDouble() * 2 - 2.1f);

            if (id >= 0)
                _enemyPos[id] = p;
            else
                _enemyPos.Add(p);
        }

        public override void NewGame()
        {
            _playerPos = new Point(0.5f, GamesParams.MarginY1).Clamp(GamesParams.Margin0.Add(_playerSize.Half()),
                                                                     GamesParams.Margin1.Sub(_playerSize.Half()));

            _enemyPos = new List<Point>();
            for (var i = 0; i < 2; i++)
            {
                AddEnemy();
            }

            _bulletPos = new List<Point>();
            _playerDir = new Point(0.0f, 0.0f);

            _isShootButtonPressed = false;
            _timeToNextPossibleShoot = 0.0f;
            base.NewGame();
        }

        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            if (AfterStartFreeze)
            {
                _timeToNextPossibleShoot -= dt;
                if (_isShootButtonPressed && _timeToNextPossibleShoot <= 0)
                {
                    var p = _playerPos.Sub(new Point(0, 0.01f));
                    _bulletPos.Add(p);

                    _timeToNextPossibleShoot = ShootInterval;
                }


                // Player moving
                _playerPos =
                    _playerPos.Add(_playerDir.Mul(PlayerSpd).Mul(dt)).Clamp(GamesParams.Margin0.Add(_playerSize.Half()), 
                                                                            GamesParams.Margin1.Sub(_playerSize.Half()));
                        
                // Enemies
                for (var e = 0; e < _enemyPos.Count; ++e)
                {
                    var enemyPos = _enemyPos[e];

                    // Enemy reach bottom line
                    _enemyPos[e] = enemyPos.Add(_enemyDir.Mul(EnemySpd).Mul(dt));
                    enemyPos = _enemyPos[e];

                    if (enemyPos.IsInside(GamesParams.Margin0.Sub(_enemySize.Half()).Add(new Point(0, 1)),
                                          GamesParams.Margin1.Add(_enemySize.Half()).Add(new Point(0, 1))))
                    {
                        AddEnemy(e);
                        AddEnemy();
                        AddEnemy();
                    }

                    // Enemy collision with player
                    if (enemyPos.IsInside(_playerPos.Sub(_playerSize.Half()).Sub(_playerSize.Half()),
                                          _playerPos.Add(_playerSize.Half()).Add(_playerSize.Half())))
                    {
                        _gameManager.Die();
                        NewGame();
                    }
                }

                // Bullet moving
                for (var i = 0; i < _bulletPos.Count; ++i)
                {
                    var bulletPos = _bulletPos[i];
                    _bulletPos[i] = bulletPos.Add(_bulletDir.Mul(BulletSpd).Mul(dt));
                    bulletPos = _bulletPos[i];

                    // Bullet collision with borders
                    if (!bulletPos.IsInside(GamesParams.Margin0.Sub(_bulletSize.Half()),
                                            GamesParams.Margin1.Add(_bulletSize.Half())))
                    {
                        _bulletPos.RemoveAt(i);
                        AddEnemy();
                    }

                    // Bullet collision with enemies
                    for (var e = 0; e < _enemyPos.Count; ++e)
                    {
                        var enemyPos = _enemyPos[e];
                        if (bulletPos.IsInside(enemyPos.Sub(_enemySize.Half()).Sub(_bulletSize.Half()),
                                               enemyPos.Add(_enemySize.Half()).Add(_bulletSize.Half())))
                        {
                            _bulletPos.RemoveAt(i);
                            AddEnemy(e);
                            AddEnemy();
                            _gameManager.AddPoints(25);
                            break;
                        }
                    }
                }
            }

            // Drawing
            foreach (var bullet in _bulletPos)
            {
                var ballBox = bullet.ToBox(_bulletSize);
                context.FillRectangle(screenSize.ApplyTo(ballBox), GamesParams.AdditionalColor);
            }

            foreach (var enemy in _enemyPos)
            {
                var box = enemy.ToBox(_enemySize);
                if (box.Left > GamesParams.MarginX1 || box.Right < GamesParams.MarginX0 || box.Bottom < GamesParams.MarginY0 || box.Top > GamesParams.MarginY1)
                    continue;

                if (box.Left < GamesParams.MarginX0)
                    box.Left = GamesParams.MarginX0;
                if (box.Right > GamesParams.MarginX1)
                    box.Right = GamesParams.MarginX1;
                if (box.Top < GamesParams.MarginY0)
                    box.Top = GamesParams.MarginY0;
                if (box.Bottom > GamesParams.MarginY1)
                    box.Bottom = GamesParams.MarginY1;

                context.FillRectangle(screenSize.ApplyTo(box), GamesParams.EnemyColor);
            }

            var playerBox = _playerPos.ToBox(_playerSize);
            context.FillRectangle(screenSize.ApplyTo(playerBox), GamesParams.PlayerColor);

            base.Update(context, target, deviceManager, screenSize, dt, elapsedTime);
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
                case InputType.Left:
                    _playerDir = new Point(-1, 0);
                    break;

                case InputType.Right:
                    _playerDir = new Point(1, 0);
                    break;

                case InputType.Up:
                case InputType.Space:
                    _isShootButtonPressed = true;
                    break;
            }
        }

        public override void KeyReleased(InputType key)
        {
            switch (key)
            {
                case InputType.Left:
                    _playerDir = new Point(0, 0);
                    break;

                case InputType.Right:
                    _playerDir = new Point(0, 0);
                    break;
                    
                case InputType.Up:
                case InputType.Space:
                    _isShootButtonPressed = false;
                    break;
            }
        }
    }
}
