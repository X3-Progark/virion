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

        public List<WhiteCell> whiteCellList;
        public List<NormalCell> cellList;
        public int level;
        public Map()
        {
            level = 1;
            cellList = new List<NormalCell>();
            whiteCellList = new List<WhiteCell>();

        }

        public void generate()
        {
            for (int i = 0; i < level * 10; i++)
                cellList.Add(new NormalCell(new Point((int)(800 * Main.getRandomD()), (int)(500 * Main.getRandomD())), 30));

            for (int i = 0; i < level * 2; i++)
                whiteCellList.Add(new WhiteCell(new Vector2((int)(800 * Main.getRandomD()), (int)(500 * Main.getRandomD())), 30));
        }

        public int Level { get; set; }

    }
}
