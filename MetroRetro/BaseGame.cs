using CommonDX;
using MetroRetro.Games;
using SharpDX;
using SharpDX.Direct2D1;
namespace MetroRetro
{
    abstract class BaseGame
    {
        protected GameManager _gameManager;
        private float _gameTime;
        private float _gameMaxTime;

        protected BaseGame(GameManager gameManager, float gameMaxTime)
        {
            _gameManager = gameManager;
            _gameMaxTime = gameMaxTime;
            _gameTime = 0;
        }

        public bool PlayedInThisSession { get; set; }

        protected void Debug(string text)
        {
            _gameManager.Page.AddDebugText(text);
        }

        protected void DrawBoardBorder(DeviceContext context, DeviceManager deviceManager, Point screenSize, float dt)
        {
            context.DrawRectangle(screenSize.ApplyTo(GamesParams.Margin0.ToRectangleWith(GamesParams.Margin1)), GamesParams.ObstaclesColor, 2);

            _gameTime += dt;

            if (!_gameManager.IsTraining)
            {
                const float u = 1.00f;
                if (_gameTime < u)
                {
                    var op = 1 - _gameTime/u;
                    var hideBrush = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White,
                                                        new BrushProperties {Opacity = op});
                    context.FillRectangle(screenSize.ApplyTo(new RectangleF(0, 0, 1, 1)), hideBrush);
                }

                if (_gameTime > _gameMaxTime - u)
                {
                    var op = 1 - (_gameMaxTime - _gameTime)/u;
                    var hideBrush = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White,
                                                        new BrushProperties {Opacity = op});
                    context.FillRectangle(screenSize.ApplyTo(new RectangleF(0, 0, 1, 1)), hideBrush);
                }

                float bar;
                bar = _gameManager.IsTraining ? 1 : _gameTime/_gameMaxTime;
                _gameManager.Page.SetTimeRectangleWidth(bar);
                _gameManager.RedrawPointsAndLifes();

                if (_gameTime > _gameMaxTime)
                    _gameManager.Interrupt();
            }
        }

        public virtual void NewGame()
        {
            _gameTime = 0;
        }

        public abstract void EndGame();
        public abstract void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime);
        public abstract void KeyPressed(InputType key);
        public abstract void KeyReleased(InputType key);
        public virtual void ContinueGame()
        {
            _gameTime = 0;
        }
    }
}
