using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
namespace MetroRetro
{
    static public class GamesParams
    {
        public static float Margin = 0.05f;
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
            PlayerColor = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.Green);
            EnemyColor = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.Red);
            AdditionalColor = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.Yellow);
            BackgroundColor = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.Black);
            ObstaclesColor = new SolidColorBrush(deviceManager.ContextDirect2D, Colors.White);
        }
    }
}
