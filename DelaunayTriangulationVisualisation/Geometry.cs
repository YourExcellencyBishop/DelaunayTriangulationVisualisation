using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DelaunayTriangulationVisualisation
{
    public static class Geometry
    {
        public static Vector2 FindCentreOfVertices(IList<Vector2> vertices)
        {
            Vector2 centre = Vector2.Zero;

            foreach (var v in vertices)
            {
                centre += v;
            }

            return centre / vertices.Count;
        }

        public static float Cross(Vector2 v1, Vector2 v2)
        {
            return v2.X * v1.Y - v2.Y * v1.X;
        }

        public static void Transform(IList<ValueWrapper<Vector2>> sourceArray, ref Matrix matrix, Vector2[] destinationArray)
        {
            for (int i = 0; i < sourceArray.Count; i++)
            {
                destinationArray[i] = Vector2.Transform(sourceArray[i].Value, matrix);
            }
        }
    }
}
