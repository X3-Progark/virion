﻿using System;
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
    public interface Unit
    {

        void LoadContent(GraphicsDevice GD);

        void Initialize();

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime, SpriteBatch sb);
         
    }
}
