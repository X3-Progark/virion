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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NormalCell : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Main game;

        //Default texture
        private Texture2D texture;

        //Time controlling variables
        private int elapsedTime;
        private int frameTime;

        //Colors for the cell
        private int[,] colorMatrix;
        private bool[,] darkMatrix;
        float percentageDarkSpots;
        float darkSpotMotionFactor;

        private Color c;
        private Color wallColor;
        private Color wallColorDark;
        private Color fillColor;
        private Color fillColorDark;
        private Color centerColor;
        private Color centerColorDark;
        
        //Cell look
        private int pixelSize;
        private int cellRadius;
        private double cellRadiusMinFactor;
        private double cellAngleFactor;
        private int cellPoints;

        //Cell pulsation
        private List<double> cellPointsAngle;
        private List<double> cellPointsAngleSpeed;
        private List<double> cellPointsLength;
        private List<double> cellPointsLengthSpeed;
        private List<Vector2> cellVectors;

        //Cell position and motion
        private Point cellPosition;
        private Vector2 cellMotion;


        public NormalCell(Main game, Point cellPosition, int frameTime)
            : base(game)
        {

            pixelSize = 5; //SHOULD BE SOME KIND OF GLOBAL VARIABLE
            cellRadius = 10; //How many pixels the MAXIMUM cell radius should be
            cellRadiusMinFactor = 0.8d; //Percentage of the length of the radius can go inwards, larger makes bigger variation
            cellAngleFactor = 0.6d; //How much the angles can vary. 1 is much, 0 is nothing. Makes shape more random!
            cellPoints = 10; //Number of points that are used to define the edge


            this.game = (Main)game;
            this.frameTime = frameTime;
            this.cellPosition.X = cellPosition.X - cellPosition.X % pixelSize;
            this.cellPosition.Y = cellPosition.Y - cellPosition.Y % pixelSize;
            
            elapsedTime = 0;
            
            

            colorMatrix = new int[cellRadius * 2, cellRadius * 2]; //A cellRadius*2 x cellRadius*2 2D int array
            darkMatrix = new bool[cellRadius * 2, cellRadius * 2]; //A cellRadius*2 x cellRadius*2 2D bool array
            percentageDarkSpots = 0.3f; //How much of the grid that should be dark
            darkSpotMotionFactor = 0.3f; //Should the dark spots move often? higher values will move more. 

            wallColor = new Color(241, 181, 141);
            wallColorDark = new Color(236, 169, 119);
            fillColor = new Color(254, 200, 200);
            fillColorDark = new Color(254, 190, 190);
            centerColor = new Color(254, 220, 220);
            centerColorDark = new Color(254, 215, 215);

            cellPointsAngle = new List<double>(); //All the angles
            cellPointsAngleSpeed = new List<double>(); //All the angle speeds
            cellPointsLength = new List<double>(); //All the lengths
            cellPointsLengthSpeed = new List<double>(); //All the length speeds
            cellVectors = new List<Vector2>(); //Contains the vectors that define the bounds of the cell
            cellMotion = new Vector2(); //Says how the cell is moving

            initDarkMatrix();

            calculateCellPulsation();
            
        }

        public void LoadContent(GraphicsDevice GD)
        {
            //Make the pixel texture that can obtain any color
            texture = new Texture2D(GD, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });

            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private void initDarkMatrix()
        {
            Random r = game.getRandom();
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

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < frameTime)
            {
                //If we are not supposed to calculate a new frame, just return
                base.Update(gameTime);
                return;
            }

            elapsedTime = 0; //We have reached the elapsed time and have to reset it

            colorMatrix = new int[cellRadius * 2, cellRadius * 2];

            calculateNewCellVectors();
            fillColorMatrix();
            updateDarkMatrix();

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
           
            for (int x = 0; x < cellRadius * 2; x++)
            {
                for (int y = 0; y < cellRadius * 2; y++)
                {
                    int p = colorMatrix[x,y];
                    drawPixel(x, y, p, spriteBatch);
                }
            }

            base.Draw(gameTime);
        }

        private void drawPixel(int x, int y, int pixelCode, SpriteBatch spriteBatch)
        {
            if (pixelCode == 0) return;
            else if (pixelCode == 1) c = (darkMatrix[x, y] ? wallColorDark : wallColor);
            else if (pixelCode == 2) c = (darkMatrix[x, y] ? fillColorDark : fillColor);
            else if (pixelCode == 3) c = (darkMatrix[x, y] ? centerColorDark : centerColor);

            int xPos = (x - cellRadius) * pixelSize + cellPosition.X;
            int yPos = (y - cellRadius) * pixelSize + cellPosition.Y;

            spriteBatch.Draw(texture, new Rectangle(xPos, yPos, pixelSize, pixelSize), c);

        }

        //
        private void updateDarkMatrix()
        {
            for (int x = 0; x < cellRadius * 2; x++)
            {
                for (int y = 0; y < cellRadius * 2; y++)
                {
                    if (game.getRandomD() <= darkSpotMotionFactor && darkMatrix[x, y])
                    {
                        double r = game.getRandomD();
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
            double angleStart = game.getRandomD() * 360d; //Find a random startingpoint for our angle
            double angleStep = 360f / cellPoints;
            double angleVariation = angleStep * cellAngleFactor; //How much should the angle vary

            for (int i = 0; i < cellPoints; i++)
            {
                double angle = angleStart + angleStep * i + angleVariation * game.getRandomD();
                double length = cellRadius - cellRadiusMinFactor * cellRadius * game.getRandomD();

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
                if (length >= cellRadius || length <= cellRadius * (1 - cellRadiusMinFactor)) cellPointsLengthSpeed[i] = -cellPointsLengthSpeed[i];

                cellVectors[i] = getVectorFromAngleAndLength(angle, length);

            }
        }

        private void fillColorMatrix()
        {
            fillEdge();
            fillInside(cellRadius, cellRadius);
            fillCenter();
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

        private void fillFloaters()
        {
            int size = cellVectors.Count;

            for (int i = 0; i < size; i++)
            {
                if (i % 2 != 0) continue;

                Vector2 v1 = cellVectors[i];
                int x = (int)(v1.X * 0.5f) + cellRadius;
                int y = (int)(v1.Y * 0.5f) + cellRadius;

                colorMatrix[x, y] = 4;

            }

        }
    }
}
