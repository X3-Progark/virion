using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Virion

{
    public class Protein : Unit
    {
        private int[,] colorMatrix;

        private Color centerColor, centerColorDark;

        public Protein(Vector2 position, int frameTime, Color c1, Color c2)
        {
            cellPosition = position;

            cellRadius = 1;

            cellMotion = new Vector2(0, 0);

            centerColor = c1;
            centerColorDark = c2;

            colorMatrix = new int[cellRadius * 2, cellRadius * 2];

        }

        private void fillColorMatrix()
        {
            for (int i = 0; i < cellRadius * 2; i++)
            {
                for (int j = 0; j < cellRadius * 2; j++)
                {
                    colorMatrix[i, j] = (getRandomD() > 0.5 ? 1 : 2);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < cellRadius * 2; i++)
            {
                for (int j = 0; j < cellRadius * 2; j++)
                {
                    colorMatrix[i, j] = (getRandomD() > 0.5 ? 1 : 2);
                }
            }
        }

        public override void Initialize()
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int i = 0; i < cellRadius * 2 ; i++)
            {
                for (int j = 0; j < cellRadius * 2; j++)
                {
                    spriteBatch.Draw(texture,
                        new Rectangle((int)(cellPosition.X + i * pixelSize), (int)(cellPosition.Y + j * pixelSize), pixelSize, pixelSize), 
                        (colorMatrix[i, j] == 1 ? centerColor : centerColorDark));
                    
                }
            }
        }

        public override void hit (int damage)
        { 

}

    }
}
