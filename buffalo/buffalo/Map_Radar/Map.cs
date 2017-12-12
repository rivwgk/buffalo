using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using buffalo;

namespace buffalo
{
    class Map
    {
        private Insel[] _ilands;
        private Texture2D _myTex;
        private Vector2 _targetPos;
        public class MapPoint
        {
            public MapPoint(int ilandId, Vector2 position)
            {
                _ilandId = ilandId;
                _position = position;
            }
            public MapPoint()
            {
                _ilandId = -1;
            }
            private int _ilandId;
            public int GetID()
            { return _ilandId; }
            private Vector2 _position;
            public Vector2 GetPos()
            { return (_ilandId < 0 ? new Vector2(0,0) : _position); }
        }
        private bool IllandCollsion(int numIlands)
        {
            for (int i = 1; i < numIlands; ++i)
            {
                for(int j = i - 1; j >= 0; --j)
                {
                    if ((_ilands[i].GetPosition() - _ilands[j].GetPosition()).LengthSquared() < (_ilands[i].GetMaxRad() + _ilands[j].GetMaxRad()))
                        return true;
                }
            }
            return false;
        }
        public Map(int width, int height, int numIlands, Texture2D ilandTex)
        {
            ContentManager cM = ContentManager.Instance;
            _myTex = ilandTex;
            _ilands = new Insel[numIlands];
            Vector2[] randPos = new Vector2[numIlands];
            float[] rad = new float[numIlands];
            for (int i = 0; i < numIlands; ++i)
            {
                randPos[i] = new Vector2(cM.random.Next() % width, cM.random.Next() % height);
                rad[i] = cM.random.Next() % 300  + 200f;    //inseln der größe 200 - 500
            }
            int j = 0;
            do
            {
                _ilands[j] = new Insel(rad[j], 4, 0.6f, 0.8f, j, randPos[j]);   //place random iland
                if(this.IllandCollsion(j + 1) )
                {
                    //stuff
                }
                j++;
            } while (j < numIlands);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if( _myTex == null )
            {
                _myTex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _myTex.SetData<Color>(new Color[] { Color.White });
            }

            Vector2[] ilandCorner = _ilands[0].GetCorner();
            Point[] points = new Point[2];
            Vector2 edge;
            float angle;
            Vector2 origin = new Vector2(0f, 0f);
            for(int i = 0; i < ilandCorner.Length; ++i)
            {
                edge = (i > 0 ? ilandCorner[i - 1] : ilandCorner[ilandCorner.Length - 1]) - ilandCorner[i]; //kanten Vector
                angle = (float)Math.Atan2(edge.Y, edge.X);
                spriteBatch.Draw(
                    _myTex,
                    new Rectangle((int)ilandCorner[i].X, (int)ilandCorner[i].Y, (int)edge.Length(), 1),
                    null,
                   Color.Yellow,
                    angle,
                    origin,
                    SpriteEffects.None,
                    0
                    );
            }
        }
        //TODO
        public enum COLLISION{NOTHING, ISLAND, TARGET}; 
        public COLLISION submarineCollision(Vector2 position, float angle)
        {
            return COLLISION.NOTHING;
        }
        public MapPoint RdarDetection(Vector2 position, float length, float angle) //direction.Length is importend
        {
            Vector2 direction = new Vector2(length * (float)Math.Cos(angle), length * (float)Math.Sin(angle));
            MapPoint mapPoint = new MapPoint();
            for(int i = 0; i < _ilands.Length; ++i)
            {
                if(_ilands[i].getDistance(position) < length)
                {
                    mapPoint = _ilands[i].Collision(position, direction);
                    if (mapPoint.GetID() != -1)
                        break;
                }
            }
            return mapPoint;
        }
    }
}
