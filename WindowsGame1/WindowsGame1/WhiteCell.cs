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
    public class WhiteCell : Unit
    {

        //Default texture
        private Texture2D texture;

        private int[,] colorMatrix;
        private bool[,] darkMatrix;

        float percentageDarkSpots,
            darkSpotMotionFactor;

        private Color c,
            wallColor, wallColorDark,
            fillColor, fillColorDark,
            centerColor, centerColorDark;

        private int pixelSize,
            cellLength, cellRadius,
            elapsedTime, frameTime;

        private Point cellPosition;
        private Vector2 cellDirection, cellMotion;

        Random random;

        public WhiteCell(Point cellPosition, int frameTime)
        {
            //TODO: Må, MÅ, hentes fra en høyere klasse slik at de får forskjellige variabler! 
            //Når de blir initialisert samtidig får de akkurat samme variabler > cellene blir identiske
            random = new Random();

            elapsedTime = 0;
            this.frameTime = frameTime;

            this.cellPosition = cellPosition;

            this.cellDirection = new Vector2(1, 0);
            this.cellDirection.Normalize();

            pixelSize = 10;

            cellLength = 15;

            //How many pixles the largest part should be
            cellRadius = 4;

            //A cellRadius*2 x cellRadius*2 2D int array
            colorMatrix = new int[cellLength * 2, cellLength * 2];

            //A cellRadius*2 x cellRadius*2 2D bool array
            darkMatrix = new bool[cellLength * 2, cellLength * 2];

            //How much of the grid that should be dark
            percentageDarkSpots = 0.3f;

            //Should the dark spots move often? higher values will move more. 
            darkSpotMotionFactor = 0.3f;

            wallColor = new Color(255, 215, 225);
            wallColorDark = new Color(255, 210, 220);
            fillColor = new Color(255, 232, 238);
            fillColorDark = new Color(255, 225, 233);
            centerColor = new Color(255, 255, 255);
            centerColorDark = new Color(255, 245, 245);

            initDarkMatrix();
            
        }

        public void LoadContent(GraphicsDevice GD)
        {
            //Make the pixel texture that can obtain any color
            texture = new Texture2D(GD, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });

            //base.LoadContent();
        }

        public void Initialize()
        {
            //base.Initialize();
        }

        private void initDarkMatrix()
        {
            int amount = (int)(percentageDarkSpots * (cellLength * cellLength * 4));

            for (int i = 0; i < amount; i++)
            {
                int x = random.Next(0, cellLength * 2);
                int y = random.Next(0, cellLength * 2);

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
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < frameTime)
            {
                //If we are not supposed to calculate a new frame, just return
                //base.Update(gameTime);
                return;
            }

            //We have reached the elapsed time and have to reset it
            elapsedTime = 0;


            colorMatrix = new int[cellLength * 2, cellLength * 2];
            updateDarkMatrix();
            fillColorMatrix();

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

        private void fillColorMatrix()
        {
            fillWall();
            fillInside(cellLength, cellLength);
            fillSingleSpots();
            fillCenter();
        }

        private void fillWall()
        {
            double startX = cellLength - cellDirection.X * cellLength * 0.25d;
            double startY = cellLength - cellDirection.Y * cellLength * 0.25d;
            double stepX = cellDirection.X / (double)pixelSize;
            double stepY = cellDirection.Y / (double)pixelSize;
            Vector2 cV = new Vector2((float)stepX, (float)stepY);
            

            while(cV.Length() <= cellLength)
            {
                Vector2 nV1 = new Vector2(-(float)stepY, (float)stepX);
                nV1.Normalize();
                //Vector2 nV2 = new Vector2((float)stepY, -(float)stepX);
                nV1.X = (float)(nV1.X * cellRadius * getCellThickness(cV.Length()));
                nV1.Y = (float)(nV1.Y * cellRadius * getCellThickness(cV.Length()));

                int x1 = (int)(startX + cV.X + nV1.X);
                int y1 = (int)(startY + cV.Y + nV1.Y);

                int x2 = (int)(startX + cV.X - nV1.X);
                int y2 = (int)(startY + cV.Y - nV1.Y);

                colorMatrix[x1, y1] = 1;
                colorMatrix[x2, y2] = 1;

                cV.X += (float)stepX;
                cV.Y += (float)stepY;
            }
            
        }

        //Returns in percent, KINDA :P
        private double getCellThickness(double xi)
        {
            double x = 4.0d * xi / (double)cellLength;
            return -0.04 * x * x * x * x + 0.42 * x * x * x - 1.6 * x * x + 2.2 * x;
        }

        private void fillInside(int x, int y)
        {
            if (x < 1 || y < 1 || x > cellLength * 2 - 2 || y > cellLength * 2 - 2) return;

            colorMatrix[x, y] = 2;
            if (colorMatrix[x + 1, y] == 0) fillInside(x + 1, y);
            if (colorMatrix[x - 1, y] == 0) fillInside(x - 1, y);
            if (colorMatrix[x, y + 1] == 0) fillInside(x, y + 1);
            if (colorMatrix[x, y - 1] == 0) fillInside(x, y - 1);
        }

        //Fills that one annoying spot that avoids the filling algorithm! Also removes ugly outsiders
        private void fillSingleSpots()
        {
            for (int x = 2; x < cellLength * 2 -2; x++)
            {
                for (int y = 2; y < cellLength * 2 - 2; y++)
                {
                    if (colorMatrix[x, y] == 0 && colorMatrix[x + 1, y] == 1 
                        && colorMatrix[x, y + 1] == 1 && colorMatrix[x - 1, y] == 1 
                        && colorMatrix[x, y - 1] == 1) colorMatrix[x, y] = 1;

                    else if (colorMatrix[x, y] == 1)
                    {
                        int i = 0;
                        if (colorMatrix[x + 1, y] == 0) i++;
                        if (colorMatrix[x + 1, y + 1] == 0) i++;
                        if (colorMatrix[x + 1, y - 1] == 0) i++;
                        if (colorMatrix[x - 1, y] == 0) i++;
                        if (colorMatrix[x - 1, y + 1] == 0) i++;
                        if (colorMatrix[x - 1, y - 1] == 0) i++;
                        if (colorMatrix[x, y + 1] == 0) i++;
                        if (colorMatrix[x, y - 1] == 0) i++;
                        if (i > 5) colorMatrix[x, y] = 0;
                    }                       
                }
            }
        }

        private void fillCenter()
        {
            colorMatrix[cellLength, cellLength] = 3;
            colorMatrix[cellLength - 1, cellLength] = 3;
            colorMatrix[cellLength, cellLength - 1] = 3;
            colorMatrix[cellLength - 1, cellLength - 1] = 3;
        }

        //DELETE THIS, KEEP THE ONE UNDER!
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 pp)
        {
            cellPosition.X = (int)(pp.X);
            cellPosition.Y = (int)(pp.Y);
            cellDirection = new Vector2(cellPosition.X - 250, cellPosition.Y - 250);
            cellDirection.Normalize(); 
            for (int x = 0; x < cellLength * 2; x++)
            {
                for (int y = 0; y < cellLength * 2; y++)
                {
                    int p = colorMatrix[x, y];
                    drawPixel(x, y, p, spriteBatch);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int x = 0; x < cellLength * 2; x++)
            {
                for (int y = 0; y < cellLength * 2; y++)
                {
                    int p = colorMatrix[x, y];
                    drawPixel(x, y, p, spriteBatch);
                }
            }
            //base.Draw(gameTime);
        }

        
        private void drawPixel(int x, int y, int pixelCode, SpriteBatch spriteBatch)
        {
            if (pixelCode == 0) return;
            else if (pixelCode == 1) c = (darkMatrix[x, y] ? wallColorDark : wallColor);
            else if (pixelCode == 2) c = (darkMatrix[x, y] ? fillColorDark : fillColor);
            else if (pixelCode == 3) c = (darkMatrix[x, y] ? centerColorDark : centerColor);

            int xPos = (x - cellLength) * pixelSize + cellPosition.X - cellPosition.X % pixelSize;
            int yPos = (y - cellLength) * pixelSize + cellPosition.Y - cellPosition.Y % pixelSize;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

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
