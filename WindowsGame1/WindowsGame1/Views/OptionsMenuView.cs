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
            back.Selected += BackToMain;

            MenuEntries.Add(resolutionEntry);
            MenuEntries.Add(fullScreenEntry);
            MenuEntries.Add(playerCountEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {

            resolutionEntry.Text = Main.Instance.Conf.Resolution.X + " x " + Main.Instance.Conf.Resolution.Y;
            fullScreenEntry.Text = "Fullscreen: " + Main.Instance.Conf.FullScreen;
            soundMenuEntry.Text = "Sound: " + sound;
            playerCountEntry.Text = "Players: " + Main.Instance.Conf.playerCount;
        }

        void ChangePlayerCount(object sender, PlayerIndexEventArgs e)
        {
            if (Main.Instance.Conf.playerCount > 3)
                Main.Instance.Conf.playerCount = 1;
            else
                Main.Instance.Conf.playerCount++;

            SetMenuEntryText();
        }

        void ToggleFullScreen(object sender, PlayerIndexEventArgs e)
        {
            Main.Instance.Conf.FullScreen = !Main.Instance.Conf.FullScreen;

            SetMenuEntryText();
        }

        void ChangeResolution(object sender, PlayerIndexEventArgs e)
        {

            Main.Instance.Conf.ResolutionIndex++;

            SetMenuEntryText();
        }
        
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;
            SetMenuEntryText();
        }

        void BackToMain(object sender, PlayerIndexEventArgs e)
        {
            Main.Instance.Conf.Save("virionconf.xml");
            OnCancel(0);
        }
    }
}
