using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;


namespace Virion
{
    class OptionsMenuView : MenuView
    {

        MenuEntry soundMenuEntry;
        MenuEntry musicMenuEntry;
        MenuEntry playerCountEntry;
        MenuEntry fullScreenEntry;
        MenuEntry resolutionEntry;


        public OptionsMenuView()
            : base("Options")
        {
            resolutionEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);
            musicMenuEntry = new MenuEntry(string.Empty);
            fullScreenEntry = new MenuEntry(string.Empty);
            playerCountEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            resolutionEntry.Selected += ChangeResolution;
            fullScreenEntry.Selected += ToggleFullScreen;
            playerCountEntry.Selected += ChangePlayerCount;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            musicMenuEntry.Selected += MusicMenuEntrySelected;
            back.Selected += BackToMain;

            MenuEntries.Add(resolutionEntry);
            MenuEntries.Add(fullScreenEntry);
            MenuEntries.Add(playerCountEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {

            resolutionEntry.Text = Main.Instance.Conf.Resolution.X + " x " + Main.Instance.Conf.Resolution.Y;
            fullScreenEntry.Text = "Fullscreen: " + Main.Instance.Conf.FullScreen;
            soundMenuEntry.Text = "Sound: " + Main.Instance.Conf.Sound;
            musicMenuEntry.Text = "Music: " + Main.Instance.Conf.Music;
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
            Main.Instance.Conf.Sound = !Main.Instance.Conf.Sound;
            SetMenuEntryText();
        }
        void MusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Main.Instance.Conf.Music = !Main.Instance.Conf.Music;
            if (!Main.Instance.Conf.Music)
                MediaPlayer.Stop();
            SetMenuEntryText();
        }

        void BackToMain(object sender, PlayerIndexEventArgs e)
        {
            Main.Instance.Conf.Save("virionconf.xml");
            OnCancel(0);
        }
    }
}
