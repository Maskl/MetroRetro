using CommonDX;
using SharpDX;
using SharpDX.Direct2D1;
namespace MetroRetro
{
    static public class GameColors
    {
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
