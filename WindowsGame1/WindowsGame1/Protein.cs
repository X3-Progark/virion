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

        private Texture2D proteinTexture;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        public Protein(Vector2 position, int frameTime)
        {
            this.position = position;

            proteinTexture = new Texture2D(Main.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            proteinTexture.SetData(new[] { Color.White });
        }


        public void Update(GameTime gameTime)
        {
     /*      
            Virus v = hitsAVirus();
            if (v != null)
            {
                v.Consume(this);
            }
      */
        }

        public void Initialize()
        {

        }

        public void LoadContent(GraphicsDevice gd)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(proteinTexture, new Rectangle((int)position.X, (int)position.Y, 20, 10), Color.Brown);
        }

    }
}
