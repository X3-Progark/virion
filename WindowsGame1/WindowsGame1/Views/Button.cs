#region File Description
//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virion
{
    
    /// A special button that handles toggling between "On" and "Off"
    
    class BooleanButton : Button
    {
        private string option;
        private bool value;

        
        /// Creates a new BooleanButton.
        public BooleanButton(string option, bool value)
            : base(option)
        {
            this.option = option;
            this.value = value;

            GenerateText();
        }

        protected override void OnTapped()
        {
            // When tapped we need to toggle the value and regenerate the text
            value = !value;
            GenerateText();

            base.OnTapped();
        }

        
        /// Helper that generates the actual Text value the base class uses for drawing.
        
        private void GenerateText()
        {
            Text = string.Format("{0}: {1}", option, value ? "On" : "Off");
        }
    }

    
    /// Represents a touchable button.
    
    class Button
    {
        
        public string Text = "Button";
        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = new Vector2(250, 75);
        public int BorderThickness = 4;
        public Color BorderColor = new Color(200, 200, 200);
        public Color FillColor = new Color(100, 100, 100) * .75f;
        public Color TextColor = Color.White;
        public float Alpha = 0f;
        public event EventHandler<EventArgs> Tapped;
        
        public Button(string text)
        {
            Text = text;
        }

        
        protected virtual void OnTapped()
        {
            if (Tapped != null)
                Tapped(this, EventArgs.Empty);
        }

        
        /// Passes a tap location to the button for handling.
        
        /// <param name="tap">The location of the tap.</param>
        /// <returns>True if the button was tapped, false otherwise.</returns>
        public bool HandleTap(Vector2 tap)
        {
            if (tap.X >= Position.X &&
                tap.Y >= Position.Y &&
                tap.X <= Position.X + Size.X &&
                tap.Y <= Position.Y + Size.Y)
            {
                OnTapped();
                return true;
            }

            return false;
        }

        
        /// Draws the button
        public void Draw(GameView view)
        {
            // Grab some common items from the ViewManager
            SpriteBatch spriteBatch = view.ViewManager.SpriteBatch;
            SpriteFont font = view.ViewManager.Font;
            Texture2D blank = view.ViewManager.BlankTexture;

            // Compute the button's rectangle
            Rectangle r = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)Size.X,
                (int)Size.Y);

            // Fill the button
            spriteBatch.Draw(blank, r, FillColor * Alpha);

            // Draw the border
            spriteBatch.Draw(
                blank, 
                new Rectangle(r.Left, r.Top, r.Width, BorderThickness),
                BorderColor * Alpha);
            spriteBatch.Draw(
                blank, 
                new Rectangle(r.Left, r.Top, BorderThickness, r.Height),
                BorderColor * Alpha);
            spriteBatch.Draw(
                blank, 
                new Rectangle(r.Right - BorderThickness, r.Top, BorderThickness, r.Height),
                BorderColor * Alpha);
            spriteBatch.Draw(
                blank, 
                new Rectangle(r.Left, r.Bottom - BorderThickness, r.Width, BorderThickness),
                BorderColor * Alpha);

            // Draw the text centered in the button
            Vector2 textSize = font.MeasureString(Text);
            Vector2 textPosition = new Vector2(r.Center.X, r.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            spriteBatch.DrawString(font, Text, textPosition, TextColor * Alpha);
        }
    }
}
