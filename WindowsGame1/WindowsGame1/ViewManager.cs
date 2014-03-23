
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;


namespace Virion
{
    /// <summary>
    /// The view manager is a component which manages one or more GameView
    /// instances. It maintains a stack of views, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active view.
    /// </summary>
    public class ViewManager : DrawableGameComponent
    {

        List<GameView> views = new List<GameView>();
        List<GameView> tempViewsList = new List<GameView>();

        InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;

        bool isInitialized;

        bool traceEnabled;


        /// <summary>
        /// A default SpriteBatch shared by all the views. This saves
        /// each view having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        /// <summary>
        /// A default font shared by all the views. This saves
        /// each view having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }


        /// <summary>
        /// If true, the manager prints out a list of all the views
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }


        /// <summary>
        /// Gets a blank texture that can be used by the views.
        /// </summary>
        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }


        /// <summary>
        /// Constructs a new view manager component.
        /// </summary>
        public ViewManager(Game game)
            : base(game)
        {

        }


        /// <summary>
        /// Initializes the view manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the view manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the views to load their content.
            foreach (GameView view in views)
            {
                view.Activate(false);
            }
        }


        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the views to unload their content.
            foreach (GameView view in views)
            {
                view.Unload();
            }
        }



        /// <summary>
        /// Allows each view to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master view list, to avoid confusion if
            // the process of updating one view adds or removes others.
            tempViewsList.Clear();

            foreach (GameView view in views)
                tempViewsList.Add(view);

            bool otherViewHasFocus = !Game.IsActive;
            bool coveredByOtherView = false;

            // Loop as long as there are views waiting to be updated.
            while (tempViewsList.Count > 0)
            {
                // Pop the topmost view off the waiting list.
                GameView view = tempViewsList[tempViewsList.Count - 1];

                tempViewsList.RemoveAt(tempViewsList.Count - 1);

                // Update the view.
                view.Update(gameTime, otherViewHasFocus, coveredByOtherView);

                if (view.ViewState == ViewState.TransitionOn ||
                    view.ViewState == ViewState.Active)
                {
                    // If this is the first active view we came across,
                    // give it a chance to handle input.
                    if (!otherViewHasFocus)
                    {
                        view.HandleInput(gameTime, input);

                        otherViewHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // views that they are covered by it.
                    if (!view.IsPopup)
                        coveredByOtherView = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceViews();
        }


        /// <summary>
        /// Prints a list of all the views, for debugging.
        /// </summary>
        void TraceViews()
        {
            List<string> viewNames = new List<string>();

            foreach (GameView view in views)
                viewNames.Add(view.GetType().Name);

            Debug.WriteLine(string.Join(", ", viewNames.ToArray()));
        }


        /// <summary>
        /// Tells each view to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameView view in views)
            {
                if (view.ViewState == ViewState.Hidden)
                    continue;

                view.Draw(gameTime);
            }
        }




        /// <summary>
        /// Adds a new view to the view manager.
        /// </summary>
        public void AddView(GameView view, PlayerIndex? controllingPlayer)
        {
            view.ControllingPlayer = controllingPlayer;
            view.ViewManager = this;
            view.IsExiting = false;
            if (isInitialized)
            {
                view.Activate(false);
            }
            views.Add(view);

        }


        /// <summary>
        /// Removes a view from the view manager. You should normally
        /// use GameView.ExitView instead of calling this directly, so
        /// the view can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveView(GameView view)
        {
            // If we have a graphics device, tell the view to unload content.
            if (isInitialized)
            {
                view.Unload();
            }

            views.Remove(view);
            tempViewsList.Remove(view);

        }


        /// <summary>
        /// Expose an array holding all the views. We return a copy rather
        /// than the real master list, because views should only ever be added
        /// or removed using the AddView and RemoveView methods.
        /// </summary>
        public GameView[] GetViews()
        {
            return views.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullview sprite, used for fading
        /// views in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(blankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            spriteBatch.End();
        }

    }
}
