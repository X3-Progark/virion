using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Virion
{
    public class Cell : Unit
    {
        //Default texture
        protected Texture2D texture;

        protected int[,] colorMatrix;
        protected bool[,] darkMatrix;

        protected float percentageDarkSpots,
            darkSpotMotionFactor;

        protected Color c,
            wallColor, wallColorDark,
            fillColor, fillColorDark,
            centerColor, centerColorDark;

        protected int pixelSize,
            cellLength, cellRadius,
            elapsedTime, frameTime;

        protected Vector2 cellPosition;
        protected Vector2 cellDirection, cellMotion;

        public Cell(Vector2 cellPosition, int frameTime, int cellRadius)
        {
            this.frameTime = frameTime;
            elapsedTime = 0;

            //Sets where the cell is
            this.cellPosition = cellPosition;

            //SHOULD BE SOME KIND OF GLOBAL VARIABLE
            pixelSize = 5;

            //How many pixels the MAXIMUM cell radius should be
            this.cellRadius = cellRadius;

            //How much of the grid that should be dark
            percentageDarkSpots = 0.3f;

            //Should the dark spots move often? higher values will move more. 
            darkSpotMotionFactor = 0.3f;
        }

        public void LoadContent(GraphicsDevice GD)
        {
            texture = new Texture2D(GD, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
        }

        public void Initialize()
        {
            //base.Initialize();
        }

        protected void initDarkMatrix()
        {
            Random r = getRandom();
            int temp;
            if (cellLength == 0)
                temp = cellLength;
            else
                temp = cellRadius;
            
            int amount = (int)(percentageDarkSpots * (temp * temp * 4));

            for (int i = 0; i < amount; i++)
            {
                int x = r.Next(0, temp * 2);
                int y = r.Next(0, temp * 2);

                if (darkMatrix[x, y])
                {
                    i -= 1;
                    continue;
                }

                darkMatrix[x, y] = true;
            }
        }


        public void Update(GameTime gameTime)
        {
        }

        //Editing the darker pixels
        private void updateDarkMatrix()
        {
            for (int x = 0; x < cellLength * 2; x++)
            {
                for (int y = 0; y < cellLength * 2; y++)
                {
                    if (getRandomD() <= darkSpotMotionFactor && darkMatrix[x, y])
                    {
                        double r = getRandomD();
                        int xM = x;
                        int yM = y;

                        if (r <= 0.25d) xM += (x == cellLength * 2 - 1) ? 0 : 1;
                        else if (r <= 0.5d) xM -= (x == 0) ? 0 : 1;
                        else if (r <= 0.75d) yM += (y == cellLength * 2 - 1) ? 0 : 1;
                        else yM -= (y == 0) ? 0 : 1;

                        if (!darkMatrix[xM, yM])
                        {
                            darkMatrix[x, y] = false;
                            darkMatrix[xM, yM] = true;
                        }
                    }
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int temp;
            if (cellLength == 0)
                temp = cellLength;
            else
                temp = cellRadius;
                
            for (int x = 0; x < temp * 2; x++)
            {
                for (int y = 0; y < temp * 2; y++)
                {
                    int p = colorMatrix[x, y];
                    drawPixel(x, y, p, temp, spriteBatch);
                }
            }
            //base.Draw(gameTime);
        }
        private void fillCenter()
        {
            colorMatrix[cellRadius, cellRadius] = 3;
            colorMatrix[cellRadius + 1, cellRadius] = 3;
            colorMatrix[cellRadius, cellRadius + 1] = 3;
            colorMatrix[cellRadius + 1, cellRadius + 1] = 3;
        }
        protected void drawPixel(int x, int y, int pixelCode, int length, SpriteBatch spriteBatch)
        {
            if (pixelCode == 0) return;
            else if (pixelCode == 1) c = (darkMatrix[x, y] ? wallColorDark : wallColor);
            else if (pixelCode == 2) c = (darkMatrix[x, y] ? fillColorDark : fillColor);
            else if (pixelCode == 3) c = (darkMatrix[x, y] ? centerColorDark : centerColor);

            int xPos = (x - length) * pixelSize + (int)cellPosition.X - (int)cellPosition.X % pixelSize;
            int yPos = (y - length) * pixelSize + (int)cellPosition.Y - (int)cellPosition.Y % pixelSize;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

        }
        public Vector2 getPosition()
        {
            return cellPosition;
        }

        public Vector2 getMotion()
        {
            return cellMotion;
        }
        //We want a random
        public Random getRandom()
        {
            return Main.random;
        }

        //We need this as a public method to get different seeds in order to get an actual random number
        public double getRandomD()
        {
            return Main.random.NextDouble();
        }
         
    }
}
