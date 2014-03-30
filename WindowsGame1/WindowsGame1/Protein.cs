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

        public Protein(Vector2 position, int frameTime)
        {
            cellPosition = position;

            cellRadius = 2;
            cellMotion = new Vector2(0, 0);
        }


        public override void Update(GameTime gameTime)
        {

        }

        public override void Initialize()
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)cellPosition.X, (int)cellPosition.Y, 20, 10), Color.Brown);
        }

    }
}
