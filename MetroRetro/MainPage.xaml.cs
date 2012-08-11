using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroRetro.Games;
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
        private readonly Dictionary<InputType, Border> _buttonsDictionary;
        private readonly Dictionary<Border, Color> _buttonsLastColors;
        private GameManager _gameManager;

        public MainPage(GameManager gameManager)
        {
            InitializeComponent();
            
            _buttonsDictionary = new Dictionary<InputType, Border>
                                    {
                                        {InputType.Space, SpaceButton},
                                        {InputType.Up, UpArrowButton},
                                        {InputType.Down, DownArrowButton},
                                        {InputType.Left, LeftArrowButton},
                                        {InputType.Right, RightArrowButton},
                                        {InputType.Next, NextButton}
                                    };

            _buttonsLastColors = new Dictionary<Border, Color>
                                    {
                                        {SpaceButton,  Colors.White},
                                        {UpArrowButton, Colors.White},
                                        {DownArrowButton, Colors.White},
                                        {LeftArrowButton, Colors.White},
                                        {RightArrowButton, Colors.White},
                                        {NextButton, Colors.White}
                                    };

            _gameManager = gameManager;
        }

        // Sets colors to buttons depends on event type.
        private void ButtonPressedOrReleased(Border button, InputState state)
        {
            var background = Colors.White;
            var foreground = Colors.Black;
            switch (state)
            {
                case InputState.Pressed:
                    background = Colors.Green;
                break;

                case InputState.Released:
                    background = _buttonsLastColors[button];
                break;

                case InputState.Over:
                    background = Colors.Yellow;
                    _buttonsLastColors[button] = background;
                break;

                case InputState.Out:
                    background = Colors.White;
                    _buttonsLastColors[button] = background;
                break;
            }

            button.Background = new SolidColorBrush(background);
            button.BorderBrush = new SolidColorBrush(foreground);
        }

        // Pass input event to GameManager
        private void HandleInput(InputType inputType, InputState state)
        {
            _gameManager.HandleInput(inputType, state);

            if (_buttonsDictionary.ContainsKey(inputType))
                ButtonPressedOrReleased(_buttonsDictionary[inputType], state);
        }

        // Input methods:
        private void InputButtonPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var inputType = _buttonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Pressed);
        }

        private void InputButtonPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var inputType = _buttonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Released);
        }

        private void InputButtonPointerOver(object sender, PointerRoutedEventArgs e)
        {
            var inputType = _buttonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
            HandleInput(inputType, InputState.Over);
        }

        private void InputButtonPointerOut(object sender, PointerRoutedEventArgs e)
        {
            var inputType = _buttonsDictionary.FirstOrDefault(x => x.Value == sender as Border).Key;
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

                case VirtualKey.F3:
                case VirtualKey.N:
                    return InputType.Next;

                case VirtualKey.Pause:
                case VirtualKey.P:
                    return InputType.Pause;
            }

            return null;
        }

        // Simply method to show debug informations.
        private readonly List<string> _debugTexts = new List<string>();
        public void AddDebugText(string text)
        {
            _debugTexts.Add(text);
            if (_debugTexts.Count > 10)
                _debugTexts.RemoveAt(0);

            DebugText.Text = _debugTexts.Aggregate("", (current, debugText) => current + (debugText + "\n"));
        }
    }
}
