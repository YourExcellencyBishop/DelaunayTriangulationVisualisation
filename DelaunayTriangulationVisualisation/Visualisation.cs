using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DelaunayTriangulationVisualisation
{
    enum TriangulationState
    {
        AddingPoints
    }

    internal class Visualisation(Game game) : DrawableGameComponent(game)
    {
        public static HashSet<Vector2> points = [];

        public static TriangulationState state = TriangulationState.AddingPoints;

        public static void AddPoint()
        {
            if (Input.LeftMouseButtonPressed)
            {
                points.Add(Input.MousePosition.ToVector2());
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (state) 
            {
                case TriangulationState.AddingPoints:
                    AddPoint();
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (Vector2 v in Visualisation.points)
            {
                PrimitiveBatch.DrawSquare(v, 20, Game1.ShapeOutlineColor);
                PrimitiveBatch.DrawSquare(v, 8, Game1.ShapeColor);
            }
        }
    }
}
