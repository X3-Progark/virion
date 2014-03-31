
using Microsoft.Xna.Framework;


namespace Virion
{
    class PauseMenuView : MenuView
    {
        public PauseMenuView()
            : base("Paused")
        {
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGame;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        void QuitGame(object sender, PlayerIndexEventArgs e)
        {
            LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
        }
    }
}
