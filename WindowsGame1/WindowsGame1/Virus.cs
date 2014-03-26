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
    public class Virus : Unit
    {
        //Default texture
        private Texture2D texture;

        private int[,] colorMatrix;

        private Color c,
            wasteColor, wallColorDark, 
            fillColor, fillColorDark, 
            centerColor, centerColorDark;
        
        private int pixelSize, 
            elapsedTime, frameTime;

        private Point cellPosition;
        private Vector2 cellMotion;

        Random random;

        public Virus(Point cellPosition, int frameTime)

        {
            //TODO: Må, MÅ, hentes fra en høyere klasse slik at de får forskjellige variabler! 
            //Når de blir initialisert samtidig får de akkurat samme variabler > cellene blir identiske
            random = new Random();

            this.frameTime = frameTime;
            elapsedTime = 0;

            //Sets where the cell is
            this.cellPosition = cellPosition;
            
            //SHOULD BE SOME KIND OF GLOBAL VARIABLE
            pixelSize = 5;

            //A 5x5 2D int array
            colorMatrix = new int[5, 5];
            
            fillColor = new Color(100, 191, 0);
            fillColorDark = new Color(94, 179, 0);
            centerColor = new Color(206, 255, 4);
            centerColorDark = new Color(222, 255, 91);
            wasteColor = new Color(138, 216, 3);
            wallColorDark = new Color(236, 169, 119);
            
            //Says how the cell is moving
            cellMotion = new Vector2();

            setColorMatrix();
        }

        private void setColorMatrix()
        {
            colorMatrix[2, 2] = 3;

            colorMatrix[3, 3] = 1;
            colorMatrix[1, 1] = 1;
            colorMatrix[3, 1] = 1;
            colorMatrix[1, 3] = 1;
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


        public void Update(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < frameTime)
            {
                //If we are not supposed to calculate a new frame, just return
                //base.Update(gameTime);
                return;
            }

            elapsedTime = 0; //We have reached the elapsed time and have to reset it

            //TODO
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    int p = colorMatrix[x, y];
                    drawPixel(x, y, p, spriteBatch);
                }
            }
        }

        private void drawPixel(int x, int y, int pixelCode, SpriteBatch spriteBatch)
        {
            if (pixelCode == 0) return;
            else if (pixelCode == 1) c = fillColor;
            else if (pixelCode == 2) c = fillColorDark;
            else if (pixelCode == 3) c = centerColor;

            int xPos = (x - 2) * pixelSize + cellPosition.X - cellPosition.X % pixelSize;
            int yPos = (y - 2) * pixelSize + cellPosition.Y - cellPosition.Y % pixelSize;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

        }
    }
}
