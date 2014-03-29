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
        public static Main Instance;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        GraphicsDevice device;

        const bool resultionIndependent = true;
        Matrix globalTransformation;
        int resolutionIndex;

        Vector2[] resolutions;

        public static Random random = new Random();
        
        ViewManager viewManager;

        public Player[] players;
        public int playerCount;

        public bool FullScreen
        {
            get { return graphics.IsFullScreen; }
            set
            { 
                graphics.IsFullScreen = value;
                graphics.ApplyChanges();
            }
        }

        public Vector2 Resolution
        {
            get { return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); }
            set
            {
                graphics.PreferredBackBufferHeight = (int)value.Y;
                graphics.PreferredBackBufferWidth = (int)value.X;
                graphics.ApplyChanges();
            }
        }

        public int ResolutionIndex
        {
            get { return resolutionIndex; }
            set
            {
                if (value > resolutions.Length -1)
                    this.resolutionIndex = 0;
                else
                    this.resolutionIndex = value;

                this.Resolution = resolutions[this.resolutionIndex];
            }
        }


        public Main()
        {
            Instance = this;
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

            playerCount = 4;

            players = new Player[playerCount];
            for (int i=0; i < playerCount; i++)
                players[i] = (new Player("Player "+(i+1), i));

            resolutions = new Vector2[3] { new Vector2(1200, 800), new Vector2(1366, 768), new Vector2(1920, 1080) };

            graphics.IsFullScreen = false;

            this.ResolutionIndex = 0;

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
                float horScaling = (float)device.PresentationParameters.BackBufferWidth / Resolution.X;
                float verScaling = (float)device.PresentationParameters.BackBufferHeight / Resolution.Y;
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
