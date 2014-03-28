


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

        List<Keys> playerInputs;
        List<Virus> playerObjects;


        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;


        List<NormalCell> cellList;

        WhiteCell wc;

        
        public GameplayView()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            //Contains the players as well as the number
            playerObjects = new List<Virus>();

            //Add in WASD order! 
            playerInputs = new List<Keys>();

            cellList = new List<NormalCell>();

            //The look of the different players
            string p1 = "OXOXMXOXO";
            string p2 = "OXOOMXXXX";
            string p3 = "XMXOMOXMX";
            string p4 = "OXOOMOXOX";

            //Creates players
            Virus player1 = new Virus(p1, new Point(100, 400), 5);
            Virus player2 = new Virus(p2, new Point(200, 400), 5);
            Virus player3 = new Virus(p3, new Point(300, 400), 5);
            Virus player4 = new Virus(p4, new Point(400, 400), 5);

            //Adds players to the list and gives the correct controls
            addNewPlayer(player1, Keys.Up, Keys.Left, Keys.Down, Keys.Right);
            addNewPlayer(player2, Keys.W, Keys.A, Keys.S, Keys.D);
            addNewPlayer(player3, Keys.T, Keys.F, Keys.G, Keys.H);
            //addNewPlayer(player4, Keys.I, Keys.J, Keys.K, Keys.L);

            for (int i = 0; i < 20; i++)
                cellList.Add(new NormalCell(new Point((int)(800 * Main.getRandomD()), (int)(500 * Main.getRandomD())), 100));

            //cellList.Add(new WhiteCell(new Point(300, 300), 200));

            wc = new WhiteCell(new Point(100, 100), 200);
        }

        public void addNewPlayer(Virus v, Keys up, Keys left, Keys down, Keys right)
        {
            playerObjects.Add(v);

            playerInputs.Add(up);
            playerInputs.Add(left);
            playerInputs.Add(down);
            playerInputs.Add(right);
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

                foreach (Virus v in playerObjects) v.LoadContent(ViewManager.GraphicsDevice);


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
                foreach (NormalCell c in cellList)
                {
                    c.collisionHandeling(cellList);
                    c.Update(gameTime);
                }

                foreach (Virus v in playerObjects) v.Update(gameTime);

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
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {

                ViewManager.AddView(new PauseMenuView(), ControllingPlayer);

            }
            else
            {

                for (int i = 0; i < playerObjects.Count; i++)
                {
                    if (keyboardState.IsKeyDown(playerInputs[4 * i])) 
                        playerObjects[i].up();

                    if (keyboardState.IsKeyDown(playerInputs[4 * i + 1]))
                        playerObjects[i].left();

                    if (keyboardState.IsKeyDown(playerInputs[4 * i + 2]))
                        playerObjects[i].down();

                    if (keyboardState.IsKeyDown(playerInputs[4 * i + 3]))
                        playerObjects[i].right();

                }

                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;


                if (keyboardState.IsKeyDown(playerInputs[0]))
                    movement.Y--;

                if (keyboardState.IsKeyDown(playerInputs[1]))
                    movement.X--;

                if (keyboardState.IsKeyDown(playerInputs[2]))
                    movement.Y++;

                if (keyboardState.IsKeyDown(playerInputs[3]))
                    movement.X++;
                

                

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                

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

            foreach (Virus v in playerObjects) v.Draw(gameTime, spriteBatch);

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
