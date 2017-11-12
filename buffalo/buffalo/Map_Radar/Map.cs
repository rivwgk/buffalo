﻿using System;
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
        public Map(int width, int height, int numIlands, Texture2D ilandTex)
        {
            _myTex = ilandTex;
            _ilands = new Insel[1];//numIlands];
            for(int i = 0; i < numIlands; ++i)
            {
                _ilands[/*i*/0] = new Insel(200, 10, 0.4f, 0.8f, /*i*/0, new Vector2(300f, 250f));
            }
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
