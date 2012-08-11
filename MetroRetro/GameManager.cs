using System.Collections.Generic;
using CommonDX;
using MetroRetro.Games;
using SharpDX.Direct2D1;

namespace MetroRetro
{
    public class GameManager
    {
        public MainPage Page { get; set; }
        public Renderer Renderer { get; set; }

        private Dictionary<GameType, BaseGame> _games;
        private BaseGame _currentGame;

        public void Create(MainPage mainPage, Renderer renderer)
        {
            Page = mainPage;
            Renderer = renderer;

            _games = new Dictionary<GameType, BaseGame>
                         {
                             {GameType.Test, new Test(this)},
                             {GameType.Pong, new Pong(this)}
                         };

            _currentGame = null;
        }

        public void HandleInput(InputType inputType, InputState state)
        {
            Page.AddDebugText(inputType + " " + state);
        }

        public void Start(GameType game)
        {
            if (_currentGame != null)
                _currentGame.EndGame();

            _currentGame = _games[game];
            _currentGame.NewGame();
        }

        public void Update(long dt, TargetBase target, DeviceManager deviceManager)
        {
            if (_currentGame == null)
                return;

            var screenSize = new Point((float)target.RenderTargetBounds.Width, (float)target.RenderTargetBounds.Height);

            var context2D = target.DeviceManager.ContextDirect2D;
            context2D.BeginDraw();
            context2D.Clear(GameColors.BackgroundColorNormal);
            context2D.TextAntialiasMode = TextAntialiasMode.Grayscale;

            _currentGame.Update(dt, screenSize, context2D, target);

            context2D.EndDraw();
        }

        public void StartFirstGame()
        {
            Start(GameType.Test);
        }
    }

    public enum GameType
    {
        Test,
        Pong
    }

    public enum InputType
    {
        Up,
        Down,
        Left,
        Right,
        Space
    }

    public enum InputState
    {
        Pressed,
        Released,
        Over,
        Out
    }
}
