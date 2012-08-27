using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
using Windows.UI;
using Colors = SharpDX.Colors;

namespace MetroRetro
{
    static public class GamesParams
    {
        public static float Margin = 0.125f;
        public static float MarginX0 = Margin;
        public static float MarginX1 = 1 - Margin;
        public static float MarginY0 = Margin;
        public static float MarginY1 = 1 - Margin;
        public static Point Margin0 = new Point(MarginX0, MarginY0);
        public static Point Margin1 = new Point(MarginX1, MarginY1);

        static public Brush PlayerColor;
        static public Brush EnemyColor;
        static public Brush AdditionalColor;
        static public Brush BackgroundColor;
        static public Brush ObstaclesColor;
        static public Color4 BackgroundColorNormal = Colors.Black;
        public static void CreateColors(DeviceManager deviceManager)
        {
            PlayerColor = new SolidColorBrush(deviceManager.ContextDirect2D, new Color4(0.0f, 0.8f, 0.0f, 1.0f));
            EnemyColor = new SolidColorBrush(deviceManager.ContextDirect2D, new Color4(0.8f, 0.0f, 0.0f, 1.0f));
            AdditionalColor = new SolidColorBrush(deviceManager.ContextDirect2D, new Color4(0.8f, 0.8f, 0.0f, 1.0f));
            BackgroundColor = new SolidColorBrush(deviceManager.ContextDirect2D, new Color4(0.0f, 0.0f, 0.0f, 1.0f));
            ObstaclesColor = new SolidColorBrush(deviceManager.ContextDirect2D, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
        }
    }
}
