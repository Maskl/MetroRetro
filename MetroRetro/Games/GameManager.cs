using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroRetro.Games
{
    public class GameManager
    {
        public MainPage Page { get; set; }
        public MetroRetroRenderer Renderer { get; set; }

        public void HandleInput(InputType inputType, InputState state)
        {
            Page.AddDebugText(inputType + " " + state);
        }

        public void Create(MainPage mainPage, MetroRetroRenderer renderer)
        {
            Page = mainPage;
            Renderer = renderer;
        }
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
