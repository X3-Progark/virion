
using Microsoft.Xna.Framework;

namespace Virion

{
    class MainMenuView : MenuView
    {
        
        public MainMenuView()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        
        /// Event handler for when the Play Game menu entry is selected.
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingView.Load(ViewManager, true, e.PlayerIndex,
                               new GameplayView());
        }

        
        /// Event handler for when the Options menu entry is selected.
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ViewManager.AddView(new OptionsMenuView(), e.PlayerIndex);
        }


        /// When the user cancels the main menu, ask if they want to exit the sample.
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ViewManager.Game.Exit();
        }
    }
}
