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
    class Radar
    {
        const float MIN_INTZESITY = 0.05f;
        const float FADEOUT_RATE = 0.97f; //intensity *= fadeOutRate
        const int RADAR_DOT_SIZE = 5;
        const float RADAR_OFFSET = 22f/180f * (float)Math.PI;
        const float RADAR_DISPLAY_RAD = 130f;
        const float RADAR_RANGE = 100f;

        private Texture2D _radarLine;
        private Texture2D _radarDot;
        private float _angle;
        private Vector2 _centerPosition;
        private Vector2 _origin;
        private float _scale;
        private Map _map;
        private class RadarPoints
        {
            private int _resulution;
            public int GetResulution() { return _resulution; }
            private float _step;
            public float GetStep() { return _step; }

            private Vector2 _centerPosition; //dx and dy to the left upper Corner from the Window to the radar Center
            Texture2D _radarDot;
            private int _lastUpdatedAngle;

            private class Points
            {
                
                private Vector2 _pos;
                public Vector2 GetPos() { return _pos; }

                private int _objId;
                public int GetObjId() { return _objId; }

                private float _intesity;
                public float GetIntensity() { return _intesity; }

                public Points()
                {
                    _objId = -1; //unselected
                    _pos = new Vector2(0, 0);
                    _intesity = 0.0f;
                }
                public void Update()
                {
                    if (_objId == -1)
                        return;

                    _intesity *= FADEOUT_RATE;          //fadeout

                    if (_intesity < MIN_INTZESITY)  //delet
                    {
                        _intesity = 0f;
                        _objId = -1;    
                    }
                }
                
                public void SetPoint(int objId, Vector2 pos)
                {
                    _objId = objId;
                    _pos = pos;
                    _intesity = 1f;
                }
            }
            private Points[] _points;

            public RadarPoints(int resulution, Vector2 centerPosition, Texture2D radarDot)
            {
                _lastUpdatedAngle = -1;
                _radarDot = radarDot;
                _centerPosition = centerPosition;
                _resulution = resulution;
                _step = (float)Math.PI * 2 / resulution;
                Console.WriteLine(_step);
                _points = new Points[_resulution];
                for(int i = 0; i < _resulution; ++i)
                {
                    _points[i] = new Points();
                }
            }
            public void SetPoint(float angle, Vector2 position, int objId)
            {
                int radPos = (int)(angle / (float)Math.PI / 2f * _resulution);
                if (radPos != _lastUpdatedAngle)                                     //dont overwrite a actually new Point
                {
                    _points[radPos].SetPoint(objId, position);
                    _lastUpdatedAngle = radPos;
                }
            }
            public void SetPoint(float angle, float distance, int objId)
            {
                int radPos = (int) (angle / (float)Math.PI / 2f * _resulution);
                if (radPos != _lastUpdatedAngle)                                     //dont overwrite a actually new Point
                {
                    _points[radPos].SetPoint(objId, new Vector2((float)Math.Cos(angle) * distance, (float)Math.Sin(angle) * distance));
                    _lastUpdatedAngle = radPos;
                }
            }

            public void Upadte()
            {
                foreach (Points p in _points)
                {
                    p.Update();
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                Points prev = _points[_points.Length - 1];
                foreach(Points p in _points)
                {
                    if (p.GetObjId() != -1)
                    {
                        int x = (int)(p.GetPos().X + _centerPosition.X);
                        int y = (int)(p.GetPos().Y + _centerPosition.Y);
                        if (y % RADAR_DOT_SIZE > RADAR_DOT_SIZE / 2)
                            y += RADAR_DOT_SIZE;
                        if (x % RADAR_DOT_SIZE > RADAR_DOT_SIZE / 2)
                            x += RADAR_DOT_SIZE;
                        x -= x % RADAR_DOT_SIZE;                            //fancy Pixel eindruck
                        y -= y % RADAR_DOT_SIZE;

                            spriteBatch.Draw(
                                _radarDot,
                                new Rectangle(x ,y, RADAR_DOT_SIZE, RADAR_DOT_SIZE),
                                Color.White * p.GetIntensity()
                                );
                    }
                    prev = p;
                }
            }

        }
        RadarPoints _radarPoints;
        Random _rnd;
        public Radar(Microsoft.Xna.Framework.Content.ContentManager Content, Map map)
        {
            _rnd = new Random();
            _map = map;
            _radarLine = Content.Load<Texture2D>("Radar-Rotor-Icon");
            _radarDot = Content.Load<Texture2D>("Radar-Objekt-Icon");

            _angle = 0.5f;
            _scale = 0.1700007f; //experiment
            _centerPosition = new Vector2(315, 230); //experiment
            _origin = new Vector2(1538, 1470);// boundignBox.Width / 2.0f, boundignBox.Height);
            _radarPoints = new RadarPoints(1024, _centerPosition, _radarDot);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_radarLine, _centerPosition, null, Color.White, _angle + RADAR_OFFSET, _origin, _scale, SpriteEffects.None, 0f);
            _radarPoints.Draw(spriteBatch);
        }

        public void Update(Vector2 suPos)
        {
            _angle += 0.05f;
            if (_angle > Math.PI * 2)
                _angle = 0f;
            _radarPoints.Upadte();
            {
                Map.MapPoint collisionPoint = _map.RdarDetection(suPos, RADAR_RANGE, _angle);
                if(collisionPoint.GetID() != -1)
                    _radarPoints.SetPoint(_angle, collisionPoint.GetPos() / RADAR_RANGE * RADAR_DISPLAY_RAD, collisionPoint.GetID());
            }
        }
    }
}
