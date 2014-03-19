

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Virion
{
    /// <summary>
    /// The background view sits behind all the other menu views.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the views on top of it may be doing.
    /// </summary>
    class BackgroundView : GameView
    {


        ContentManager content;
        Texture2D backgroundTexture;


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundView()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this view. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ViewManager.Game.Services, "Content");

                backgroundTexture = content.Load<Texture2D>("background");
            }
        }


        /// <summary>
        /// Unloads graphics content for this view.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }



        /// <summary>
        /// Updates the background view. Unlike most views, this should not
        /// transition off even if it has been covered by another view: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherView parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherViewHasFocus,
                                                       bool coveredByOtherView)
        {
            base.Update(gameTime, otherViewHasFocus, false);
        }


        /// <summary>
        /// Draws the background view.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            Viewport viewport = ViewManager.GraphicsDevice.Viewport;
            Rectangle fullview = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullview,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }

    }
}
