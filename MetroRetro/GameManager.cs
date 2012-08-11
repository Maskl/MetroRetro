using System;
using System.Collections.Generic;
using CommonDX;
using MetroRetro.Games;
using SharpDX.Direct2D1;
using Windows.UI.Popups;

namespace MetroRetro
{
    public class GameManager
    {
        public MainPage Page { get; set; }
        public Renderer Renderer { get; set; }
        public bool IsPause { get; set; }

        private Dictionary<GameType, BaseGame> _games;
        private BaseGame _currentGame;

        public void Create(MainPage mainPage, Renderer renderer)
        {
            Page = mainPage;
            Renderer = renderer;
            IsPause = false;

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

            if (state == InputState.Released)
            {
                if (key == InputType.Next)
                {
                    StartNextGame();
                    return;
                }

                if (key == InputType.Pause)
                {
                    TogglePause();
                    return;
                }
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

        private float _oldElapsedTimeF;
        public void Update(long elapsedTime, TargetBase target, DeviceManager deviceManager)
        {
            if (_currentGame == null || IsPause)
                return;

            var screenSize = new Point((float)target.RenderTargetBounds.Width, (float)target.RenderTargetBounds.Height);

            var context2D = target.DeviceManager.ContextDirect2D;
            context2D.BeginDraw();
            context2D.Clear(GamesParams.BackgroundColorNormal);
         //   context2D.TextAntialiasMode = TextAntialiasMode.Grayscale;

            var elapsedTimeF = elapsedTime / 1000.0f;
            var dt = elapsedTimeF - _oldElapsedTimeF;
            _oldElapsedTimeF = elapsedTimeF;
            _currentGame.Update(context2D, target, screenSize, dt, elapsedTimeF);

            context2D.EndDraw();
        }

        public void TogglePause()
        {
            IsPause = !IsPause;

            if (!IsPause)
            {
                Renderer.Unpause();
                return;
            }

            Renderer.Pause();
            ShowPauseDialog();
        }

        private async void ShowPauseDialog()
        {
            var dialog = new MessageDialog("Paused. Click button below to play.", "Pause");

            var ans = 0;
            var cmd1 = new UICommand("Next game", cmd => ans = 1, 1);
            var cmd2 = new UICommand("Resume", cmd => ans = 2, 2);

            dialog.Commands.Add(cmd1);
            dialog.Commands.Add(cmd2);
            dialog.DefaultCommandIndex = 1;

            await dialog.ShowAsync();

            TogglePause();
            
            if (ans == 1)
                StartNextGame();
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
        Next,
        Pause,
        Unpause
    }

    public enum InputState
    {
        Pressed,
        Released,
        Over,
        Out
    }
}
