using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;

namespace MetroRetro.Games
{
    public class GameManager
    {
        public MainPage Page { get; set; }
        public Renderer Renderer { get; set; }

        private Dictionary<GameType, IGame> _games;
        private IGame _currentGame;

        public void Create(MainPage mainPage, Renderer renderer)
        {
            Page = mainPage;
            Renderer = renderer;

            _games = new Dictionary<GameType, IGame>
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

            _currentGame.Update(dt, target, deviceManager);
        }

        public void StartFirstGame()
        {
            Start(GameType.Pong);
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
