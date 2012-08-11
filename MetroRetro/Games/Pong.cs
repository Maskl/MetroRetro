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

        private Point _player;
        private Point _enemy;

        public override void NewGame()
        {
            _player = new Point(0.05f, 0.5f);
            _enemy  = new Point(0.95f, 0.5f);
        }

        public override void EndGame()
        {
        }

        public override void Update(long dt, Point screenSize, DeviceContext context, TargetBase target)
        {
            var playerBox = _player.ToBox(new Point(0.05f, 0.3f));
            var enemyBox = _enemy.ToBox(new Point(0.05f, 0.3f));

            context.FillRectangle(screenSize.ApplyTo(playerBox), GameColors.PlayerColor);
            context.FillRectangle(screenSize.ApplyTo(enemyBox), GameColors.EnemyColor);
        }

        public override void KeyPressed(InputType key)
        {
        }

        public override void KeyReleased(InputType key)
        {
        }
    }
}
