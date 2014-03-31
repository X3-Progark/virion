
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Virion
{
    
    /// The loading View coordinates transitions between the menu system and the
    /// game itself. Normally one View will transition off at the same time as
    /// the next view is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing views to transition off.
    /// - Activate a loading view, which will transition on at the same time.
    /// - The loading view watches the state of the previous views.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next view, which may take a long time to load its data. The loading
    ///   view will be the only thing displayed while this load is taking place.
    
    class LoadingView : GameView
    {


        bool loadingIsSlow;
        bool otherViewsAreGone;

        GameView[] viewsToLoad;

        
        /// The constructor is private: loading views should
        /// be activated via the static Load method instead.
        
        private LoadingView(ViewManager viewManager, bool loadingIsSlow,
                              GameView[] viewsToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.viewsToLoad = viewsToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }


        
        /// Activates the loading view.
        public static void Load(ViewManager viewManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameView[] viewsToLoad)
        {
            // Tell all the current views to transition off.
            foreach (GameView view in viewManager.GetViews())
                view.ExitView();

            // Create and activate the loading view.
            LoadingView loadingView = new LoadingView(viewManager,
                                                            loadingIsSlow,
                                                            viewsToLoad);

            viewManager.AddView(loadingView, controllingPlayer);
        }


        
        /// Updates the loading view.
        public override void Update(GameTime gameTime, bool otherViewHasFocus,
                                                       bool coveredByOtherView)
        {
            base.Update(gameTime, otherViewHasFocus, coveredByOtherView);

            // If all the previous views have finished transitioning
            // off, it is time to actually perform the load.
            if (otherViewsAreGone)
            {
                ViewManager.RemoveView(this);

                foreach (GameView view in viewsToLoad)
                {
                    if (view != null)
                    {
                        ViewManager.AddView(view, ControllingPlayer);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ViewManager.Game.ResetElapsedTime();
            }
        }

        
        /// Draws the loading view.
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active view, that means all the previous views
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // views to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ViewState == ViewState.Active) &&
                (ViewManager.GetViews().Length == 1))
            {
                otherViewsAreGone = true;
            }

            // The gameplay view takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (loadingIsSlow)
            {
                SpriteBatch spriteBatch = ViewManager.SpriteBatch;
                SpriteFont font = ViewManager.Font;

                const string message = "Loading...";

                // Center the text in the viewport.
                Viewport viewport = ViewManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }
    }
}
