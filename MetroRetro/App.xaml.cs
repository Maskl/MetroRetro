using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonDX;
using MetroRetro.Games;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MetroRetro
{
    sealed partial class App
    {
        private DeviceManager _deviceManager;
        private SwapChainBackgroundPanelTarget _target;
        private Renderer _renderer;
        private GameManager _gameManager;
        private MainPage _mainPage;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create Game Manager
            _gameManager = new GameManager();

            // Place the frame in the current Window and ensure that it is active
            _mainPage = new MainPage(_gameManager);
            Window.Current.Content = _mainPage;
            Window.Current.Activate();

            // Safely dispose any previous instance
            // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
            _deviceManager = new DeviceManager();

            // New CubeRenderer
            _renderer = new Renderer(_gameManager);
            var fpsRenderer = new FpsRenderer();

            // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
            _target = new SwapChainBackgroundPanelTarget(_mainPage);

            // Add Initializer to device manager
            _deviceManager.OnInitialize += _target.Initialize;
            _deviceManager.OnInitialize += _renderer.Initialize;
            _deviceManager.OnInitialize += fpsRenderer.Initialize;

            // Render the cube within the CoreWindow
            _target.OnRender += _renderer.Render;
            _target.OnRender += fpsRenderer.Render;

            // Initialize the device manager and all registered deviceManager.OnInitialize 
            _deviceManager.Initialize(DisplayProperties.LogicalDpi);

            // Setup rendering callback
            CompositionTarget.Rendering += CompositionTargetRendering;

            // Callback on DpiChanged
            DisplayProperties.LogicalDpiChanged += DisplayPropertiesLogicalDpiChanged;

            // Create colors which will be use in all games.
            GamesParams.CreateColors(_deviceManager);
            
            // Create Game Manager
            _gameManager.Create(_mainPage, _renderer);

            // And start first game
            _gameManager.StartSession();
        }

        void DisplayPropertiesLogicalDpiChanged(object sender)
        {
            _deviceManager.Dpi = DisplayProperties.LogicalDpi;
        }

        void CompositionTargetRendering(object sender, object e)
        {
            if (_gameManager.IsPause)
                return;

            _target.RenderAll();
            _target.Present();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }
    }
}
