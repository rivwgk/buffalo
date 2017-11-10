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
        public Map(int width, int height, int numIlands, Texture2D ilandTex)
        {
            _myTex = ilandTex;
            _ilands = new Insel[1];//numIlands];
            for(int i = 0; i < numIlands; ++i)
            {
                _ilands[/*i*/0] = new Insel(200, 5, 0.5f, 0.8f, /*i*/0, new Vector2(300f, 250f));
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
    }
}
