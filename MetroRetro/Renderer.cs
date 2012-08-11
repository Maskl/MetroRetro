using System;
using System.Diagnostics;
using CommonDX;
using MetroRetro.Games;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Matrix = SharpDX.Matrix;

namespace MetroRetro
{
    public class Renderer
    {
        private Stopwatch _clock;
        private readonly GameManager _gameManager;
        private DeviceManager _deviceManager;

        public bool Show { get; set; }

        public Renderer(GameManager gameManager)
        {
            _gameManager = gameManager;
            Show = true;
        }

        public virtual void Initialize(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _clock = Stopwatch.StartNew();
            _clock.Stop();
        }

        public virtual void Render(TargetBase target)
        {
            if (!Show)
                return;

            _gameManager.Update(_clock.ElapsedMilliseconds, target, _deviceManager);
        }

        public void Pause()
        {
            _clock.Stop();
        }

        public void Unpause()
        {
            _clock.Start();
        }
    }
}
