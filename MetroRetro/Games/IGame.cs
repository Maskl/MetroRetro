namespace MetroRetro.Games
{
    interface IGame
    {
        void KeyPressed(InputType key);
        void KeyReleased(InputType key);

        void Update();
        void Render();
    }
}
