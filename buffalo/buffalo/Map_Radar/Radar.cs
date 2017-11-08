using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace buffalo
{
    class Radar
    {
        private Texture2D radarLine;
        private float angle;
        private Vector2 position;
        private Vector2 origin;
        private Rectangle boundignBox;
        private float scale;
        public Radar(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            radarLine = Content.Load<Texture2D>("Radar-Rotor-Icon");

            angle = 0.5f;
            scale = 0.1700007f; //experiment
            position = new Vector2(315, 230); //experiment
            boundignBox = new Rectangle(0, 0, radarLine.Width, radarLine.Height);
            origin = new Vector2(1538, 1470);// boundignBox.Width / 2.0f, boundignBox.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(radarLine, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0f);
        }

        public void Update()
        {
            angle += 0.05f;
        }
    }
}
