using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MetroRetro
{
    public sealed partial class MainPage
    {
        public Dictionary<InputType, Border> ButtonsDictionary;
        public Dictionary<Border, Color> ButtonsLastColors;

        public MainPage()
        {
            InitializeComponent();

            ButtonsDictionary = new Dictionary<InputType, Border>
                                    {
                                        {InputType.Space, SpaceButton},
                                        {InputType.Up, UpArrowButton},
                                        {InputType.Down, DownArrowButton},
                                        {InputType.Left, LeftArrowButton},
                                        {InputType.Right, RightArrowButton}
                                    };

            ButtonsLastColors = new Dictionary<Border, Color>
                                    {
                                        {SpaceButton, Colors.White},
                                        {UpArrowButton, Colors.White},
                                        {DownArrowButton, Colors.White},
                                        {LeftArrowButton, Colors.White},
                                        {RightArrowButton, Colors.White}
                                    };
        }

        public void ButtonPressedOrReleased(Border button, InputState state)
        {
            var background = Colors.White;
            var foreground = Colors.Black;
            switch (state)
            {
                case InputState.Pressed:
                    background = Colors.Green;
                break;

                case InputState.Released:
                    background = ButtonsLastColors[button];
                break;

                case InputState.Over:
                    background = Colors.Yellow;
                    ButtonsLastColors[button] = background;
                break;

                case InputState.Out:
                    background = Colors.White;
                    ButtonsLastColors[button] = background;
                break;
            }


            button.Background = new SolidColorBrush(background);
            button.BorderBrush = new SolidColorBrush(foreground);
        }

        private readonly List<string> _debugTexts = new List<string>(); 
        public void AddDebugText(string text)
        {
            _debugTexts.Add(text);
            if (_debugTexts.Count > 10)
                _debugTexts.RemoveAt(0);

            DebugText.Text = _debugTexts.Aggregate("", (current, debugText) => current + (debugText + "\n"));
        }

        public void HandleInput(InputType inputType, InputState state)
        {
            ButtonPressedOrReleased(ButtonsDictionary[inputType], state);
            AddDebugText(inputType + " " + state);
        }

        private void InputButtonPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var inputType = ButtonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Pressed);
        }

        private void InputButtonPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var inputType = ButtonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Released);
        }

        private void InputButtonPointerOver(object sender, PointerRoutedEventArgs e)
        {
            var inputType = ButtonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Over);
        }

        private void InputButtonPointerOut(object sender, PointerRoutedEventArgs e)
        {
            var inputType = ButtonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Out);
        }

        private void SwapChainBackgroundPanelKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var possibleInput = KeyToPossibleInputType(e.Key);
            if (possibleInput == null)
                return;

            HandleInput((InputType)possibleInput, InputState.Pressed);
        }

        private void SwapChainBackgroundPanelKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var possibleInput = KeyToPossibleInputType(e.Key);
            if (possibleInput == null)
                return;

            HandleInput((InputType)possibleInput, InputState.Released);
        }

        public InputType? KeyToPossibleInputType(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Space:
                case VirtualKey.Enter:
                case VirtualKey.Z:
                case VirtualKey.X:
                case VirtualKey.LeftControl:
                case VirtualKey.RightControl:
                    return InputType.Space;

                case VirtualKey.Up:
                case VirtualKey.W:
                    return InputType.Up;

                case VirtualKey.Down:
                case VirtualKey.S:
                    return InputType.Down;

                case VirtualKey.Left:
                case VirtualKey.A:
                    return InputType.Left;

                case VirtualKey.Right:
                case VirtualKey.D:
                    return InputType.Right;
            }

            return null;
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
