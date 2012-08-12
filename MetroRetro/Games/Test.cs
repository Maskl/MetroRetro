using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Matrix = SharpDX.Matrix;

namespace MetroRetro.Games
{
    class Test : BaseGame
    {
        public Test(GameManager gameManager, float maxTime) : base(gameManager, maxTime)
        {
        }

        public override void NewGame()
        {
            base.NewGame();
        }

        public override void EndGame()
        {
        }
        
        private TextFormat _textFormat;
        private PathGeometry1 _pathGeometry1;
        public override void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime)
        {
            var centerX = screenSize.X / 2;
            var centerY = screenSize.Y / 2;

            if (_textFormat == null)
            {
                // Initialize a TextFormat
                _textFormat = new TextFormat(target.DeviceManager.FactoryDirectWrite, "Calibri", 96 * screenSize.X / 1920) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };
            }

            if (_pathGeometry1 == null)
            {
                var sizeShape = screenSize.X / 4.0f;

                // Creates a random geometry inside a circle
                _pathGeometry1 = new PathGeometry1(target.DeviceManager.FactoryDirect2D);
                var pathSink = _pathGeometry1.Open();
                var startingPoint = new DrawingPointF(sizeShape * 0.5f, 0.0f);
                pathSink.BeginFigure(startingPoint, FigureBegin.Hollow);
                for (var i = 0; i < 128; i++)
                {
                    var angle = (float)i / 128.0f * (float)Math.PI * 2.0f;
                    var R = (float)(Math.Cos(angle) * 0.1f + 0.4f);
                    R *= sizeShape;
                    var point1 = new DrawingPointF(R * (float)Math.Cos(angle), R * (float)Math.Sin(angle));

                    if ((i & 1) > 0)
                    {
                        R = (float)(Math.Sin(angle * 6.0f) * 0.1f + 0.9f);
                        R *= sizeShape;
                        point1 = new DrawingPointF(R * (float)Math.Cos(angle + Math.PI / 12), R * (float)Math.Sin(angle + Math.PI / 12));
                    }
                    pathSink.AddLine(point1);
                }
                pathSink.EndFigure(FigureEnd.Open);
                pathSink.Close();
            }

            context.TextAntialiasMode = TextAntialiasMode.Grayscale;
            var t = elapsedTime;

            context.Transform = Matrix.RotationZ((float)(Math.Cos(t * 2.0f * Math.PI * 0.5f))) * Matrix.Translation(centerX, centerY, 0);

            context.DrawText("SharpDX\nDirect2D1\nDirectWrite", _textFormat, new RectangleF(-screenSize.X / 2.0f, -screenSize.Y / 2.0f, +screenSize.X / 2.0f, screenSize.Y / 2.0f), GamesParams.AdditionalColor);

            var scaling = (float)(Math.Cos(t * 2.0 * Math.PI * 0.25) * 0.5f + 0.5f) * 0.5f + 0.5f;
            context.Transform = Matrix.Scaling(scaling) * Matrix.RotationZ(t * 1.5f) * Matrix.Translation(centerX, centerY, 0);

            context.DrawGeometry(_pathGeometry1, GamesParams.EnemyColor, 2.0f);

            DrawBoardBorder(context, deviceManager, screenSize, dt);
        }

        public override void KeyPressed(InputType key)
        {
        }

        public override void KeyReleased(InputType key)
        {
        }
    }
}
