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
    class Insel
    {
        private Vector2[] _corner;
        private Vector2 _position;
        private int _id;
        public Insel(float maxRad, int mainCornerNum, float regularity, float spiks)    //-, -, 1-0, 1-0
        {

        }

        public float getDistance(Vector2 pos)
        {
            return (_position - pos).Length();
        }

        public bool collision(Vector2 pos, float rot)       //collision with submarin, pos Submarin + direction (eliips Collisionbox)
        {
            return false;
        }

        public Vector2 collision(Vector2 pos, Vector2 direction)    //return first Collision Point between strahl and Iland
        {
            return new Vector2(0, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
