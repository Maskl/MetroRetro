using System;
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

        private Point _enemyPos;

        private float _playerSpeed = 0.8f;
        private float _enemySpeed = 0.01f;
        private float _ballSpeed = 0.01f;

        public override void NewGame()
        {
            _playerPos = new Point(0.05f, 0.5f);
            _enemyPos  = new Point(0.95f, 0.5f);
            _playerDir = new Point(0.0f, 0.0f);
        }

        public override void EndGame()
        {
        }

        public override void Update(DeviceContext context, TargetBase target, Point screenSize, float dt, float elapsedTime)
        {
            _playerPos = _playerPos.Add(_playerDir.Mul(dt));

            var playerBox = _playerPos.ToBox(new Point(0.05f, 0.3f));
            var enemyBox = _enemyPos.ToBox(new Point(0.05f, 0.3f));

            context.FillRectangle(screenSize.ApplyTo(playerBox), GameColors.PlayerColor);
            context.FillRectangle(screenSize.ApplyTo(enemyBox), GameColors.EnemyColor);
        }

        public override void KeyPressed(InputType key)
        {
            switch (key)
            {
                case InputType.Up:
                    _playerDir = new Point(0, -_playerSpeed);
                    break;

                case InputType.Down:
                    _playerDir = new Point(0, _playerSpeed);
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
