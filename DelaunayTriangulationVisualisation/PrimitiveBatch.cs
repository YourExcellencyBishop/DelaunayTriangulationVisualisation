using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DelaunayTriangulationVisualisation
{
    public struct ShapeData : IPrimitive
    {
        public VertexPositionColor[] Vertices { get; set; }
        public int[] Indices { get; set; }
        public readonly int vertexCount;
        public readonly int primitiveCount;
        public readonly RasterizerState rasterizerState;

        public ShapeData(VertexPositionColor[] vertices, int[] indices, RasterizerState rasterizer = null)
        {
            this.Vertices = vertices;
            vertexCount = this.Vertices.Length;
            this.Indices = indices;
            primitiveCount = this.Indices.Length / 3;
            rasterizerState = rasterizer ?? PrimitiveBatch.DefaultRS;
        }
    }

    public struct LineData : IPrimitive
    {
        public VertexPositionColor[] Vertices { get; set; }
        public int[] Indices { get; set; }
        public readonly int pointCount;
        public readonly int primitiveCount;

        public LineData(VertexPositionColor[] points, int[] indices)
        {
            this.Vertices = points;
            this.Indices = indices;
            pointCount = this.Vertices.Length;
            primitiveCount = pointCount - 1;
        }
    }

    public struct PointData : IPrimitive
    {
        public VertexPositionColor[] Vertices { get; set; }
        public int[] Indices { get; set; } = [0];

        public readonly int pointCount;
        public readonly int primitiveCount;

        public PointData(VertexPositionColor[] points)
        {
            Vertices = points;
            Indices = new int[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++) Indices[i] = i;
            pointCount = this.Vertices.Length;
            primitiveCount = pointCount;
        }
    }

    public static partial class PrimitiveBatch
    {
        private static Matrix world, view, projection;

        public static RasterizerState DefaultRS = new()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Solid
        };

        public static RasterizerState WireFrameRS = new()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.WireFrame
        };

        public static readonly List<IPrimitive> primitives = [];

        public readonly static Effect effect;

        static PrimitiveBatch()
        {
            world = Matrix.Identity;
            view = Matrix.Identity;
            effect = Game1.ContentManager.Load<Effect>("Primitive");
        }

        public static void Begin(int viewWidth, int viewHeight)
        {
            projection = Matrix.CreateOrthographicOffCenter(0, viewWidth, viewHeight, 0, 0, 1);
            effect.Parameters["WorldViewProjection"].SetValue(world * view * (Matrix.CreateTranslation(-0.5f, -0.5f, 0) * projection));
        }

        public static void Flush()
        {
            if (primitives.Count == 0) return;

            foreach (object primitive in primitives)
            {
                if (primitive is ShapeData shape) DrawShapePrimitive(shape);
                else if (primitive is LineData line) DrawLinePrimitive(line);
                else if (primitive is PointData point) DrawPointPrimitive(point);
                else throw new Exception("Unrecognised Primitive");
            }

            primitives.Clear();
        }

        public static void End()
        {
            Flush();
        }

        public static void DrawPolygon(Vector2[] vertices, int[] indices, Color color, Vector2? translation = null, float rotation = 0, RasterizerState rasterizerState = null)
        {
            Matrix rotationMat = Matrix.CreateRotationZ(rotation);
            Vector2[] rotatedVertices = new Vector2[vertices.Length];
            Vector2.Transform(vertices, ref rotationMat, rotatedVertices);

            primitives.Add(new ShapeData(Vector2ToVertex(rotatedVertices, color, translation), indices, rasterizerState));
        }

        public static void DrawPolygon(IList<ValueWrapper<Vector2>> vertices, int[] indices, Color color, Vector2? translation = null, float rotation = 0, RasterizerState rasterizerState = null)
        {
            Matrix rotationMat = Matrix.CreateRotationZ(rotation);
            Vector2[] rotatedVertices = new Vector2[vertices.Count];
            Geometry.Transform(vertices, ref rotationMat, rotatedVertices);

            primitives.Add(new ShapeData(Vector2ToVertex(rotatedVertices, color, translation), indices, rasterizerState));
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            primitives.Add(new LineData(Vector2ToVertex([start, end], color), [0, 1]));
        }

        public static void DrawPoint(Vector2 position, Color color)
        {
            primitives.Add(new PointData(Vector2ToVertex([position], color)));
        }

        public static void DrawPoints(Vector2[] positions, Color color)
        {
            primitives.Add(new PointData(Vector2ToVertex(positions, color)));
        }

        public static VertexPositionColor[] Vector2ToVertex(Vector2[] vectors, Color color, Vector2? translation = null)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[vectors.Length];
            Vector2 t = translation ?? Vector2.Zero;

            for (int i = 0; i < vectors.Length; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3(vectors[i] + t, 0), color);
            }

            return vertices;
        }

        private static void DrawShapePrimitive(ShapeData shape)
        {
            Game1.Renderer.RasterizerState = shape.rasterizerState;

            foreach (EffectPass pass in effect.Techniques[0].Passes)
            {
                pass.Apply();

                Game1.Renderer.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    shape.Vertices,
                    0,
                    shape.vertexCount,
                    shape.Indices,
                    0,
                    shape.primitiveCount
                );
            }
        }

        private static void DrawLinePrimitive(LineData line)
        {
            Game1.Renderer.RasterizerState = DefaultRS;

            foreach (EffectPass pass in effect.Techniques[0].Passes)
            {
                pass.Apply();

                Game1.Renderer.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    line.Vertices,
                    0,
                    line.pointCount,
                    line.Indices,
                    0,
                    line.primitiveCount
                );
            }
        }

        private static void DrawPointPrimitive(PointData point)
        {
            Game1.Renderer.RasterizerState = DefaultRS;

            foreach (EffectPass pass in effect.Techniques[0].Passes)
            {
                pass.Apply();

                Game1.Renderer.DrawUserIndexedPrimitives(
                    PrimitiveType.PointList,
                    point.Vertices,
                    0,
                    point.pointCount,
                    point.Indices,
                    0,
                    point.primitiveCount
                );
            }
        }

        public static void DrawSquare(Vector2 position, float size, Color color)
        {
            float halfSize = size * 0.5f;

            Vector2[] vertices =
            [
                new Vector2(-halfSize) + position,
                new Vector2(halfSize, -halfSize) + position,
                new Vector2(-halfSize, halfSize) + position,
                new Vector2(halfSize) + position
            ];

            DrawPolygon(vertices, [0, 1, 2, 1, 2, 3], color);
        }
    }
}
