using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Virion
{
    class LossView : GameView
    {

        InputAction exitView;


        public LossView()
        {

            exitView = new InputAction(
                new Keys[] { Keys.Escape, Keys.Enter },
                true);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (exitView.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            SpriteFont font = ViewManager.Font;

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "You LOST", new Vector2(graphics.Viewport.Width/2, graphics.Viewport.Height/2), Color.Red);

            spriteBatch.End();
        }

    }
}
