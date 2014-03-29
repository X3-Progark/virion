using Microsoft.Xna.Framework;


namespace Virion
{
    class OptionsMenuView : MenuView
    {

        MenuEntry soundMenuEntry;
        MenuEntry playerCountEntry;

        static bool sound = true;

        public OptionsMenuView()
            : base("Options")
        {
            soundMenuEntry = new MenuEntry(string.Empty);
            playerCountEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            playerCountEntry.Selected += ChangePlayerCount;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(playerCountEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {
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
        
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;
            SetMenuEntryText();
        }
    }
}
