


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Virion
{
    class GameplayView : GameView
    {

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;


        List<Unit> cellList;

        WhiteCell wc;

        
        public GameplayView()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Keys[] { Keys.Escape },
                true);

            cellList = new List<Unit>();


            cellList.Add(new NormalCell(new Point(100, 200), 100));
            cellList.Add(new NormalCell(new Point(250, 200), 200));
            cellList.Add(new NormalCell(new Point(400, 200), 200));
            cellList.Add(new NormalCell(new Point(550, 200), 200));
            cellList.Add(new WhiteCell(new Point(300, 300), 200));

            cellList.Add(new Virus(new Point(300, 400), 200));

            wc = new WhiteCell(new Point(100, 100), 200);
        }

        
        
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ViewManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                foreach (Unit c in cellList)
                {
                    c.LoadContent(ViewManager.GraphicsDevice);
                }

                wc.LoadContent(ViewManager.GraphicsDevice);
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



            // Gradually fade in or out depending on whether we are covered by the pause view.
            if (coveredByOtherView)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);



            if (IsActive)
            {
                foreach (Unit c in cellList)
                {
                    c.Update(gameTime);
                }

                wc.Update(gameTime);
            }
        }


        
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
                ViewManager.AddView(new PauseMenuView(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 1f;
            }
        }


        
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ViewManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Pink, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();

            //spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);

            foreach (Unit c in cellList)
            {
                c.Draw(gameTime, spriteBatch);
            }

            wc.Draw(gameTime, spriteBatch, playerPosition);
            //wc.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ViewManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
