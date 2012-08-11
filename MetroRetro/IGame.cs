using CommonDX;

namespace MetroRetro.Games
{
    interface IGame
    {
        void NewGame();
        void EndGame();
        void Update(long dt, TargetBase target, DeviceManager deviceManager);
        void KeyPressed(InputType key);
        void KeyReleased(InputType key);
    }
}
