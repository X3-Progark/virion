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
    public abstract class Unit
    {
        protected Texture2D texture;

        protected Vector2 cellPosition, cellMotion;

        protected static int pixelSize = (int)(Main.Instance.Conf.Resolution.X * 0.004f); 
        protected int cellRadius;

        public virtual void LoadContent(Texture2D t)
        {
            this.texture = t;
        }

        public abstract void Initialize();

        public abstract void Update(GameTime gameTime);
        
        public abstract void Draw(GameTime gameTime, SpriteBatch sb);

        public virtual int getRadius()
        {
            return cellRadius;
        }

        public virtual Vector2 getPosition()
        {
            return cellPosition;
        }

        public virtual Vector2 getMotion()
        {
            return cellMotion;
        }

        public virtual bool isClose(Unit u)
        {
            Vector2 distance = Vector2.Subtract(getPosition(), u.getPosition());

            if (distance.Length() == 0) 
                return false;
            else 
                return distance.Length() < ((getRadius() + u.getRadius()) * (pixelSize - 1));
        }
        /*
        public virtual bool isColliding(Unit u)
        {
            return true;
        }*/

        public virtual Random getRandom()
        {
            return Main.getRandom();
        }

        public virtual double getRandomD()
        {
            return Main.getRandomD();
        }

    }
}
