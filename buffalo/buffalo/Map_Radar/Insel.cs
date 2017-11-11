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
        private const int CORER_RESULUTION = 3;
        private Vector2[] _corner;                  //Kordinates in Map Coordinates, NOT Px
        private float[] _cornerAngle;               //cos(rad)
        public Vector2[] GetCorner()
        {
            Vector2[] result = new Vector2[_corner.Length]; 
            for(int i = 0; i < _corner.Length; ++i)             //alle Ecken Mappen
            {
                result[i] = _corner[i] + _position;
            }
            return result;
        }
        private Vector2 _position;
        private int _id;
        private float _maxRad;
        public Insel(float maxRad, int mainCornerNum, float regularity, float spiks, int id, Vector2 centerPosition)    //-, -, 1-0, 1-0
        {
            _maxRad = 0f;
            int subCornerSum = 0;                       //every Resulution Point more is async split from every edge))
            for (int i = 1; i <= CORER_RESULUTION; ++i)
                subCornerSum += subCornerSum + 1;
            Console.WriteLine("ScubCorner:" + subCornerSum);
            _rnd = new Random();
            _position = centerPosition;
            _corner = new Vector2[mainCornerNum * (subCornerSum + 1)];
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

            regularity *= 0.5f;                 //because maximum +-0.25 center offset
            int subResulutionLvl = subCornerSum;

            byte[] signeds = new byte[((subCornerSum +1) * mainCornerNum + 7) / 8]; //+1 because of trumc, needed for direction of ortogonal offset    
            _rnd.NextBytes(signeds);
            float ortogonalOffset;
            float centerOffset;
            float ortogonalMaxOffset;
            Vector2[] nighbarPoints = new Vector2[2];

            for (int i = 0; i < CORER_RESULUTION; ++i)
            {
                int cornerNum = 0;
                subResulutionLvl = (int)Math.Round(subResulutionLvl / 2f);
                Console.WriteLine(subResulutionLvl);
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
                    nighbarPoints[0] = _corner[cornerNum - subResulutionLvl];
                    nighbarPoints[1] = _corner[ (cornerNum + subResulutionLvl >= _corner.Length) ? cornerNum + subResulutionLvl - _corner.Length : cornerNum + subResulutionLvl ];

                    Vector2 delta = nighbarPoints[1] - nighbarPoints[0];
                    centerOffset = 0.5f + (float)_rnd.NextDouble() * regularity - regularity / 2;
                    delta *= centerOffset;
                    Vector2 newPos = nighbarPoints[0] + delta;
                    /*
                     * rotation Matrix: cos -sin    positiv rotation
                     *                  sin  cos
                     * 90° ->           0   -1  *   x   ->  0x -  y
                     *                  1    0      y       x  + 0y
                     *  => x    ->  -y
                     *     y         x
                     * --------------------------------------------------------
                     * corner[i-1].r ? corner[i+1].r | siegnd | colision corner
                     *              >                   +           corner[i-1]
                     *              >                   -           corner[i+1]
                     *              <                   +           corner[i+1]
                     *              <                   -           corner[i-1]
                     * schnittpunkt zweier Geraden in Vectordarstellung a + r * x  | c + t * o
                     *                                                  b       y  | d       p
                     * r = ( (b-d) * o / p - a + c ) / ( x - y * o / p )
                     * vec(c,d) = 0
                     * -> r = ( b * o / p - a ) / ( x - y * o / p)
                     * vec(o,p) = _corner[i+-1].pos
                     * vec(a,b) = newPos
                     * vec(x,y) = rotated delta
                    */
                    bool sigend = (signeds[cornerNum / 8] & (1 << cornerNum % 8)) > 0;
                    ortogonalOffset = (float)_rnd.NextDouble() * spiks * ( sigend ? -1f : 1f);    //C# is unable to convert int too bool
                    bool realation = nighbarPoints[0].LengthSquared() > nighbarPoints[1].LengthSquared();
                    Vector2 collisionCorner; //vec(o,p);
                    if (sigend)                                                                   //rotation 90° + or -
                    {
                        delta = new Vector2(-delta.Y, delta.X);
                        collisionCorner = nighbarPoints[0].LengthSquared() > nighbarPoints[1].LengthSquared() ? nighbarPoints[0] : nighbarPoints[1];
                    }
                    else
                    {
                        delta = new Vector2(delta.Y, -delta.X);
                        collisionCorner = nighbarPoints[0].LengthSquared() > nighbarPoints[1].LengthSquared() ? nighbarPoints[1] : nighbarPoints[0];
                    }
                    if (delta.X - delta.Y * newPos.X / newPos.Y == 0f)
                        ortogonalMaxOffset = 1f;
                    else
                        ortogonalMaxOffset = ( newPos.Y * collisionCorner.X / collisionCorner.Y - newPos.X ) / ( delta.X - delta.Y * newPos.X / newPos.Y );
                    //TODO only beacause Test start
                    ortogonalMaxOffset = Math.Abs(ortogonalMaxOffset);
                    if (ortogonalMaxOffset > 1)
                        ortogonalMaxOffset = 1f;
                    delta *= ortogonalMaxOffset * ortogonalOffset;
                    //Test end
                    _corner[cornerNum] = newPos + delta;
                    if(_corner[cornerNum].Length() > maxRad)                    //avoid that points escape
                    {
                        _corner[cornerNum] *= maxRad / _corner[cornerNum].Length();
                    }

                    if (_corner[cornerNum].Length() > _maxRad)
                        maxRad = _corner[cornerNum].Length();
                } while (cornerNum + subResulutionLvl < (subCornerSum + 1) * mainCornerNum);
            }

            _cornerAngle = new float[_corner.Length];
            for(int i = 0; i < _corner.Length; ++i)
            {
                _cornerAngle[i] =_corner[i].X / _corner[i].Length(); //winkel zur Wagerechten
            }
        }

        public float getDistance(Vector2 pos)               //return minimal posible distance approximated
        {
            return (_position - pos).Length() - _maxRad;
        }

        public bool Collision(Vector2 pos, float rot)       //collision with submarin, pos Submarin + direction (eliips Collisionbox)
        {
            return false;
        }

        public Map.MapPoint Collision(Vector2 pos, Vector2 direction)    //return first Collision Point between strahl and Iland, direction.Length is importend
        {
            pos -= _position;           //transform Koordinatensystem, now cenmter iland is center
            float minAngle = pos.X / pos.Length();
            float maxAngle = (pos.X + direction.X) / (pos + direction).Length();
            int i = 0;
            bool[] sigend = new bool[2] ;                //safes the last signed from the operateion ortogonalVec(direction) * _corner (beacuse, when the signed switch -> the last poiont and this have cross the radar line
            Vector2 delta;              //delta vec to calculate sigend
            Map.MapPoint mapPoint;

            direction = new Vector2(direction.Y, -direction.X);         //ortogonal Vec

            while (_cornerAngle[i] < minAngle) ++i; //go to the first possiblepoint
            i--;
            //TODO overthinking problem: cos is not eindeutig, rotation diretion is not equal
            delta = _corner[i] - pos;
            sigend[1] = (direction.X * delta.X + direction.Y * delta.Y > 0f);
            if (maxAngle < minAngle)
                maxAngle += (float)Math.PI * 2f;
            float overflow = 0f;
            do
            {
                i++;
                if (i == _corner.Length)
                {
                    i = 0;
                    overflow = (float)Math.PI * 2f;
                }
                sigend[0] = sigend[1];
                delta = _corner[i] - pos;
                sigend[1] = (direction.X * delta.X + direction.Y * delta.Y > 0f);
            } while (sigend[1] == sigend[0] && _cornerAngle[i] + overflow <= maxAngle);

            if (sigend[1] != sigend[0])
            {
                delta = (_corner[i] + _corner[i - 1]) * 0.5f;
                mapPoint = new Map.MapPoint(_id, delta - pos);
            }
            else
                mapPoint = new Map.MapPoint();
            return mapPoint;
        }
    }
}
