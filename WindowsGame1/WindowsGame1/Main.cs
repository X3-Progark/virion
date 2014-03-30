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
using System.IO;
using System.Xml.Serialization;

namespace Virion
{
    
    public class Main : Microsoft.Xna.Framework.Game
    {
        public static Main Instance;

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        GraphicsDevice device;

        public static Random random = new Random();
        
        ViewManager viewManager;

        public Config Conf;

        public Player[] players;
        public int level;
        

        public Main()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            level = 1;

            Conf = new Config();
            viewManager = new ViewManager(this);

            Components.Add(viewManager);

            viewManager.AddView(new BackgroundView(), null);
            viewManager.AddView(new MainMenuView(), null);
        }

        
        protected override void Initialize()
        {
            base.Initialize();

            if (players == null)
            {
                players = new Player[Conf.playerCount];
                for (int i = 0; i < Conf.playerCount; i++)
                {
                    players[i] = (new Player());
                    players[i].Initialize("Player " + (i + 1), i);
                }
            }

        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            Conf.LoadContent();

            texture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            //Texture2D texture = new Texture2D(graphics, 1, 1, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });

            try
            {
                Save s = Load();
                this.level = s.level;
                this.players = new Player[4];

                for (int i = 0; i < 4; i++)
                {
                    if (s.players.Length > i)
                    {
                        this.players[i] = s.players[i];
                    }
                    else
                    {
                        Player p = new Player();
                        p.Initialize("Player " + (i + 1), i);
                        this.players[i] = p;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.Write(e.ToString());
            }
            
            base.LoadContent();
        }


        protected override void UnloadContent() {}
        

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, globalTransformation);
            base.Draw(gameTime);
            //spriteBatch.End();
        }

        public static Random getRandom()
        {
            return random;
        }

        public static double getRandomD()
        {
            return random.NextDouble();
        }

        public void Save()
        {
            Stream stream = File.Create("virionSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(Save));

            Save s = new Save();
            s.players = this.players;
            s.level = this.level;

            serializer.Serialize(stream, s);
            stream.Close();
        }

        public Save Load()
        {
            Stream stream = File.OpenRead("virionSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(Save));
            Save c = (Save)serializer.Deserialize(stream);
            stream.Close();
            return c;
        }
    }
}
