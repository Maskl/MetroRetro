using System;
using System.Collections.Generic;
using System.Linq;
using CommonDX;
using SharpDX.Direct2D1;

namespace MetroRetro.Games
{
    class Snake : BaseGame
    {
        private List<Point> _snake;
        private Point _snakeDir;
        private Point _apple;
        private float _snakeSpd;
        private float _sumDt = 0;

        private readonly Point _snakePartSize = new Point(0.01f, 0.01f);
        private readonly Point _appleSize = new Point(0.02f, 0.02f);
        private Point _snakePos, _snakeLastPos;
        private int _snakePossibleSize;

        public Snake(GameManager gameManager, float maxTime)
            : base(gameManager, maxTime)
        {
        }

        public override void SetArrows()
        {
            _gameManager.Page.SetArrowButtons(true, true, true, true);
        }

        public override void NewGame()
        {
            _snake = new List<Point>();
            _snakePos = new Point(0.5f, 0.5f);
            _snakeLastPos = new Point(0.5f, 0.5f);
            _snake.Add(_snakePos);
            _snakeSpd = 0.1f;
            _snakeDir = new Point(1, 0);
            _sumDt = 0;
            _snakePossibleSize = 100;
            PlaceNewApple();
            base.NewGame();
        }

        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            if (AfterStartFreeze)
            {
                // Player moving
                _snakePos = _snakePos.Add(_snakeDir.Mul(_snakeSpd).Mul(dt));

                _sumDt += dt;
                if (_sumDt > 0.05)
                {
                    var s0 = _snakeLastPos;
                    var s1 = _snakePos;
                    var it = s1.Sub(s0);
                    var lp = Math.Round(it.Length()/_snakePartSize.Mul(0.5f).X);
                    it = it.Normalise().Mul(_snakePartSize.Mul(0.5f));
                    var pp = s0;
                    while (lp-- > 0)
                    {
                        // Eating apple
                        if (pp.Sub(_apple).Length() < _snakePartSize.X/2 + _appleSize.X/2)
                        {
                            _gameManager.Win(300);
                            _snakePossibleSize += 20;
                            _snakeSpd += 0.01f;
                            PlaceNewApple();
                            return;
                        }

                        // Collision with borders
                        if (!pp.IsInside(GamesParams.Margin0.Add(_snakePartSize.Half()),
                                         GamesParams.Margin1.Sub(_snakePartSize.Half())))
                        {
                            NewGame();
                            _gameManager.Die();
                            return;
                        }

                        // Self collision
                        var mm = _snake.Count - 3;
                        if (
                            _snake.TakeWhile(snakePart => --mm > 0).Any(
                                snakePart => snakePart.Sub(pp).Length() < _snakePartSize.X/3*2))
                        {
                            NewGame();
                            _gameManager.Die();
                            return;
                        }

                        _snake.Add(new Point(pp.X, pp.Y));
                        pp = pp.Add(it);
                        _sumDt = 0;
                        _snakeLastPos = _snakePos;
                    }

                    while (_snake.Count > _snakePossibleSize)
                    {
                        _snake.RemoveAt(0);
                    }

                    Debug(_snake.Count.ToString());
                }
            }

            // Drawing
            foreach (var snakePart in _snake)
            {
                var box = snakePart.ToBox(_snakePartSize);
                context.FillRectangle(screenSize.ApplyTo(box), GamesParams.PlayerColor);
            }

            var headBox = _snake.Last().ToBox(_snakePartSize.Mul(0.5f));
            context.FillRectangle(screenSize.ApplyTo(headBox), GamesParams.AdditionalColor);

            var appleBox = _apple.ToBox(_appleSize);
            context.FillRectangle(screenSize.ApplyTo(appleBox), GamesParams.AdditionalColor);
            base.Update(context, target, deviceManager, screenSize, dt, elapsedTime);
        }

        private void PlaceNewApple(int tryCount = 20)
        {
            var rand = new Random();
            
            var xx = rand.NextDouble();
            while (xx < GamesParams.MarginX0 + _appleSize.X || xx > GamesParams.MarginX1 - _appleSize.X)
                xx = rand.NextDouble();
            
            var yy = rand.NextDouble();
            while (yy < GamesParams.MarginY0 + _appleSize.Y || yy > GamesParams.MarginY1 - _appleSize.Y)
                yy = rand.NextDouble();

            _apple = new Point((float)xx, (float)yy);
            if (_snake.Any(snakePart => snakePart.Sub(_apple).Length() < _snakePartSize.X / 3 * 2))
            {
                NewGame();
                _gameManager.Die();
                return;
            }

            if (tryCount == 0)
                return;

            PlaceNewApple(tryCount - 1);
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                    _snakeDir = new Point(0, -1);
                    _sumDt = 999;
                    break;

                case InputType.Down:
                    _snakeDir = new Point(0, 1);
                    _sumDt = 999;
                    break;
                case InputType.Left:
                    _snakeDir = new Point(-1, 0);
                    _sumDt = 999;
                    break;

                case InputType.Right:
                    _snakeDir = new Point(1, 0);
                    _sumDt = 999;
                    break;
            }
        }

        public override void KeyReleased(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                case InputType.Down:
                case InputType.Left:
                case InputType.Right:
                    break;
            }
        }
    }
}
