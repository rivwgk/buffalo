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

        Vector2 subPos;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pult;
        Texture2D kurs;
        Texture2D stuff1, stuff2;
        Radar radar;
        Map map;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            subPos = new Vector2(500, 200);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            pult = Content.Load<Texture2D>("Oberfläche-Pult");
            kurs = Content.Load<Texture2D>("Rahmen-Kursregler");
            stuff1 = Content.Load<Texture2D>("Rahmen-Schieberegler");
            stuff2 = Content.Load<Texture2D>("Schieberegler");
            map = new Map(100, 100, 1, null);
            radar = new Radar(Content, map);
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

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                subPos.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                subPos.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                subPos.X -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                subPos.X += 1;
            if (!_mapNew && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                map = new Map(100, 100, 1, null);
                _mapNew = true;
            }
            if (_mapNew && Keyboard.GetState().IsKeyUp(Keys.R))
                _mapNew = false;
            radar.Update(subPos);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PaleVioletRed);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(pult, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            spriteBatch.Draw(kurs, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            spriteBatch.Draw(stuff2, new Rectangle(REGLER_SKALE_X, REGLER_SKALE_Y, REGLER_SKALE_WIDTH, REGLER_SKALE_HEIGHT), Color.White);
            spriteBatch.Draw(stuff1, GraphicsDevice.PresentationParameters.Bounds, Color.White);
            radar.Draw(spriteBatch);
            map.Draw(spriteBatch); //only test actuallay
            spriteBatch.Draw(kurs, new Rectangle((int)subPos.X - 20, (int)subPos.Y - 20, 40, 40), Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
