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
        private const int CORER_RESULUTION = 5;
        private Vector2[] _corner;                  //Kordinates in Map Coordinates, NOT Px
        private float[] _cornerAngle;          
        private float VecAngle(Vector2 vec)         //return angle to vec(1, 0)
        {
            float angle = (float)Math.Cos(vec.X / vec.Length());
            if (vec.X < 0f)
                angle = (float)Math.PI - angle;
            return angle;
        }
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
        public Vector2 GetPosition() { return _position; }
        private int _id;
        private float _maxRad;
        public float GetMaxRad() { return _maxRad; }
        public Insel(float maxRad, int mainCornerNum, float regularity, float spiks, int id, Vector2 centerPosition)    //-, -, 1-0, 1-0
        {
            _maxRad = maxRad;
            int subCornerSum = 0;                       //every Resulution Point more is async split from every edge))
            for (int i = 1; i <= CORER_RESULUTION; ++i)
                subCornerSum += subCornerSum + 1;

            Console.WriteLine("ScubCorner:" + subCornerSum);
            _position = centerPosition;
            _corner = new Vector2[mainCornerNum * (subCornerSum + 1)];
            _id = id;
            regularity = 1f - regularity;                           //invert because is logical better

            float angle = 2 * (float)Math.PI / (float)mainCornerNum; 
            float[] angles = new float[mainCornerNum + 1];
            angles[0] = 0f;
            float dAngle;
            ContentManager cM = ContentManager.Instance;
            for(int i = 1; i < mainCornerNum + 1; ++i)  //init mainCorners on Rad pos
            {
                dAngle = angle + (2 * (float)cM.random.NextDouble() - 1f) * angle * regularity; //result C(6) -> 60° +- 60°*regularity
                angles[i] = angles[i - 1] + dAngle;
            }

            float resizeAngleScale = 2 * (float)Math.PI / angles[mainCornerNum]; //resize angles, to set angleSUm to 2*PI
            float r;
            for(int i = 0; i < mainCornerNum; ++i) ///set corner Points
            {
                r = maxRad - maxRad * spiks * (float)cM.random.NextDouble();
                Console.WriteLine(r);
                angles[i] *= resizeAngleScale;
                _corner[(subCornerSum + 1) * i] = new Vector2((float)Math.Cos(angles[i]) * r, (float)Math.Sin(angles[i]) * r);
            }

            regularity *= 0.5f;                 //because maximum +-0.25 center offset
            int subResulutionLvl = subCornerSum;

            byte[] signeds = new byte[((subCornerSum +1) * mainCornerNum + 7) / 8]; //+1 because of trumc, needed for direction of ortogonal offset    
            cM.random.NextBytes(signeds);
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
                    centerOffset = 0.5f + (float)cM.random.NextDouble() * regularity - regularity / 2;
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
                    ortogonalOffset = (float)cM.random.NextDouble() * spiks * ( sigend ? -1f : 1f);    //C# is unable to convert int too bool
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
                    //ortogonal transformation
                    ortogonalMaxOffset = Math.Abs(ortogonalMaxOffset);
                    if (ortogonalMaxOffset > 1)
                        ortogonalMaxOffset = 1f;
                    delta *= ortogonalMaxOffset * ortogonalOffset;

                    _corner[cornerNum] = newPos + delta;
                    if(_corner[cornerNum].Length() > maxRad)                    //avoid that points escape
                    {
                   //     _corner[cornerNum] *= maxRad / _corner[cornerNum].Length();
                    }

                    if (_corner[cornerNum].Length() > _maxRad)
                        maxRad = _corner[cornerNum].Length();
                } while (cornerNum + subResulutionLvl < (subCornerSum + 1) * mainCornerNum);
            }

            _cornerAngle = new float[_corner.Length];
            for(int i = 0; i < _corner.Length; ++i)
            {
                _cornerAngle[i] = VecAngle(_corner[i]);
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
            float minAngle = VecAngle(pos);
            float maxAngle = VecAngle(pos + direction);
            bool maxLoMin = maxAngle < minAngle;        //wenn min > max

            int i = 0;
            Map.MapPoint mapPoint;
            Vector2 interceptPoint = Vector2.Zero;
            Vector2 nerstPoint = direction;         //point wih largest distance
            Vector2 delta = new Vector2(0,0);
            do
            {
                delta = _corner[i == 0 ? _corner.Length - 1 : i - 1] - _corner[i];
                interceptPoint = Map_Radar.Algebra.Intercept(_corner[i], delta, pos, direction);
                if (interceptPoint != Vector2.Zero)
                    if ((interceptPoint - pos).Length() < nerstPoint.Length())
                        nerstPoint = interceptPoint;
                i++;
            } while (i < _corner.Length);

            if (nerstPoint != direction)
            {
                mapPoint = new Map.MapPoint(_id, nerstPoint - pos);
            }
            else
                mapPoint = new Map.MapPoint();
            return mapPoint;
        }
    }
}
