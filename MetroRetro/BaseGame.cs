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

        protected void Debug(string text)
        {
            _gameManager.Page.AddDebugText(text);
        }

        protected void DrawBoardBorder(DeviceContext context, DeviceManager deviceManager, Point screenSize, float dt)
        {
            context.DrawRectangle(screenSize.ApplyTo(GamesParams.Margin0.ToRectangleWith(GamesParams.Margin1)), GamesParams.ObstaclesColor, 2);

            _gameTime += dt;

            const float u = 0.25f;
            if (_gameTime < 2 * u)
            {
                var op = _gameTime < u ? _gameTime / u : 1 - (_gameTime - u) * u;
                var hideBrush = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White, new BrushProperties { Opacity = op });
                context.FillRectangle(screenSize.ApplyTo(new RectangleF(0, 0, 1, 1)), hideBrush);
            }

            _gameManager.Page.SetTimeRectangleWidth(_gameTime / _gameMaxTime);
         /*   float x = _gameTime / _gameMaxTime;
            var xxx = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White, new BrushProperties { Opacity = 0.25f });
            context.FillRectangle(screenSize.ApplyTo(new RectangleF(
                GamesParams.MarginX0, 
                GamesParams.MarginY0 - 0.06f,
                GamesParams.MarginX0 * x + GamesParams.MarginX1 * (1 - x),
                GamesParams.MarginY0 - 0.014f)), xxx);
            */
            if (_gameTime > _gameMaxTime)
                _gameManager.Interrupt();
        }

        public virtual void NewGame()
        {
            _gameTime = 0;
        }

        public abstract void EndGame();
        public abstract void Update(DeviceContext context, TargetBase target, DeviceManager deviceManager, Point screenSize, float dt, float elapsedTime);
        public abstract void KeyPressed(InputType key);
        public abstract void KeyReleased(InputType key);
    }
}
