using System;
using System.Collections.Generic;
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

        private readonly Point _snakePartSize = new Point(0.005f, 0.005f);
        private readonly Point _appleSize = new Point(0.01f, 0.01f);

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
            _snake.Add(new Point(0.5f, 0.5f));

            _apple = new Point(0.3f, 0.3f);

            _snakeSpd = 0.8f;
            base.NewGame();
        }

        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            // Player moving
            //_playerPos = _playerPos.Add(_playerDir.Mul(_playerSpd).Mul(dt)).Clamp(GamesParams.Margin0.Add(_padSize.Half()),
            //                                                                      GamesParams.Margin1.Sub(_padSize.Half()));
                 
            // Drawing
            foreach (var snakePart in _snake)
            {
                var box = snakePart.ToBox(_snakePartSize);
                context.FillRectangle(screenSize.ApplyTo(box), GamesParams.PlayerColor);
            }

            var appleBox = _apple.ToBox(_appleSize);
            context.FillRectangle(screenSize.ApplyTo(appleBox), GamesParams.AdditionalColor);
            base.Update(context, target, deviceManager, screenSize, dt, elapsedTime);
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
                case InputType.Left:
                    _snakeDir = new Point(-1, 0);
                    break;

                case InputType.Right:
                    _snakeDir = new Point(1, 0);
                    break;
            }
        }

        public override void KeyReleased(InputType key)
        {
            switch (key)
            {
                case InputType.Left:
                    _snakeDir = new Point(0, 0);
                    break;

                case InputType.Right:
                    _snakeDir = new Point(0, 0);
                    break;
            }
        }
    }
}
