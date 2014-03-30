﻿using System;
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
    
    public class NormalCell : Unit
    {

        enum State
        {
            Healthy,
            Infected,
            Dead
        };


        //Default texture
        private Texture2D texture;

        private int[,] colorMatrix;
        private bool[,] darkMatrix;

        float percentageDarkSpots,
            darkSpotMotionFactor, 
            maxSpeed, minSpeed, 
            speedFromOtherCell;

        private Color c,
            wallColor, wallColorDark, 
            fillColor, fillColorDark, 
            centerColor, centerColorDark;
        
        private int pixelSize, 
            cellRadius,cellPoints,
            elapsedTime, frameTime;

        private double cellRadiusMinFactor, 
            cellAngleFactor;
        
        private List<double> cellPointsAngle,
            cellPointsAngleSpeed,cellPointsLength,
            cellPointsLengthSpeed;

        private List<Vector2> cellVectors;

        private Vector2 cellPosition;
        private Vector2 cellMotion;

        private State state;

        private int infectionProgress;
        private float health;

        public NormalCell(Vector2 cellPosition, int frameTime)

        {
            //TODO: Må, MÅ, hentes fra en høyere klasse slik at de får forskjellige variabler! 
            //Når de blir initialisert samtidig får de akkurat samme variabler > cellene blir identiske

            this.state = State.Healthy;

            this.infectionProgress = 0;
            this.health = 100.0f;

            this.frameTime = frameTime;
            elapsedTime = 0;

            //Sets where the cell is
            this.cellPosition = cellPosition;
            
            //SHOULD BE SOME KIND OF GLOBAL VARIABLE
            pixelSize = 5;

            //How many pixels the MAXIMUM cell radius should be
            cellRadius = 5;

            //Number of points that are used to define the edge
            cellPoints = 10;

            //Percentage of the length of the radius can go inwards, larger makes bigger variation
            cellRadiusMinFactor = 0.1d;

            //How much the angles can vary. 1 is much, 0 is nothing. Makes shape more random!
            cellAngleFactor = 0.5d;

            //A cellRadius*2 x cellRadius*2 2D int array
            colorMatrix = new int[cellRadius * 2, cellRadius * 2];

            //A cellRadius*2 x cellRadius*2 2D bool array
            darkMatrix = new bool[cellRadius * 2, cellRadius * 2];

            //How much of the grid that should be dark
            percentageDarkSpots = 0.3f;

            //Should the dark spots move often? higher values will move more. 
            darkSpotMotionFactor = 0.3f;

            wallColor = new Color(241, 181, 141);
            wallColorDark = new Color(236, 169, 119);
            fillColor = new Color(254, 200, 200);
            fillColorDark = new Color(254, 190, 190);
            centerColor = new Color(254, 220, 220);
            centerColorDark = new Color(254, 215, 215);

            //All the angles
            cellPointsAngle = new List<double>();
            
            //All the angle speeds
            cellPointsAngleSpeed = new List<double>();
            
            //All the lengths
            cellPointsLength = new List<double>();
            
            //All the length speeds
            cellPointsLengthSpeed = new List<double>();
            
            //Contains the vectors that define the bounds of the cell
            cellVectors = new List<Vector2>();
            
            //Says how the cell is moving
            cellMotion = getVectorFromAngleAndLength((float)360 * getRandomD(), (float)pixelSize/2);

            //The maximum speed a cell can reach
            maxSpeed = pixelSize * 0.9f;

            //The minimum speed a cell can have
            minSpeed = pixelSize * 0.5f;

            //How much speed you get from another cell in a bump
            speedFromOtherCell = 0.5f;

            initDarkMatrix();

            calculateCellPulsation();
            
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

        public bool isInfected()
        {
            return this.state == State.Infected;
        }

        public bool isDead()
        {
            return this.state == State.Dead;
        }

        private void initDarkMatrix()
        {
            Random r = getRandom();
            int amount = (int)(percentageDarkSpots*(cellRadius * cellRadius *4));

            for (int i = 0; i < amount; i++)
            {
                int x = r.Next(0, cellRadius * 2);
                int y = r.Next(0, cellRadius * 2);

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

            if (!isDead() && isInfected())
            {
                health -= 0.1f;
            }

            health -= 1.0f;
            Console.Write(health);

            if (health <= 0.0f)
            {
                this.state = State.Dead;
            }

            if (isInfected())
            {
                maxSpeed = pixelSize * 0.2f;
                minSpeed = pixelSize * 0.1f;
            }
            else if (isDead())
            {
                maxSpeed = 0.0f;
                minSpeed = 0.0f;
            }

            elapsedTime = 0; //We have reached the elapsed time and have to reset it

            colorMatrix = new int[cellRadius * 2, cellRadius * 2];

            if (cellMotion.Length() > maxSpeed) cellMotion *= 0.8f;
            else if (cellMotion.Length() < minSpeed) cellMotion *= 1.2f;

            moveCell();
            calculateNewCellVectors();
            fillColorMatrix();
            updateDarkMatrix();

            //base.Update(gameTime);
        }

        public void Infect(int rate)
        {
            this.infectionProgress += rate;
            if (infectionProgress <= 100)
            {
                this.state = State.Infected;
            }
        }

        private void moveCell()
        {
            cellPosition.X += (int)cellMotion.X;
            cellPosition.Y += (int)cellMotion.Y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!isDead())
            {
                for (int x = 0; x < cellRadius * 2; x++)
                {
                    for (int y = 0; y < cellRadius * 2; y++)
                    {
                        int p = colorMatrix[x, y];
                        drawPixel(x, y, p, spriteBatch);
                    }
                }
                //base.Draw(gameTime);
            }
        }

        private void drawPixel(int x, int y, int pixelCode, SpriteBatch spriteBatch)
        {
            if (pixelCode == 0) return;
            else if (pixelCode == 1) c = (darkMatrix[x, y] ? wallColorDark : wallColor);
            else if (pixelCode == 2) c = (darkMatrix[x, y] ? fillColorDark : fillColor);
            else if (pixelCode == 3) c = (darkMatrix[x, y] ? centerColorDark : centerColor);

            int xPos = (x - cellRadius) * pixelSize + (int)cellPosition.X;// -cellPosition.X % pixelSize;
            int yPos = (y - cellRadius) * pixelSize + (int)cellPosition.Y;// -cellPosition.Y % pixelSize;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

        }

        //Editing the darker pixels
        private void updateDarkMatrix()
        {
            for (int x = 0; x < cellRadius * 2; x++)
            {
                for (int y = 0; y < cellRadius * 2; y++)
                {
                    if (getRandomD() <= darkSpotMotionFactor && darkMatrix[x, y])
                    {
                        double r = getRandomD();
                        int xM = x;
                        int yM = y;

                        if (r <= 0.25d) xM += (x == cellRadius * 2 - 1) ? 0 : 1;
                        else if (r <= 0.5d) xM -= (x == 0) ? 0 : 1;
                        else if (r <= 0.75d) yM += (y == cellRadius * 2 - 1) ? 0 : 1;
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

        //Calculates how the cell vectors look like
        private void calculateCellPulsation()
        {
            double angleStart = getRandomD() * 360d; //Find a random startingpoint for our angle
            double angleStep = 360f / cellPoints;
            double angleVariation = angleStep * cellAngleFactor; //How much should the angle vary

            for (int i = 0; i < cellPoints; i++)
            {
                double angle = angleStart + angleStep * i + angleVariation * getRandomD();
                double length = cellRadius - cellRadiusMinFactor * cellRadius * getRandomD();

                cellPointsAngle.Add(angle);
                cellPointsLength.Add(length);
                cellVectors.Add(getVectorFromAngleAndLength(angle, length));
            }

            for (int i = 0; i < cellPoints; i++)
            {
                /* //Dunno if this is needed, but we get the 
                Vector2 before = cellVectors[(i != 0 ? i - 1 : cellVectors.Count)];
                Vector2 current = cellVectors[i];
                Vector2 after = cellVectors[(i != cellVectors.Count - 1 ? i + 1 : 0)];
                 * WORK NEEDED
                 * WORK NEEDED
                 * WORKNEEDED
                 * 
                 * 
                 * 
                 * 
                 * 
                */

                int max = cellPoints - 1;

                double angleSpeed = 1d;
                cellPointsAngleSpeed.Add(angleSpeed);

                double lengthSpeed = cellRadius + cellRadius * (1 - cellRadiusMinFactor) - 2 * cellPointsLength[i];
                cellPointsLengthSpeed.Add(lengthSpeed*0.1f);
            }
        }

        //Creates a vector with the correct angle and length
        private Vector2 getVectorFromAngleAndLength(double angle, double length)
        {
            double x = Math.Sin((angle) * Math.PI / 180);
            double y = Math.Cos((angle) * Math.PI / 180);

            //Creating the vecor in the correct size
            Vector2 v = new Vector2((float)x, (float)y);
            v.Normalize();
            v = v * (float)length;

            return v;
        }

        private void calculateNewCellVectors()
        {
            for (int i = 0; i < cellVectors.Count; i++ )
            {
                double angle = cellPointsAngle[i];
                angle += cellPointsAngleSpeed[i];
                cellPointsAngle[i] = angle;

                double length = cellPointsLength[i];
                length += cellPointsLengthSpeed[i];
                cellPointsLength[i] = length;

                //SOME UGLY SHIET
                if (length >= cellRadius || length <= cellRadius * (1 - cellRadiusMinFactor))
                    cellPointsLengthSpeed[i] = -cellPointsLengthSpeed[i];

                cellVectors[i] = getVectorFromAngleAndLength(angle, length);

            }
        }

        private void fillColorMatrix()
        {
            fillEdge();
            fillInside(cellRadius, cellRadius);
            fillCenter();
            removeOutsiders();
        }

        private void fillEdge()
        {
            int size = cellVectors.Count;

            for (int i = 0; i < size; i++)
            {
                Vector2 v1 = cellVectors[i];
                Vector2 v2 = cellVectors[(i < size - 1 ? i + 1 : 0)];

                List<Point> points = getPointsBetweenVectors(v1, v2);

                foreach (Point p in points)
                {
                    int x = p.X;
                    int y = p.Y;
                    if (x < 0 || y < 0 || x > cellRadius * 2 - 1 || y > cellRadius * 2 - 1) continue;
                    colorMatrix[x, y] = 1;
                }
            }
        }
      
        private List<Point> getPointsBetweenVectors(Vector2 v1, Vector2 v2)
        {
            List<Point> points = new List<Point>();
            int xS = (int)v1.X;
            int yS = (int)v1.Y;

            //The step-vector
            Vector2 vE = Vector2.Subtract(v2, v1);
            vE.Normalize();

            double xStep = vE.X;
            double yStep = vE.Y;

            //Make a vector that says how long vE should be when ending
            Vector2 vO = Vector2.Subtract(v2, v1);

            points.Add(new Point(xS + cellRadius, yS + cellRadius)); //just in case

            while (vE.Length() <= vO.Length())
            {
                int oldX = (int)vE.X;
                int oldY = (int)vE.Y;

                vE.X += (float)xStep * 0.4f;
                vE.Y += (float)yStep * 0.4f;

                if (oldX != (int)(vE.X) || oldY != (int)(vE.Y))
                {
                    int aX = xS + oldX + cellRadius;
                    int aY = yS + oldY + cellRadius;

                    points.Add(new Point(aX, aY));
                }

                //One final point to close the loop/line
                points.Add(new Point(xS + (int)vO.X + cellRadius, yS + (int)vO.Y + cellRadius));
            }

            return points;
        }

        private void fillInside(int x, int y)
        {
            if (x < 1 || y < 1 || x > cellRadius * 2 - 2 || y > cellRadius * 2 - 2) return;

            colorMatrix[x, y] = 2;
            if (colorMatrix[x + 1, y] == 0) fillInside(x + 1, y);
            if (colorMatrix[x - 1, y] == 0) fillInside(x - 1, y);
            if (colorMatrix[x, y + 1] == 0) fillInside(x, y + 1);
            if (colorMatrix[x, y - 1] == 0) fillInside(x, y - 1);
        }

        private void fillCenter()
        {
            colorMatrix[cellRadius, cellRadius] = 3;
            colorMatrix[cellRadius + 1, cellRadius] = 3;
            colorMatrix[cellRadius, cellRadius + 1] = 3;
            colorMatrix[cellRadius + 1, cellRadius + 1] = 3;
        }

        //Removes ugly outsiders
        private void removeOutsiders()
        {
            for (int x = 1; x < cellRadius * 2 - 1; x++)
            {
                for (int y = 1; y < cellRadius * 2 - 1; y++)
                {
                    if (colorMatrix[x, y] == 1)
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

        public void collisionHandeling(List<NormalCell> cellList)
        {
            foreach(NormalCell c in cellList)
            {
                if (isClose(c) && isColliding(c))
                {
                    coll(c);
                    c.coll(this);
                }
            }

            if (cellPosition.X < cellRadius * pixelSize * 0.5) hitLeft(0f);
            if (cellPosition.Y < cellRadius * pixelSize * 0.5) hitOver(0f);
            if (cellPosition.X > Main.Instance.GraphicsDevice.Viewport.Width - cellRadius * pixelSize * 0.5) hitRight(0f);
            if (cellPosition.Y > Main.Instance.GraphicsDevice.Viewport.Height - cellRadius * pixelSize * 0.5) hitUnder(0f);

        }

        private bool isClose(NormalCell c)
        {
            Vector2 distance = Vector2.Subtract(getPosition(), c.getPosition());
            if (distance.Length() == 0) return false;
            return distance.Length() < (2 * cellRadius * (pixelSize - 1));
        }

        private bool isColliding(NormalCell c)
        {
            Vector2 thisCellPosition = getPosition();
            Vector2 otherCellPosition = c.getPosition();
            Vector2 currentDistance = Vector2.Subtract(thisCellPosition, otherCellPosition);

            Vector2 thisNextPosition = Vector2.Add(thisCellPosition, getMotion());
            Vector2 otherNextPosition = Vector2.Add(otherCellPosition, c.getMotion());
            Vector2 nextDistance = Vector2.Subtract(thisNextPosition, otherNextPosition);

            if (currentDistance.Length() >= nextDistance.Length()) return true;
            else return false;
        }

        public void coll(NormalCell c)
        {
            Vector2 between = Vector2.Subtract(getPosition(), c.getPosition());

            //Got hit in the y direction?
            if (Math.Abs(between.Y) > Math.Abs(between.X))
            {
                //Got hit under?
                if (between.Y < 0) hitUnder(c.getMotion().Y);
                else hitOver(c.getMotion().Y);
            }
            else
            {
                //Got hit right?
                if (between.X < 0) hitRight(c.getMotion().X);
                else hitLeft(c.getMotion().X);
            }
        }

        private void hitUnder(float otherCellMotionY)
        {
            cellMotion.Y = -Math.Abs(cellMotion.Y) + otherCellMotionY * speedFromOtherCell;
            moveCell();
        }

        private void hitOver(float otherCellMotionY)
        {
            cellMotion.Y = Math.Abs(cellMotion.Y) + otherCellMotionY * speedFromOtherCell;
            moveCell();
        }

        private void hitRight(float otherCellMotionX)
        {
            cellMotion.X = -Math.Abs(cellMotion.X) + otherCellMotionX * speedFromOtherCell;
            moveCell();
        }

        private void hitLeft(float otherCellMotionX)
        {
            cellMotion.X = Math.Abs(cellMotion.X) + otherCellMotionX * speedFromOtherCell;
            moveCell();
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
        private Random getRandom()
        {
            return Main.getRandom();
        }

        //We need this as a public method to get different seeds in order to get an actual random number
        private double getRandomD()
        {
            return Main.getRandomD();
        }
    }
}
