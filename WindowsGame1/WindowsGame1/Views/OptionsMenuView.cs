using Microsoft.Xna.Framework;


namespace Virion
{
    class OptionsMenuView : MenuView
    {

        MenuEntry soundMenuEntry;
        MenuEntry playerCountEntry;
        MenuEntry fullScreenEntry;
        MenuEntry resolutionEntry;

        static bool sound = true;

        public OptionsMenuView()
            : base("Options")
        {
            resolutionEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);
            fullScreenEntry = new MenuEntry(string.Empty);
            playerCountEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            resolutionEntry.Selected += ChangeResolution;
            fullScreenEntry.Selected += ToggleFullScreen;
            playerCountEntry.Selected += ChangePlayerCount;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(resolutionEntry);
            MenuEntries.Add(fullScreenEntry);
            MenuEntries.Add(playerCountEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {
            resolutionEntry.Text = Main.Instance.Resolution.X + " x " + Main.Instance.Resolution.Y;
            fullScreenEntry.Text = "Fullscreen: " + Main.Instance.FullScreen;
            soundMenuEntry.Text = "Sound: " + sound;
            playerCountEntry.Text = "Players: " + Main.Instance.playerCount;
        }

        void ChangePlayerCount(object sender, PlayerIndexEventArgs e)
        {
            if (Main.Instance.playerCount > 3)
                Main.Instance.playerCount = 1;
            else
                Main.Instance.playerCount++;

            SetMenuEntryText();
        }

        void ToggleFullScreen(object sender, PlayerIndexEventArgs e)
        {
            Main.Instance.FullScreen = !Main.Instance.FullScreen;

            SetMenuEntryText();
        }

        void ChangeResolution(object sender, PlayerIndexEventArgs e)
        {

            Main.Instance.ResolutionIndex++;

            SetMenuEntryText();
        }
        
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;
            SetMenuEntryText();
        }
    }
}
