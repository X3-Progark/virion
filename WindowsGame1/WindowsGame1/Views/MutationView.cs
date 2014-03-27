using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Virion
{
    class MutationView : GameView
    {
        ContentManager content;
        SpriteFont gameFont;

        InputAction backAction;
        InputAction buyAction;

        public MutationView()
        {
            backAction = new InputAction(
                new Buttons[] { Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            backAction = new InputAction(
                new Buttons[] { Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }
        
        
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ViewManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");
                ViewManager.Game.ResetElapsedTime();
            }
        }


        public override void Unload()
        {
            content.Unload();
        }


        public override void Update(GameTime gameTime, bool otherViewHasFocus,
                                                       bool coveredByOtherView)
        {
            base.Update(gameTime, otherViewHasFocus, false);


            if (IsActive)
            {
            }
        }


        
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex player;
            if (backAction.Evaluate(input, ControllingPlayer, out player))
            {
                LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
            }
        }


        
        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Blue, 0, 0);

            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Mutations", new Vector2(100, 100), Color.Green);

            spriteBatch.DrawString(gameFont, "Buy things",
                                   new Vector2(300, 300), Color.DarkRed);


            spriteBatch.End();

        }
    }
}
