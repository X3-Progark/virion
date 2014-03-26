using Microsoft.Xna.Framework;


namespace Virion
{
    class OptionsMenuView : MenuView
    {

        MenuEntry soundMenuEntry;

        static bool sound = true;

        public OptionsMenuView()
            : base("Options")
        {
            soundMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            soundMenuEntry.Selected += SoundMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {
            soundMenuEntry.Text = "Sound on/off: " + soundMenuEntry;
        }
        
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;
            SetMenuEntryText();
        }
    }
}
