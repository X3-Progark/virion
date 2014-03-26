
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


        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        public SpriteFont Font
        {
            get { return font; }
        }


        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        
        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }


        public ViewManager(Game game)
            : base(game)
        {

        }


        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        protected override void LoadContent()
        {
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


        protected override void UnloadContent()
        {
            foreach (GameView view in views)
            {
                view.Unload();
            }
        }


        public override void Update(GameTime gameTime)
        {
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

        
        void TraceViews()
        {
            List<string> viewNames = new List<string>();

            foreach (GameView view in views)
                viewNames.Add(view.GetType().Name);

            Debug.WriteLine(string.Join(", ", viewNames.ToArray()));
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (GameView view in views)
            {
                if (view.ViewState == ViewState.Hidden)
                    continue;

                view.Draw(gameTime);
            }
        }

        
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


        public GameView[] GetViews()
        {
            return views.ToArray();
        }


        
        public void FadeBackBufferToBlack(float alpha)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(blankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            spriteBatch.End();
        }
    }
}
