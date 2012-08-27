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

        private const float GroundY = 0.7f;

        private readonly Point _playerSize = new Point(0.1f, 0.06f);
        private readonly Point _holeSize = new Point(0.06f, 0.08f);

        
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
            _holePos = new Point(GamesParams.MarginX0 + 0.8f, GroundY + _holeSize.Y / 2.5f);

            _holeSpd = 0.2f;

            base.NewGame();
        }

        Random _r = new Random();
        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            if (AfterStartFreeze)
            {
                // Hole moving
                _holePos = _holePos.Add(_holeDir.Mul(_holeSpd).Mul(dt));
                if (_holePos.X + _holeSize.X / 2 <= GamesParams.MarginX0)
                {
                    _holePos.X = (float)_r.NextDouble() + 1.0f;
                }

                // Ball collision with player pad
                if (_holePos.IsInside(_playerPos.Sub(_playerSize.Half()).Sub(_holeSize.Half()),
                                      _playerPos.Add(_playerSize.Half()).Add(_holeSize.Half())))
                {
               //     _gameManager.Die();
               //     NewGame();
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

                context.FillRectangle(screenSize.ApplyTo(holeBox), GamesParams.BackgroundColor);
            }
            var playerBox = _playerPos.ToBox(_playerSize);
            context.FillRectangle(screenSize.ApplyTo(playerBox), GamesParams.PlayerColor);

            base.Update(context, target, deviceManager, screenSize, dt, elapsedTime);
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
            }
        }
    }
}
