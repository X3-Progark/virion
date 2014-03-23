using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Random random;

        List<DrawableGameComponent> cellList;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            random = new Random();

            cellList = new List<DrawableGameComponent>();


            cellList.Add(new NormalCell(this, new Point(220, 200), 100));
            cellList.Add(new NormalCell(this, new Point(250, 200), 200));
            cellList.Add(new NormalCell(this, new Point(280, 200), 300));
            cellList.Add(new NormalCell(this, new Point(313, 200), 400));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (NormalCell c in cellList)
            {
                c.LoadContent(graphics.GraphicsDevice);
            }

            texture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            //Texture2D texture = new Texture2D(graphics, 1, 1, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            foreach (NormalCell c in cellList)
            {
                c.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(255,153,153));


            spriteBatch.Begin();

            foreach (NormalCell c in cellList)
            {
                c.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }


        //We want a random
        public Random getRandom()
        {
            return random;
        }

        //We need this as a public method to get different seeds in order to get an actual random number
        public double getRandomD()
        {
            return random.NextDouble();
        }
    }
}
