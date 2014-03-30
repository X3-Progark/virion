using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Virion
{
    class Map
    {

        public int normalCells;
        public int whiteCells;
        int level;
        public Map()
        {
            level = 1;
        }

        public void generate()
        {
            normalCells = level * 5 + 10;
            whiteCells = level * 1;
        }

        public int Level { get; set; }
    }
}
