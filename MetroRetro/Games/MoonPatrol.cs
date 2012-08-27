using System;
using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;

namespace MetroRetro.Games
{
    class MoonPatrol : BaseGame
    {
        private Point _playerPos;

        private Point _holePos;
        private readonly Point _holeDir = new Point(-1, 0);
        private float _holeSpd;
        private bool _holeIsStone;

        private const float GroundY = 0.7f;
        private const float JumpHeight = 0.2f;
        private const float JumpSpeed = 2.0f;
        private const float HoleSpdAdd = 0.03f;

        private readonly Point _playerSize = new Point(0.1f, 0.06f);
        private Point _holeSize = new Point(0.06f, 0.08f);

        private float _canWin;


        private Random _r = new Random();
        private float _timeToNextPossibleJump;
        private float _jumpTimer;

        public MoonPatrol(GameManager gameManager, float maxTime)
            : base(gameManager, maxTime)
        {
        }

        public override void SetArrows()
        {
            _gameManager.Page.SetArrowButtons(true, false, false, false);
        }

        public override void NewGame()
        {
            _playerPos = new Point(GamesParams.MarginX0 + 0.2f, GroundY - _playerSize.Y / 2);

            AddNewObstacle();
            _holePos.X = GamesParams.MarginX1 + _holeSize.X;

            _jumpTimer = -1;
            _holeSpd = 0.2f;
            _timeToNextPossibleJump = 0;

            _canWin = 1000;

            base.NewGame();
        }

        public override void ContinueGame()
        {
            _canWin = 1000;
            base.ContinueGame();
        }

        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            if (AfterStartFreeze)
            {
                _timeToNextPossibleJump -= dt;

                if (_jumpTimer >= 0)
                {
                    _jumpTimer += dt * JumpSpeed;
                    _playerPos.Y = GroundY - _playerSize.Y / 2 - (float)Math.Sin(_jumpTimer) * JumpHeight;

                    if (_jumpTimer > Math.PI)
                    {
                        _playerPos.Y = GroundY - _playerSize.Y / 2;
                        _jumpTimer = -1;
                    }
                }
                else
                {
                    _canWin -= dt;
                    if (_canWin < 0)
                    {
                        _canWin = 1000;
                        _gameManager.Win(300);
                        return;
                    }
                }

                // Hole moving
                _holePos = _holePos.Add(_holeDir.Mul(_holeSpd).Mul(dt));
                if (_holePos.X + _holeSize.X / 2 <= GamesParams.MarginX0)
                {
                    AddNewObstacle();
                }
                if (_holePos.X < _playerPos.X && _canWin > 100)
                    _canWin = 0.5f;

                // Ball collision with player pad
                if (_holePos.IsInside(_playerPos.Sub(_playerSize.Half()).Sub(_holeSize.Half()),
                                      _playerPos.Add(_playerSize.Half()).Add(_holeSize.Half())))
                {
                    NewGame();
                    _gameManager.Die();
                    return;
                }
            }

            // Drawing
            context.FillRectangle(screenSize.ApplyTo(new RectangleF(GamesParams.MarginX0, GroundY, GamesParams.MarginX1, GamesParams.MarginY1)), GamesParams.ObstaclesColor);

            var holeBox = _holePos.ToBox(_holeSize);
            if (holeBox.Left < GamesParams.MarginX1)
            {
                if (holeBox.Left < GamesParams.MarginX0)
                    holeBox.Left = GamesParams.MarginX0;

                if (holeBox.Right > GamesParams.MarginX1)
                    holeBox.Right = GamesParams.MarginX1;

                context.FillRectangle(screenSize.ApplyTo(holeBox), _holeIsStone ? GamesParams.ObstaclesColor : GamesParams.BackgroundColor);
            }
            var playerBox = _playerPos.ToBox(_playerSize);
            context.FillRectangle(screenSize.ApplyTo(playerBox), GamesParams.PlayerColor);

            base.Update(context, target, deviceManager, screenSize, dt, elapsedTime);
        }

        private void AddNewObstacle()
        {
            _holeIsStone = _r.Next(1) == 0;
            if (_holeIsStone)
            {
                _holeSize = new Point(0.02f + (float)_r.NextDouble() * 0.05f, 0.02f + (float)_r.NextDouble() * 0.05f);
                _holePos = new Point((float)_r.NextDouble() / 2 + 1.2f, GroundY - _holeSize.Y / 2.1f);
            }
            else
            {
                _holeSize = new Point(0.07f + (float)_r.NextDouble() * 0.1f, 0.07f + (float)_r.NextDouble() * 0.1f);
                _holePos = new Point((float)_r.NextDouble() / 2 + 1.2f, GroundY + _holeSize.Y / 2.1f);
            }

            _holeSpd += HoleSpdAdd;
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
            }
        }

        public override void KeyReleased(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                    if (_timeToNextPossibleJump > 0 || _jumpTimer >= 0)
                        break;

                    _jumpTimer = 0.0f;

                    break;
            }
        }
    }
}
