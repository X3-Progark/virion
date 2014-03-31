
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Virion
{
    class MenuEntry
    {
        string text;
        float selectionFade;
        Vector2 position;
        
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        public MenuEntry(string text)
        {
            this.text = text;
        }

        public virtual void Update(MenuView view, bool isSelected, GameTime gameTime)
        {

            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        public virtual void Draw(MenuView view, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Lime : Color.Black;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= view.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ViewManager viewManager = view.ViewManager;
            SpriteBatch spriteBatch = viewManager.SpriteBatch;
            SpriteFont font = viewManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        
        public virtual int GetHeight(MenuView view)
        {
            return view.ViewManager.Font.LineSpacing;
        }


        public virtual int GetWidth(MenuView view)
        {
            return (int)view.ViewManager.Font.MeasureString(Text).X;
        }
    }
}
