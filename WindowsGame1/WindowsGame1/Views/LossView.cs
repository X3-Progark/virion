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
                if (Main.Instance.level > 1)
                    Main.Instance.level--;

                Main.Instance.Save();
                LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            SpriteFont font = ViewManager.Font;

            spriteBatch.Begin();

            string t = "You lost!";
            Vector2 levelPosition = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height / 2);

            spriteBatch.DrawString(font, t,
                levelPosition,
                Color.Red, 0, (font.MeasureString(t) / 2), 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

    }
}
