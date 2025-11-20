using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DelaunayTriangulationVisualisation
{
    enum TriangulationState
    {
        AddingPoints,
        Triangulate,
        Triangulated
    }

    internal class Visualisation : DrawableGameComponent
    {
        static Game game;
        public static HashSet<Vector2> points = [];
        public static int[] indices;

        public static TriangulationState state = TriangulationState.AddingPoints;

        public Visualisation(Game game) : base(game) 
        {
            Visualisation.game = game;
        }

        public static void AddPoint()
        {
            if (Input.LeftMouseButtonPressed && game.IsActive)
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
                    if (Input.KeyClicked(Keys.Space))
                    {
                        state = TriangulationState.Triangulate;
                    }
                    break;
                case TriangulationState.Triangulate:
                    indices = DelaunayTriangulator.Triangulate(points.ToList());
                    state = TriangulationState.Triangulated;
                    break;
                case TriangulationState.Triangulated:
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            

            if (state == TriangulationState.Triangulated)
            {
                var pointsList = points.ToList();
                for (int i = 0; i < indices.Length; i++) 
                {
                    PrimitiveBatch.DrawLine2(pointsList[indices[3 * (i / 3) + i % 3]], pointsList[indices[3 * (i / 3) + (i + 1) % 3]], Game1.ShapeOutlineColor);
                }
            }

            foreach (Vector2 v in Visualisation.points)
            {
                PrimitiveBatch.DrawCircle(v, 15, 25, Game1.ShapeOutlineColor);
                PrimitiveBatch.DrawCircle(v, 10, 25, Game1.ShapeColor);
                //PrimitiveBatch.DrawSquare(v, 20, Game1.ShapeOutlineColor);
                //PrimitiveBatch.DrawSquare(v, 8, Game1.ShapeColor);
            }
        }
    }
}
