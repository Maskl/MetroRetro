using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Matrix = SharpDX.DirectWrite.Matrix;

namespace MetroRetro.Games
{
    class Pong : IGame
    {
        private GameManager _gameManager;

        public Pong(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void NewGame()
        {
        }

        public void EndGame()
        {
        }
        
        private TextFormat _textFormat;
        private PathGeometry1 _pathGeometry1;

        public void Update(long dt, TargetBase target, DeviceManager deviceManager)
        {
            Brush sceneColorBrush = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White);

            var context2D = target.DeviceManager.ContextDirect2D;

            context2D.BeginDraw();

            context2D.Clear(Colors.Black);

            var sizeX = (float)target.RenderTargetBounds.Width;
            var sizeY = (float)target.RenderTargetBounds.Height;
            var globalScaling = SharpDX.Matrix.Scaling(Math.Min(sizeX, sizeY));

            var centerX = (float)(target.RenderTargetBounds.X + sizeX / 2.0f);
            var centerY = (float)(target.RenderTargetBounds.Y + sizeY / 2.0f);

            if (_textFormat == null)
            {
                // Initialize a TextFormat
                _textFormat = new TextFormat(target.DeviceManager.FactoryDirectWrite, "Calibri", 96 * sizeX / 1920) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };
            }

            if (_pathGeometry1 == null)
            {
                var sizeShape = sizeX / 4.0f;

                // Creates a random geometry inside a circle
                _pathGeometry1 = new PathGeometry1(target.DeviceManager.FactoryDirect2D);
                var pathSink = _pathGeometry1.Open();
                var startingPoint = new DrawingPointF(sizeShape * 0.5f, 0.0f);
                pathSink.BeginFigure(startingPoint, FigureBegin.Hollow);
                for (int i = 0; i < 128; i++)
                {
                    float angle = (float)i / 128.0f * (float)Math.PI * 2.0f;
                    float R = (float)(Math.Cos(angle) * 0.1f + 0.4f);
                    R *= sizeShape;
                    DrawingPointF point1 = new DrawingPointF(R * (float)Math.Cos(angle), R * (float)Math.Sin(angle));

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

            context2D.TextAntialiasMode = TextAntialiasMode.Grayscale;
            float t = dt / 1000.0f;

            context2D.Transform = SharpDX.Matrix.RotationZ((float)(Math.Cos(t * 2.0f * Math.PI * 0.5f))) * SharpDX.Matrix.Translation(centerX, centerY, 0);

            context2D.DrawText("SharpDX\nDirect2D1\nDirectWrite", _textFormat, new RectangleF(-sizeX / 2.0f, -sizeY / 2.0f, +sizeX/2.0f, sizeY/2.0f), sceneColorBrush);

            float scaling = (float)(Math.Cos(t * 2.0 * Math.PI * 0.25) * 0.5f + 0.5f) * 0.5f + 0.5f;
            context2D.Transform = SharpDX.Matrix.Scaling(scaling) * SharpDX.Matrix.RotationZ(t * 1.5f) * SharpDX.Matrix.Translation(centerX, centerY, 0);

            context2D.DrawGeometry(_pathGeometry1, sceneColorBrush, 2.0f);


            context2D.DrawRectangle(new RectangleF(10, 10, 200, 200), sceneColorBrush );
            context2D.EndDraw();
        }

        public void KeyPressed(InputType key)
        {
        }

        public void KeyReleased(InputType key)
        {
        }
    }
}
