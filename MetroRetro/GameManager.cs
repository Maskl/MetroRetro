using System;
using System.Collections.Generic;
using System.Globalization;
using CommonDX;
using MetroRetro.Games;
using SharpDX.Direct2D1;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MetroRetro
{
    public class GameManager
    {
        public MainPage Page { get; set; }
        public Renderer Renderer { get; set; }
        public bool IsPause { get; set; }
        public bool IsTraining { get; set; }
        public static int Record { get; set; }

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
                             {GameType.Pong, new Pong(this, 20)},
                             {GameType.Arkanoid, new Arkanoid(this, 20)},
                             {GameType.Snake, new Snake(this, 20)},
                             {GameType.Tigers, new Tigers(this, 20)},
                             {GameType.MoonPatrol, new MoonPatrol(this, 20)}
                         };

            _currentGame = null;

            LoadRecord();
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

            if (!IsTraining && _currentGame.PlayedInThisSession)
            {
                _currentGame.ContinueGame();
            }
            else
            {
                _currentGame.NewGame();
                _currentGame.PlayedInThisSession = true;
            }

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

            var elapsedTimeF = elapsedTime / 1000.0f;
            var dt = elapsedTimeF - _oldElapsedTimeF;
            _oldElapsedTimeF = elapsedTimeF;
            _currentGame.Update(context2D, target, deviceManager, screenSize, dt, elapsedTimeF);

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
            var dialog = new MessageDialog("Tap one of the buttons below.", "Pause");

            var ans = 0;
            var cmd1 = new UICommand("Resume", cmd => ans = 1, 1);
            var cmd2 = new UICommand("Next game", cmd => ans = 2, 2);
            var cmd3 = new UICommand("Back to menu", cmd => ans = 3, 3);

            dialog.Commands.Add(cmd1);

            if (IsTraining)
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

            var text = Points + " points\n";

            if (Record < Points)
            {
                text += "YOUR BEST RESULT!";
                if (Record > 0)
                    text += "\n(Previous record: " + Record + ")";

                SaveRecord(Points);
            }
            else
            {
                text += "(Record to beat: " + Record + ")";
            }

            var dialog = new MessageDialog(text, "End of the game");

            var ans = 0;
            var cmd1 = new UICommand("Ok", cmd => ans = 1, 1);

            dialog.Commands.Add(cmd1);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            if (ans == 1)
                StartSession();
        }

        public async void StartSession()
        {
            Pause(false);
            var dialog = new MessageDialog("Choose one of the game modes below.", "Hi!");

            var ans = 0;
            var cmd1 = new UICommand("Training", cmd => ans = 1, 1);
            var cmd2 = new UICommand("Fast game", cmd => ans = 2, 2);

            dialog.Commands.Add(cmd1);
            dialog.Commands.Add(cmd2);
            dialog.DefaultCommandIndex = 0;

            await dialog.ShowAsync();

            IsTraining = ans == 1;

            Points = 0;
            Lifes = 3;

            foreach (var game in _games)
            {
                game.Value.PlayedInThisSession = false;
            }

            if (IsTraining)
            {
                Page.SetTimeBorderVisibility(Visibility.Collapsed);
            }
            else
            {
                Page.SetTimeBorderVisibility(Visibility.Visible);
            }

            Start(GameType.GamesCount - 1);
            Unpause();

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

        public void Win(int points)
        {
            Points += points;
            RedrawPointsAndLifes();
            if (!IsTraining)
                StartNextGame();
        }

        public void Die()
        {
            if (--Lifes <= 0)
            {
                RedrawPointsAndLifes();
                
                if (!IsTraining)
                    EndSession();
                else
                    StartNextGame();
            }
            else
            {
                RedrawPointsAndLifes();
                
                if (!IsTraining)
                    StartNextGame();
            }
        }

        public void Interrupt()
        {
            StartNextGame();
        }

        public void RedrawPointsAndLifes()
        {
            var text = "";
            for (var i = 0; i < Lifes; i++)
                text += "❤";

            Page.SetPointsText(Points.ToString(/*"D8"*/));
            Page.SetLifesText(text);
        }


        static public async void SaveRecord(int record)
        {
            Record = record;

            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync("metroretrorecord.xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, record.ToString());
            }
            catch (Exception)
            {
            }
        }

        static public async void LoadRecord()
        {
            var recordInt = 0;
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("metroretrorecord.xml");
                var record = await FileIO.ReadTextAsync(file);
                recordInt = Convert.ToInt32(record);
            }
            catch (Exception)
            {
            }

            Record = recordInt;
        }
    }

    public enum GameType
    {
        Pong = 0,
        Arkanoid = 1,
        Snake = 2,
        Tigers = 3,
        MoonPatrol = 4,
        GamesCount = 5
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
