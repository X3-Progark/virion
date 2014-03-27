using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Virion
{
    public class SoundManager
    {
        public Song bgmusic;

        public SoundManager()
        {
            bgmusic = null;
        }

        public void loadContent(ContentManager Content)
        {
            bgmusic = Content.Load<Song>("theme");
        }
    }
}
