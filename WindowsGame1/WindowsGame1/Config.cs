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
using System.IO;
using System.Xml.Serialization;

namespace Virion
{
    public class Config
    {

        public int playerCount;
        Vector2 resolution;
        Vector2[] resolutions;
        int resolutionIndex;

        public bool FullScreen
        {
            get { return Main.Instance.graphics.IsFullScreen; }
            set
            {
                Main.Instance.graphics.IsFullScreen = value;
                Main.Instance.graphics.ApplyChanges();
            }
        }

        public Vector2 Resolution
        {
            get { return resolution; }
            set
            {
                resolution = value;
                Main.Instance.graphics.PreferredBackBufferHeight = (int)value.Y;
                Main.Instance.graphics.PreferredBackBufferWidth = (int)value.X;
                Main.Instance.graphics.ApplyChanges();
            }
        }

        public int ResolutionIndex
        {
            get { return resolutionIndex; }
            set
            {
                if (value > resolutions.Length - 1)
                    this.resolutionIndex = 0;
                else
                    this.resolutionIndex = value;

                this.Resolution = resolutions[this.resolutionIndex];
            }
        }


        public Config()
        {
            this.resolutions = new Vector2[3] { new Vector2(1200, 800), new Vector2(1366, 768), new Vector2(1920, 1080) };
            this.ResolutionIndex = 0;
            this.playerCount = 4;
        }


        public void LoadContent()
        {
            ContentManager content = Main.Instance.Content;
            Console.Write("Loading content");

            try
            {
                Config c = Load("virionconf.xml");

                this.ResolutionIndex = c.resolutionIndex;
                this.Resolution = c.Resolution;
                this.playerCount = c.playerCount;
            }
            catch (FileNotFoundException e)
            {
                
            }
        }


        public void Save(string filename)
        {
            Stream stream = File.Create(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        public static Config Load(string filename)
        {
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            Config c = (Config)serializer.Deserialize(stream);
            stream.Close();
            return c;
        }
    }
}
