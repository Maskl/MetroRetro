// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MetroRetro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SwapChainBackgroundPanel
    {


        public Dictionary<PossibleInputTypes, Border> ButtonsDictionary;

        public MainPage()
        {
            InitializeComponent();

            ButtonsDictionary = new Dictionary<PossibleInputTypes, Border> {{PossibleInputTypes.Space, SpaceButton}};
        }

        public void ButtonPressedOrReleased(Border button, bool pressed)
        {
            var background = pressed ? Colors.Yellow : Colors.White;
            var foreground = pressed ? Colors.Blue   : Colors.Black;

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

        public void HandleInput(PossibleInputTypes inputType, bool pressed)
        {
            ButtonPressedOrReleased(ButtonsDictionary[inputType], pressed);
            AddDebugText(inputType + " " + pressed);
        }

        private void SpaceButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            HandleInput(PossibleInputTypes.Space, true);
        }

        private void SpaceButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            HandleInput(PossibleInputTypes.Space, false);
        }

        private void UpArrowButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void UpArrowButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void DownArrowButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void DownArrowButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void LeftArrowButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void LeftArrowButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void RightArrowButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void RightArrowButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void SwapChainBackgroundPanel_KeyDown_1(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var possibleInput = KeyToPossibleInputType(e.Key);
            if (possibleInput == null)
                return;
            
            HandleInput((PossibleInputTypes)possibleInput, true);
        }

        private void SwapChainBackgroundPanel_KeyUp_1(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var possibleInput = KeyToPossibleInputType(e.Key);
            if (possibleInput == null)
                return;

            HandleInput((PossibleInputTypes)possibleInput, false);
        }

        public PossibleInputTypes? KeyToPossibleInputType(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Space:
                case VirtualKey.Enter:
                case VirtualKey.Z:
                case VirtualKey.X:
                case VirtualKey.LeftControl:
                case VirtualKey.RightControl:
                    return PossibleInputTypes.Space;

                case VirtualKey.Up:
                case VirtualKey.W:
                    return PossibleInputTypes.Up;

                case VirtualKey.Down:
                case VirtualKey.S:
                    return PossibleInputTypes.Down;

                case VirtualKey.Left:
                case VirtualKey.A:
                    return PossibleInputTypes.Left;

                case VirtualKey.Right:
                case VirtualKey.D:
                    return PossibleInputTypes.Right;
            }

            return null;
        }

    }

    public enum PossibleInputTypes
    {
        Up,
        Down,
        Left,
        Right,
        Space
    }
}
