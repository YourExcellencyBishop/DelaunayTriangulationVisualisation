using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace DelaunayTriangulationVisualisation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        public static GraphicsDevice Renderer;
        public static ContentManager ContentManager;

        private Color BackgroundColor = new(0, 127, 255); // Azure Blue
        private Color ShapeOutlineColor = new(44, 19, 32); // Midnight Violet
        private Color ShapeColor = new(95, 75, 102); // Vintage Grape

        public static readonly (int Width, int Height) Resolution = (1280, 720);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Resolution.Width,
                PreferredBackBufferHeight = Resolution.Height
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Renderer = GraphicsDevice;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            ContentManager = Content;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            // TODO: Add your drawing code here

            PrimitiveBatch.Begin(Resolution.Width, Resolution.Height);

            PrimitiveBatch.DrawSquare(new Vector2(Resolution.Width / 2, Resolution.Height / 2), 70, ShapeOutlineColor);
            PrimitiveBatch.DrawSquare(new Vector2(Resolution.Width/2, Resolution.Height/2), 30, ShapeColor);

            PrimitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}
