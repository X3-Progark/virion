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

namespace Virion
{
    
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        GraphicsDevice device;

        const bool resultionIndependent = true;
        Vector2 baseScreenSize;
        int screenWidth, screenHeight;
        Matrix globalTransformation;

        public static Random random = new Random();
        
        ViewManager viewManager;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            viewManager = new ViewManager(this);
            Components.Add(viewManager);

            viewManager.AddView(new BackgroundView(), null);
            viewManager.AddView(new MainMenuView(), null);
        }

        
        protected override void Initialize()
        {
            base.Initialize();

            baseScreenSize = new Vector2(800, 600);

            if (resultionIndependent)
            {
                screenWidth = (int)baseScreenSize.X;
                screenHeight = (int)baseScreenSize.Y;
            }
            else
            {
                screenWidth = device.PresentationParameters.BackBufferWidth;
                screenHeight = device.PresentationParameters.BackBufferHeight;
            }
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();

        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            

            texture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            //Texture2D texture = new Texture2D(graphics, 1, 1, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            
            base.LoadContent();
        }


        protected override void UnloadContent() {}
        

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            Vector3 screenScalingFactor;
            if (resultionIndependent)
            {
                float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseScreenSize.X;
                float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
                screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            }
            else
            {
                screenScalingFactor = new Vector3(1, 1, 1);
            }
            globalTransformation = Matrix.CreateScale(screenScalingFactor);


            graphics.GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);
            base.Draw(gameTime);
            //spriteBatch.End();
        }

        public static Random getRandom()
        {
            return random;
        }

        public static double getRandomD()
        {
            return random.NextDouble();
        }
    }
}
