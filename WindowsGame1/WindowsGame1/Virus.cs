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
        private int[,] colorMatrix;

        private Color c,
            wasteColor, wallColorDark, 
            fillColor, fillColorDark, 
            centerColor, centerColorDark;
        
        private int elapsedTime, frameTime;

        private float breakFactor, motionAdd, maxSpeed;

        private Player player;

        Random random;

        private int health;
        public int Health
        {
            get { return this.health; }
            set { this.health = value; }
        }

        private int strength;
        public int Strength
        {
            get { return this.strength; }
            set { this.strength = value; }
        }

        public Player Player
        {
            get { return this.player; }
            set { this.player = value; }
        }

        public Virus(Player player, Vector2 cellPosition, int frameTime)

        {
            //TODO: Må, MÅ, hentes fra en høyere klasse slik at de får forskjellige variabler! 
            //Når de blir initialisert samtidig får de akkurat samme variabler => cellene blir identiske
            random = new Random();

            this.strength = 1 * player.Strength;
            this.health = 100 * player.Health;
            this.player = player;
            this.frameTime = frameTime;
            elapsedTime = 0;

            //Sets where the cell is
            this.cellPosition = cellPosition;

            //The radius of the cell
            cellRadius = 2; //1; //Maybe one?

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

            //How quick the speed should slow down
            breakFactor = 0.98f - 0.02f * player.Speed;

            //How much it should move
            motionAdd = pixelSize * ( 0.05f + 0.02f * player.Speed );

            maxSpeed = pixelSize * 0.5f * player.Speed;

            setColorMatrix(player.Model);
        }

        private void setColorMatrix(string look)
        {
            for (int i = 0; i < 9; i++)
            {
                int x = (int)Math.Ceiling((i + 1)/3.0);
                int y = (i + 1) + (1 - x) * 3;
                if (look[i].Equals('X'))
                    colorMatrix[y, x] = 1;
                else if (look[i].Equals('M'))
                    colorMatrix[y, x] = 3;
            }
            /*    colorMatrix[2, 2] = 3;

            colorMatrix[3, 3] = 1;
            colorMatrix[1, 1] = 1;
            colorMatrix[3, 1] = 1;
            colorMatrix[1, 3] = 1;*/
        }



        public override void Initialize()
        {
            //base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < frameTime)
            {
                //We are not supposed to calculate a new frame, just return
                return;
            }
            //We have reached the elapsed time and have to reset it
            elapsedTime = 0;

            //Slowing down the virus
            cellMotion.X *= breakFactor;
            cellMotion.Y *= breakFactor;

            if (cellMotion.X > maxSpeed) cellMotion.X = maxSpeed;
            else if (cellMotion.X < -maxSpeed) cellMotion.X = -maxSpeed;
            
            if (cellMotion.Y > maxSpeed) cellMotion.Y = maxSpeed;
            else if (cellMotion.Y < -maxSpeed) cellMotion.Y = -maxSpeed;

            cellPosition.X += (int)cellMotion.X;
            cellPosition.Y += (int)cellMotion.Y;



            //TODO
        }

        public void up()
        {
            cellMotion.Y -= motionAdd;
        }

        public void down()
        {
            cellMotion.Y += motionAdd;
        }

        public void left()
        {
            cellMotion.X -= motionAdd;
        }

        public void right()
        {
            cellMotion.X += motionAdd;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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

            int xPos = (x - 2) * pixelSize + (int)cellPosition.X - (int)cellPosition.X % pixelSize;
            int yPos = (y - 2) * pixelSize + (int)cellPosition.Y - (int)cellPosition.Y % pixelSize;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

        }
        public Vector2 getPosition()
        {
            return this.cellPosition;
        }
    }
}
