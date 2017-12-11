using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace buffalo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const int REGLER_SKALE_X = 56;
        const int REGLER_SKALE_Y = 114;
        const int REGLER_SKALE_WIDTH = 41;
        const int REGLER_SKALE_HEIGHT = 111;
        const int MODE_SUBMARINE = 0;
        const int MODE_MAP = 1;

        Vector2 _subPos;
        GraphicsDeviceManager _graphics;
        ContentManager _contentManager;
        SpriteBatch _spriteBatch;
        Texture2D _pult;
        Texture2D _kurs;
        Texture2D _assat1, _stuff2;
        Radar _radar;
        Map _map;
        int _mode;
        System.Collections.Generic.Dictionary<Keys, bool> _keyPressed;

        public Game1()
        {
            _contentManager = ContentManager.Instance;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _mode = MODE_SUBMARINE;
            _keyPressed = new System.Collections.Generic.Dictionary<Keys, bool>();
            _keyPressed[Keys.M] = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _subPos = new Vector2(500, 200);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _pult = Content.Load<Texture2D>("Oberfläche-Pult");
            _kurs = Content.Load<Texture2D>("Rahmen-Kursregler");
            _assat1 = Content.Load<Texture2D>("Rahmen-Schieberegler");
            _stuff2 = Content.Load<Texture2D>("Schieberegler");
            _map = new Map(100, 100, 1, null);
            _radar = new Radar(Content, _map);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        bool _mapNew = true; //TODO only Temporer for test
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.W)) { _subPos.Y -= 5; }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { _subPos.Y += 5; }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { _subPos.X -= 5; }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { _subPos.X += 5; }
            if (! _keyPressed[Keys.M] && Keyboard.GetState().IsKeyDown(Keys.M))
            {
                _keyPressed[Keys.M] = true;
                switch(_mode)
                {
                    case MODE_SUBMARINE: _mode = MODE_MAP;
                        break;
                    case MODE_MAP: _mode = MODE_SUBMARINE;
                        break;
                    default: _mode = MODE_SUBMARINE;
                        break;
                }
            }
            else if(_keyPressed[Keys.M] && !Keyboard.GetState().IsKeyDown(Keys.M))    //nicht gedrückt aber true -> dfalse
            {
                _keyPressed[Keys.M] = false;
            }

            _radar.Update(_subPos);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (_mode)
            {
                case MODE_SUBMARINE:
                    {
                        GraphicsDevice.Clear(Color.PaleVioletRed);

                        // TODO: Add your drawing code here
                        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                        _spriteBatch.Draw(_pult, GraphicsDevice.PresentationParameters.Bounds, Color.White);
                        _spriteBatch.Draw(_kurs, GraphicsDevice.PresentationParameters.Bounds, Color.White);
                        _spriteBatch.Draw(_stuff2, new Rectangle(REGLER_SKALE_X, REGLER_SKALE_Y, REGLER_SKALE_WIDTH, REGLER_SKALE_HEIGHT), Color.White);
                        _spriteBatch.Draw(_assat1, GraphicsDevice.PresentationParameters.Bounds, Color.White);
                        _radar.Draw(_spriteBatch);

                        _spriteBatch.End();
                    }break;
                case MODE_MAP:
                    {
                        GraphicsDevice.Clear(Color.SandyBrown);
                        _spriteBatch.Begin();
                        _map.Draw(_spriteBatch);
                        _spriteBatch.Draw(_kurs, new Rectangle((int)_subPos.X - 5, (int)_subPos.Y - 5, 10, 10), Color.White);
                        _spriteBatch.End();
                    }break;
            }
            base.Draw(gameTime);
        }
    }
}
