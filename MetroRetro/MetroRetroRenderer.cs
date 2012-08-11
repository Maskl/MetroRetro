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
    public class MetroRetroRenderer
    {
        private Stopwatch _clock;
        private readonly GameManager _gameManager;
        private DeviceManager _deviceManager;

        public bool Show { get; set; }

        public MetroRetroRenderer(GameManager gameManager)
        {
            _gameManager = gameManager;
            Show = true;
        }

        public virtual void Initialize(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _clock = Stopwatch.StartNew();
        }

        public virtual void Render(TargetBase target)
        {
            if (!Show)
                return;

            _gameManager.Update(_clock.ElapsedMilliseconds, target, _deviceManager);
        }
    }
}
