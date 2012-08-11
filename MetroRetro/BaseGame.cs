using CommonDX;
using MetroRetro.Games;
using SharpDX;
using SharpDX.Direct2D1;
namespace MetroRetro
{
    abstract class BaseGame
    {
        protected GameManager _gameManager;

        protected BaseGame(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        protected void Debug(string text)
        {
            _gameManager.Page.AddDebugText(text);
        }

        public abstract void NewGame();
        public abstract void EndGame();
        public abstract void Update(DeviceContext context, TargetBase target, Point screenSize, float dt, float elapsedTime);
        public abstract void KeyPressed(InputType key);
        public abstract void KeyReleased(InputType key);
    }
}
