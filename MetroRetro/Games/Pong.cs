﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Windows.Foundation;
using Matrix = SharpDX.DirectWrite.Matrix;

namespace MetroRetro.Games
{
    class Pong : BaseGame
    {
        public Pong(GameManager gameManager) : base(gameManager)
        {
        }

        private Point _playerPos;
        private Point _playerDir;
        private float _playerSpd;

        private Point _enemyPos;
        private Point _enemyDir;
        private float _enemySpd;

        private Point _ballPos;
        private Point _ballDir;
        private float _ballSpd;

        private readonly Point _padSize = new Point(0.05f, 0.25f);
        private readonly Point _ballSize = new Point(0.01f, 0.01f);

        public override void NewGame()
        {
            _playerPos = new Point(GamesParams.MarginX0, 0.5f);
            _enemyPos = new Point(GamesParams.MarginX1, 0.5f);
            _ballPos = new Point(0.5f, 0.5f);

            _playerSpd = 0.8f;
            _enemySpd = 0.8f;
            _ballSpd = 0.3f;

            _playerDir = new Point(0.0f, 0.0f);
            _enemyDir = new Point(0.0f, 0.0f);
            _ballDir = new Point(-1, 0.0f);
        }

        public override void EndGame()
        {
        }

        public override void Update(DeviceContext context, TargetBase target, Point screenSize, float dt, float elapsedTime)
        {
            // Player moves
            _playerPos = _playerPos.Add(_playerDir.Mul(_playerSpd).Mul(dt)).Clamp(GamesParams.Margin0.Add(_padSize.Half()),
                                                                                  GamesParams.Margin1.Sub(_padSize.Half()));

            // Ball moves
            _ballPos = _ballPos.Add(_ballDir.Mul(_ballSpd).Mul(dt));

            // Ball collision with borders
            if (!_ballPos.IsInside(GamesParams.Margin0.Add(_ballSize.Half()), 
                                   GamesParams.Margin1.Sub(_ballSize.Half())))
            {
                _ballDir.Y = -_ballDir.Y;
            }

            _ballPos = _ballPos.Clamp(GamesParams.Margin0.Add(_ballSize.Half()),
                                      GamesParams.Margin1.Sub(_ballSize.Half()));

            // Ball collision with player pad
            if (_ballPos.IsInside(_playerPos.Sub(_padSize.Half()).Sub(_ballSize.Half()),
                                   _playerPos.Add(_padSize.Half()).Add(_ballSize.Half())))
            {
                _ballDir.X = 1;
                _ballDir.Y = _ballPos.Sub(_playerPos).Y / _padSize.Half().Y;
                _ballDir = _ballDir.Normalise();
            }


            var playerBox = _playerPos.ToBox(_padSize);
            var enemyBox = _enemyPos.ToBox(_padSize);
            var ballBox = _ballPos.ToBox(_ballSize);

            context.FillRectangle(screenSize.ApplyTo(playerBox), GamesParams.PlayerColor);
            context.FillRectangle(screenSize.ApplyTo(enemyBox), GamesParams.EnemyColor);

            context.FillRectangle(screenSize.ApplyTo(ballBox), GamesParams.AdditionalColor);

            DrawBoardBorder(context, screenSize);
        }

        protected void DrawBoardBorder(DeviceContext context, Point screenSize)
        {
            context.DrawRectangle(screenSize.ApplyTo(GamesParams.Margin0.ToRectangleWith(GamesParams.Margin1)), GamesParams.ObstaclesColor, 5);
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                    _playerDir = new Point(0, -1);
                    break;

                case InputType.Down:
                    _playerDir = new Point(0, 1);
                    break;
            }
        }

        public override void KeyReleased(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                    _playerDir = new Point(0, 0);
                    break;

                case InputType.Down:
                    _playerDir = new Point(0, 0);
                    break;
            }
        }
    }
}
