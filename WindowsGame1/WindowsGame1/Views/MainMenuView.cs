
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Virion

{
    class MainMenuView : MenuView
    {

        Song song;
        

        public MainMenuView()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry mutationsMenuEntry = new MenuEntry("Mutations");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            mutationsMenuEntry.Selected += MutationsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(mutationsMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            ContentManager content = Main.Instance.Content;
            song = content.Load<Song>("menu_music");
            MediaPlayer.Stop();
            if (Main.Instance.Conf.Music)
                MediaPlayer.Play(song);

            
        }

        
        /// Event handler for when the Play Game menu entry is selected.
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();
            ViewManager.AddView(new GameplayView(), e.PlayerIndex);
            //LoadingView.Load(ViewManager, true, e.PlayerIndex,
            //                   new BackgroundView(), new GameplayView());
        }

        /// Event handler for when the Mutations menu entry is selected.
        void MutationsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ViewManager.AddView(new MutationView(), e.PlayerIndex);
            //LoadingView.Load(ViewManager, true, e.PlayerIndex,
            //                   new BackgroundView(), new MutationView());
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

        /// When the user cancels the main menu, ask if they want to exit the sample.
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            SpriteFont font = ViewManager.Font;
            SpriteFont creditFont = ViewManager.CreditFont;

            spriteBatch.Begin();

            string t = "Level: " + Main.Instance.level;
            string credit = "Thanks to Eric Skiff for the music, ericskiff.com";
            string credit2 = "A game by Tobias Linkjendal and Jorgen Foss Eri";

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 levelPosition = new Vector2(graphics.Viewport.Width / 2, 125);
            Vector2 creditPosition = new Vector2(graphics.Viewport.Width / 2, 500);
            Vector2 credit2Position = new Vector2(graphics.Viewport.Width / 2, 475);
            levelPosition.Y -= transitionOffset * 100;
            creditPosition.Y -= transitionOffset * 100;
            credit2Position.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, t,
                levelPosition, 
                Color.Black, 0, (font.MeasureString(t) / 2), 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(creditFont, credit,
                creditPosition,
                Color.Black, 0, (creditFont.MeasureString(credit) / 2), 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(creditFont, credit2,
                credit2Position,
                Color.Black, 0, (creditFont.MeasureString(credit2) / 2), 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
