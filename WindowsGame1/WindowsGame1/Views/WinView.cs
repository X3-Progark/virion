﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Virion
{
    class WinView : GameView
    {

        InputAction exitView;


        public WinView()
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

            string t = "You won!";
            Vector2 levelPosition = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height / 2);

            spriteBatch.DrawString(font, t,
                levelPosition,
                Color.Green, 0, (font.MeasureString(t) / 2), 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

    }
}
