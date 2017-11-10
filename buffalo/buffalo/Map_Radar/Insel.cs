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
        Random _rnd;
        private const int CORER_RESULUTION = 10;
        private Vector2[] _corner;                  //Kordinates in Map Coordinates, NOT Px
        private Vector2 _position;
        private int _id;
        public Insel(float maxRad, int mainCornerNum, float regularity, float spiks, int id, Vector2 centerPosition)    //-, -, 1-0, 1-0
        {
            int subCornerSum = 0;                       //every Resulution Point more is async split from every edge))
            for (int i = 1; i <= CORER_RESULUTION; ++i)
                subCornerSum += subCornerSum + 1;

            _rnd = new Random();
            _position = centerPosition;
            _corner = new Vector2[mainCornerNum * subCornerSum];
            _id = id;
            regularity = 1f - regularity;                           //invert because is logical better

            float angle = 2 * (float)Math.PI / (float)mainCornerNum; 
            float[] angles = new float[mainCornerNum];
            angles[0] = 0f;
            float dAngle;

            for(int i = 1; i < mainCornerNum; ++i)  //init mainCorners on Rad pos
            {
                dAngle = angle + (2 * (float)_rnd.NextDouble() - 1f) * angle * regularity; //result C(6) -> 60° +- 60°*regularity
                angles[i] = angles[i - 1] + dAngle;
            }

            float resizeAngleScale = 2 * (float)Math.PI / angles[mainCornerNum - 1]; //resize angles, to set angleSUm to 2*PI
            float r;
            for(int i = 0; i < mainCornerNum; ++i) ///set corner Points
            {
                r = maxRad - maxRad * spiks * (float)_rnd.NextDouble();
                angles[i] *= resizeAngleScale; 
                _corner[(subCornerSum + 1) * i] = new Vector2((float)Math.Cos(angles[i]) * r, (float)Math.Sin(angles[i]) * r);
            }

            int subResulutionLvl = subCornerSum;
            for (int i = 0; i < CORER_RESULUTION; ++i)
            {
                int cornerNum = 0;
                subResulutionLvl /= 2;
                subResulutionLvl += 1;
                int run = 0;
                do
                {
                    run++;
                    cornerNum += subResulutionLvl;

                    if (run % 2 == 0)               //every second was already claimend by one resulution higher
                        continue;

                    //TODO
                    //get Corner befor and after get center, offset ~ 1/regularity, set point
                    //get ortogonal offset ~ spiks (max = distance to area from nect point
                } while (cornerNum < (subCornerSum + 1) * mainCornerNum);
            }
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
