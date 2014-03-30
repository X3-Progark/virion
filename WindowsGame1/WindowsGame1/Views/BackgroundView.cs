

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Virion
{
    class BackgroundView : GameView
    {

        ContentManager content;
        private Texture2D texture, backgroundTexture;

        private Random random;

        private int[,] colorMatrix;

        private int amountOfTiles,
            elapsedTime, frameTime;

        
        public BackgroundView()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            

            elapsedTime = 0;
            frameTime = 50;

            random = Main.getRandom();

            //How many tiles there should be in x-direction
            amountOfTiles = 100;

            colorMatrix = new int[amountOfTiles, amountOfTiles];
            fillColorMatrix();
        }

        public void fillColorMatrix()
        {
            for (int i = 0; i < amountOfTiles; i++)
            {
                for (int j = 0; j < amountOfTiles; j++)
                {

                    colorMatrix[i, j] = random.Next(0, 500);
                }
            }
        }


        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                texture = new Texture2D(Main.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData(new[] { Color.White });

                if (content == null)
                    content = new ContentManager(ViewManager.Game.Services, "Content");

                backgroundTexture = content.Load<Texture2D>("background");
            }
        }

        public override void Unload()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherViewHasFocus,
                                                       bool coveredByOtherView)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < frameTime)
            {
                //If we are not supposed to calculate a new frame, just return
                return;
            }

            for (int i = 0; i < amountOfTiles; i++)
            {
                for (int j = 0; j < amountOfTiles; j++)
                {
                    colorMatrix[i, j] = colorMatrix[i, j] + 1;
                }
            }

            base.Update(gameTime, otherViewHasFocus, false);
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            Viewport viewport = ViewManager.GraphicsDevice.Viewport;
            Rectangle fullview = new Rectangle(0, 0, viewport.Width, viewport.Height);

            int size = (int)(viewport.Width / amountOfTiles);

            spriteBatch.Begin();

            for (int i = 0; i < amountOfTiles; i++)
            {
                for (int j = 0; j < amountOfTiles; j++)
                {
                    double cosX = Math.Cos(colorMatrix[i, j]/10);
                    spriteBatch.Draw(texture, new Rectangle(size * i, size * j, size, size),
                        new Color(
                            255, 
                            187 + (int)(cosX * 17/4),
                            202 + (int)(cosX * 12/4)));
                }
            }

            //spriteBatch.Draw(backgroundTexture, fullview,
            //                 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }
    }
}
