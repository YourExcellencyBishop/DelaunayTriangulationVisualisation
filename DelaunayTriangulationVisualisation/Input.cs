using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DelaunayTriangulationVisualisation
{
    public class Input(Game game) : GameComponent(game)
    {
        private static MouseState currMouseState;
        private static MouseState prevMouseState;
        private static KeyboardState currKeyboardState;
        private static KeyboardState prevKeyboardState;
        private static GamePadState currGamePadState;
        private static GamePadState prevGamePadState;

        public override void Update(GameTime gameTime)
        {
            prevMouseState = currMouseState;
            currMouseState = Mouse.GetState();

            prevKeyboardState = currKeyboardState;
            currKeyboardState = Keyboard.GetState();

            prevGamePadState = currGamePadState;
            currGamePadState = GamePad.GetState(0);
        }

        public static bool LeftMouseButtonDown { get => currMouseState.LeftButton == ButtonState.Pressed; }
        public static bool RightMouseButtonDown { get => currMouseState.RightButton == ButtonState.Pressed; }
        public static bool MiddleMouseButtonDown { get => currMouseState.MiddleButton == ButtonState.Pressed; }

        public static bool LeftMouseButtonUp { get => currMouseState.LeftButton == ButtonState.Released; }
        public static bool RightMouseButtonUp { get => currMouseState.RightButton == ButtonState.Released; }
        public static bool MiddleMouseButtonUp { get => currMouseState.MiddleButton == ButtonState.Released; }

        public static bool LeftMouseButtonPressed { get => currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released; }
        public static bool RightMouseButtonPressed { get => currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released; }
        public static bool MiddleMouseButtonPressed { get => currMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released; }

        public static bool LeftMouseButtonReleased { get => currMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed; }
        public static bool RightMouseButtonReleased { get => currMouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed; }
        public static bool MiddleMouseButtonReleased { get => currMouseState.MiddleButton == ButtonState.Released && prevMouseState.MiddleButton == ButtonState.Pressed; }

        public static bool MouseScrollUp { get => currMouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue; }

        public static bool MouseScrollDown { get => currMouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue; }

        public static bool AnyKeyClicked { get => currKeyboardState.GetPressedKeyCount() > 0; }

        public static Point MousePosition { get => currMouseState.Position; }

        public static Vector2 LeftStick { get => currGamePadState.ThumbSticks.Left * new Vector2(1, -1); }
        public static Vector2 RightStick { get => currGamePadState.ThumbSticks.Right * new Vector2(1, -1); }

        public static bool KeyDown(Keys key) => currKeyboardState.IsKeyDown(key);
        public static bool KeyClicked(Keys key) => currKeyboardState.IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);

        public static bool ButtonDown(Buttons button) => currGamePadState.IsButtonDown(button);
        public static bool ButtonPressed(Buttons button) => currGamePadState.IsButtonDown(button) && prevGamePadState.IsButtonUp(button);

        public static bool GetMovementDirection(out Vector3 direction,
            Keys Left = Keys.Left, Keys Right = Keys.Right, Keys Up = Keys.I, Keys Down = Keys.K, Keys Forwards = Keys.Up, Keys Backwards = Keys.Down)
        {
            direction = Vector3.Zero;

            if (KeyDown(Left)) direction.X -= 1;
            if (KeyDown(Right)) direction.X += 1;
            if (KeyDown(Up)) direction.Y -= 1;
            if (KeyDown(Down)) direction.Y += 1;
            if (KeyDown(Backwards)) direction.Z -= 1;
            if (KeyDown(Forwards)) direction.Z += 1;

            if (direction != Vector3.Zero)
            {
                direction.Normalize();
                return true;
            }

            return false;
        }

    }
}
