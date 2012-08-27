using System;
using CommonDX;
using SharpDX.Direct2D1;

namespace MetroRetro.Games
{
    class MoonPatrol : BaseGame
    {
        private const int BlocksCountX = 7;
        private const int BlocksCountY = 5;
        private const float BlockMargin = 0.05f;
        private const float BottomLinesOfBlocks = 0.5f;

        private Point _playerPos;
        private Point _playerDir;
        private float _playerSpd;

        private Point[,] _blockPos;

        private Point _ballPos;
        private Point _ballDir;
        private float _ballSpd;

        private const float _ballSpdInc = 0.01f;

        private readonly Point _padSize = new Point(0.20f, 0.03f);
        private readonly Point _ballSize = new Point(0.01f, 0.01f);
        private readonly Point _blockSize = new Point(
            (GamesParams.MarginX0 + (GamesParams.MarginX1 - GamesParams.MarginX0) - BlockMargin * (BlocksCountX + 1)) / BlocksCountX,
            (GamesParams.MarginY0 + (GamesParams.MarginY1 - BottomLinesOfBlocks) - BlockMargin * (BlocksCountY + 1)) / BlocksCountY);

        public MoonPatrol(GameManager gameManager, float maxTime)
            : base(gameManager, maxTime)
        {
        }

        public override void SetArrows()
        {
            _gameManager.Page.SetArrowButtons(false, false, true, true);
        }

        public override void NewGame()
        {
            _playerPos = new Point(0.5f, GamesParams.MarginY1).Clamp(GamesParams.Margin0.Add(_padSize.Half()),
                                                                     GamesParams.Margin1.Sub(_padSize.Half()));

            _ballPos = _playerPos.Sub(new Point(0, 0.1f));

            _blockPos = new Point[BlocksCountX, BlocksCountY];
            for (var y = 0; y < BlocksCountY; ++y)
            {
                for (var x = 0; x < BlocksCountX; x++)
                {
                    var xx = GamesParams.MarginX0 + (GamesParams.MarginX1 - GamesParams.MarginX0) / (BlocksCountX + 1) * (x + 1);
                    var yy = GamesParams.MarginY0 + (GamesParams.MarginY1 - BottomLinesOfBlocks ) / (BlocksCountY + 1) * (y + 1);

                    _blockPos[x, y] = new Point(xx, yy).Clamp(GamesParams.Margin0.Add(_blockSize.Half()),
                                                              GamesParams.Margin1.Sub(_blockSize.Half()));
                }
            }

            _playerSpd = 0.8f;
            _ballSpd = 0.3f;

            _playerDir = new Point(0.0f, 0.0f);
            _ballDir = new Point(0.0f, -1.0f);

            base.NewGame();
        }

        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            if (AfterStartFreeze)
            {
                // Player moving
                _playerPos =
                    _playerPos.Add(_playerDir.Mul(_playerSpd).Mul(dt)).Clamp(GamesParams.Margin0.Add(_padSize.Half()),
                                                                             GamesParams.Margin1.Sub(_padSize.Half()));

                // Ball moving
                var oldBallPos = _ballPos;
                _ballPos = _ballPos.Add(_ballDir.Mul(_ballSpd).Mul(dt));

                // Ball collision with borders
                if (!_ballPos.IsInside(GamesParams.Margin0.Add(_ballSize.Half()),
                                       GamesParams.Margin1.Sub(_ballSize.Half())))
                {
                    if (_ballPos.X - GamesParams.MarginX0 < _ballPos.Y - GamesParams.MarginY0 ||
                        GamesParams.MarginX1 - _ballPos.X < _ballPos.Y - GamesParams.MarginY0)
                        _ballDir.X = -_ballDir.X;
                    else
                        _ballDir.Y = -_ballDir.Y;

                    if (_ballPos.Y > GamesParams.MarginY1 - 0.01f)
                    {
                        _gameManager.Die();
                        _playerPos =
                            new Point(0.5f, GamesParams.MarginY1).Clamp(GamesParams.Margin0.Add(_padSize.Half()),
                                                                        GamesParams.Margin1.Sub(_padSize.Half()));
                        _ballPos = _playerPos.Sub(new Point(0, 0.1f));
                        _ballDir = new Point(0.0f, -1.0f);
                    }
                }

                _ballPos = _ballPos.Clamp(GamesParams.Margin0.Add(_ballSize.Half()),
                                          GamesParams.Margin1.Sub(_ballSize.Half()));

                // Ball collision with player pad
                if (_ballPos.IsInside(_playerPos.Sub(_padSize.Half()).Sub(_ballSize.Half()),
                                      _playerPos.Add(_padSize.Half()).Add(_ballSize.Half())))
                {
                    _ballDir.Y = -1;
                    _ballDir.X = _ballPos.Sub(_playerPos).X/_padSize.Half().X;
                    _ballDir = _ballDir.Normalise();

                    _ballSpd += _ballSpdInc;
                }

                // Ball collision with player pad
                foreach (var block in _blockPos)
                {
                    if (_ballPos.IsInside(block.Sub(_blockSize.Half()).Sub(_ballSize.Half()),
                                          block.Add(_blockSize.Half()).Add(_ballSize.Half())))
                    {
                        if (oldBallPos.X < block.Sub(_blockSize.Half()).Sub(_ballSize.Half()).X ||
                            oldBallPos.X > block.Add(_blockSize.Half()).Add(_ballSize.Half()).X)
                            _ballDir.X = -_ballDir.X;
                        else
                            _ballDir.Y = -_ballDir.Y;

                        block.X = -100;
                        block.Y = -100;

                        _gameManager.AddPoints(100);
                    }
                }
            }

            // Drawing
            var anyBlock = false;
            foreach (var block in _blockPos)
            {
                if (block.X < 0)
                    continue;

                anyBlock = true;
                var box = block.ToBox(_blockSize);
                context.FillRectangle(screenSize.ApplyTo(box), GamesParams.EnemyColor);
            }

            if (!anyBlock)
            {
                NewGame();
                _gameManager.Win(5000);
            }

            var playerBox = _playerPos.ToBox(_padSize);
            var ballBox = _ballPos.ToBox(_ballSize);
            context.FillRectangle(screenSize.ApplyTo(playerBox), GamesParams.PlayerColor);
            context.FillRectangle(screenSize.ApplyTo(ballBox), GamesParams.AdditionalColor);

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
            }
        }
    }
}
