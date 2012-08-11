using System;
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

        public void HandleInput(InputType key, InputState state)
        {
            if (_currentGame == null)
                return;

            if (state == InputState.Released && key == InputType.Next)
            {
                StartNextGame();
                return;
            }

            if (state == InputState.Pressed)
                _currentGame.KeyPressed(key);

            if (state == InputState.Released)
                _currentGame.KeyReleased(key);
        }

        public void Start(GameType game)
        {
            if (_currentGame != null)
                _currentGame.EndGame();

            _currentGame = _games[game];
            _currentGame.NewGame();
        }

        private float oldElapsedTimeF;
        public void Update(long elapsedTime, TargetBase target, DeviceManager deviceManager)
        {
            if (_currentGame == null)
                return;

            var screenSize = new Point((float)target.RenderTargetBounds.Width, (float)target.RenderTargetBounds.Height);

            var context2D = target.DeviceManager.ContextDirect2D;
            context2D.BeginDraw();
            context2D.Clear(GamesParams.BackgroundColorNormal);
            context2D.TextAntialiasMode = TextAntialiasMode.Grayscale;

            var elapsedTimeF = elapsedTime / 1000.0f;
            var dt = elapsedTimeF - oldElapsedTimeF;
            oldElapsedTimeF = elapsedTimeF;
            _currentGame.Update(context2D, target, screenSize, dt, elapsedTimeF);

            context2D.EndDraw();
        }

        public void StartFirstGame()
        {
            Start(GameType.Pong);
        }

        public void StartNextGame()
        {
            var rand = new Random();
            Start( (GameType) rand.Next((int)GameType.GamesCount) );
        }
    }

    public enum GameType
    {
        Test = 0,
        Pong = 1,
        GamesCount = 2
    }

    public enum InputType
    {
        Up,
        Down,
        Left,
        Right,
        Space,
        Next
    }

    public enum InputState
    {
        Pressed,
        Released,
        Over,
        Out
    }
}
