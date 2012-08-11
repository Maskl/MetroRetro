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

        public abstract void NewGame();
        public abstract void EndGame();
        public abstract void Update(long dt, Point screenSize, DeviceContext context, TargetBase target);
        public abstract void KeyPressed(InputType key);
        public abstract void KeyReleased(InputType key);
    }
}
