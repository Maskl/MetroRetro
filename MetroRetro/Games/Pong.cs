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

        private int _playerY;
        private int _enemyY;
        public override void NewGame()
        {
            _playerY = 300;
            _enemyY = 300;
        }

        public override void EndGame()
        {
        }

        public override void Update(long dt, Point screenSize, DeviceContext context, TargetBase target)
        {
            context.FillRectangle(new RectangleF(10, 10, screenSize.X / 3, screenSize.Y / 3), EnemyColor);
        }

        public override void KeyPressed(InputType key)
        {
        }

        public override void KeyReleased(InputType key)
        {
        }
    }
}
