using System;
using System.Collections.Generic;
using System.Globalization;
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
        public bool IsTraining { get; set; }

        public int Points { get; set; }
        public int Lifes { get; set; }

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
                    TogglePause(true);
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

            RedrawPointsAndLifes();
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

        public void TogglePause(bool showDialog)
        {
            if (IsPause)
                Unpause();
            else
                Pause(showDialog);
        }

        public void Pause(bool showDialog)
        {
            if (IsPause)
                return;
            
            IsPause = true;

            Renderer.Pause();
            if (showDialog)
                ShowPauseDialog();
        }

        public void Unpause()
        {
            if (!IsPause)
                return;

            IsPause = false;
            Renderer.Unpause();
        }

        private async void ShowPauseDialog()
        {
            var dialog = new MessageDialog("Click one of the buttons below.", "Pause");

            var ans = 0;
            var cmd1 = new UICommand("Resume", cmd => ans = 1, 1);
            var cmd2 = new UICommand("Next game", cmd => ans = 2, 2);
            var cmd3 = new UICommand("Back to menu", cmd => ans = 3, 3);

            dialog.Commands.Add(cmd1);
            dialog.Commands.Add(cmd2);
            dialog.Commands.Add(cmd3);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();
            
            if (ans == 1)
                Unpause();

            if (ans == 2)
                StartNextGame();

            if (ans == 3)
                EndSession();
        }

        public async void EndSession()
        {
            Pause(false);

            var text = "Your result: 56411 points\nYOUR RECORD!";
            var dialog = new MessageDialog(text, "End of the game");

            var ans = 0;
            var cmd1 = new UICommand("Back to menu", cmd => ans = 1, 1);

            dialog.Commands.Add(cmd1);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            if (ans == 1)
                StartSession();
        }

        public async void StartSession()
        {
            Pause(false);
            var dialog = new MessageDialog("Tap one of the buttons below.\n" +
                                           "In training mode you will need to change game manually (Button on the top)", "Hi!");

            var ans = 0;
            var cmd1 = new UICommand("Training", cmd => ans = 1, 1);
            var cmd2 = new UICommand("Fast game", cmd => ans = 2, 2);

            dialog.Commands.Add(cmd1);
            dialog.Commands.Add(cmd2);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            IsTraining = ans == 1;
                
            Start(GameType.Pong);
            Unpause();

            Points = 0;
            Lifes = 3;
        }

        public void StartNextGame()
        {
            Pause(false);
            var rand = new Random();
            Start((GameType)rand.Next((int)GameType.GamesCount));
            Unpause();
        }

        public void AddPoints(int points)
        {
            Points += points;
            RedrawPointsAndLifes();
        }

        public void Dead(int points)
        {
            if (--Lifes <= 0)
            {
                EndSession();
            }
            else
            {
                RedrawPointsAndLifes();
                StartNextGame();
            }
        }

        private void RedrawPointsAndLifes()
        {
            var text = Points.ToString(new NumberFormatInfo {NumberDecimalDigits = 8});
            text += " ";
            for (var i = 0; i < Lifes; i++)
                text += "❤";

            Page.SetPointsText(text);
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
