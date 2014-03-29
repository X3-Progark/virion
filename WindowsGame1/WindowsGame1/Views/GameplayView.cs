


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
        List<WhiteCell> whiteCellList;


        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;


        List<NormalCell> cellList;

        
        public GameplayView()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Keys[] { Keys.Escape },
                true);

            //Contains the players as well as the number
            playerObjects = new List<Virus>();

            //Add in WASD order! 
            playerInputs = new List<Keys>();

            cellList = new List<NormalCell>();
            whiteCellList = new List<WhiteCell>();

            //The look of the different players

            string[] models = new string[4] { "XOXXOXXOX", "OXOOMXXXX", "XMXOMOXMX", "OXOOMOXOX" };

            Keys[] up = new Keys[4] { Keys.Up, Keys.W, Keys.T, Keys.I };
            Keys[] left = new Keys[4] { Keys.Left, Keys.A, Keys.F, Keys.J };
            Keys[] down = new Keys[4] { Keys.Down, Keys.S, Keys.G, Keys.K };
            Keys[] right = new Keys[4] { Keys.Right, Keys.D, Keys.H, Keys.L }; 

            for (int i = 0; i < Main.Instance.playerCount; i++)
            {
                Main.Instance.players[i].Model = models[i];
                Virus p = new Virus(Main.Instance.players[i], new Vector2(i * 100, i * 100), 5);
                addNewPlayer(p, up[i], left[i], down[i], right[i]);
            }


            for (int i = 0; i < 40; i++)
                cellList.Add(new NormalCell(new Point((int)(800 * Main.getRandomD()), (int)(500 * Main.getRandomD())), 30));

            for (int i = 0; i < 5; i++)
                whiteCellList.Add(new WhiteCell(new Vector2((int)(800 * Main.getRandomD()), (int)(500 * Main.getRandomD())), 30));

            //cellList.Add(new WhiteCell(new Point(300, 300), 200));

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

                foreach (WhiteCell w in whiteCellList) w.LoadContent(ViewManager.GraphicsDevice);

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
                    c.Update(gameTime);
                    c.collisionHandeling(cellList);
                    
                }

                foreach (Virus v in playerObjects) v.Update(gameTime);

                foreach (WhiteCell wc in whiteCellList)
                {
                    wc.updateVirus(playerObjects);
                    wc.Update(gameTime);
                }
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
                

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 1f;
            }
        }


        public void GameWon()
        {
            foreach (Player p in Main.Instance.players)
            {
                p.Proteins++;
            }
            LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
        }


        
        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Pink, 0, 0);

            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (Unit c in cellList) c.Draw(gameTime, spriteBatch);
            foreach (Virus v in playerObjects) v.Draw(gameTime, spriteBatch);

            //wc.Draw(gameTime, spriteBatch, playerPosition);
            foreach (WhiteCell wc in whiteCellList) wc.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ViewManager.FadeBackBufferToBlack(alpha);
            }
        }

    }
}
