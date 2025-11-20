using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DelaunayTriangulationVisualisation
{
    public struct Triangle(int V1 = -1, int V2 = -1, int V3 = -1, int N1 = 0, int N2 = 0, int N3 = 0)
    {
        public int V1 = V1, V2 = V2, V3 = V3;
        public int N1 = N1, N2 = N2, N3 = N3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PrevIndex(int i) => (i == 1 ? 3 : i - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextIndex(int i) => (i % 3) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVertex(in Triangle t, int index)
        {
            return index switch
            {
                1 => t.V1,
                2 => t.V2,
                3 => t.V3,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVertex(ref Triangle t, int index, int value)
        {
            switch (index)
            {
                case 1: t.V1 = value; break;
                case 2: t.V2 = value; break;
                case 3: t.V3 = value; break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNeighbor(in Triangle t, int index)
        {
            return index switch
            {
                1 => t.N1,
                2 => t.N2,
                3 => t.N3,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetNeighbor(ref Triangle t, int index, int value)
        {
            switch (index)
            {
                case 1: t.N1 = value; break;
                case 2: t.N2 = value; break;
                case 3: t.N3 = value; break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class DelaunayTriangulator
    {
        public static int[] Triangulate<T>(IList<T> points)
        {
            int vertexCount = points.Count;
            int sloanVertexCount = vertexCount + 4;
            Vector2[] vertices = new Vector2[sloanVertexCount];
            int[] listPoints = new int[vertexCount];
            for (int i = 1; i <= vertexCount; i++) listPoints[i - 1] = i;

            if (points is IList<Vector2> pureVertices)
            {
                for (int i = 1; i <= vertexCount; i++)
                {
                    vertices[i] = pureVertices[i - 1];
                    vertices[i].Y = -vertices[i].Y;
                }
            }
            else if (points is IList<ValueWrapper<Vector2>> wrappedVertices)
            {
                for (int i = 1; i <= vertexCount; i++)
                {
                    vertices[i] = wrappedVertices[i - 1];
                    vertices[i].Y = -vertices[i].Y;
                }
            }
            else
            {
                throw new Exception("What is this kind of list my brother???");
            }

            float xMin = vertices[1].X;
            float xMax = xMin;
            float yMin = vertices[1].Y;
            float yMax = yMin;

            for (int i = 2; i <= vertexCount; i++)
            {
                var v = vertices[i];
                if (v.X < xMin) xMin = v.X;
                if (v.X > xMax) xMax = v.X;
                if (v.Y < yMin) yMin = v.Y;
                if (v.Y > yMax) yMax = v.Y;
            }

            float xDiff = xMax - xMin;
            float yDiff = yMax - yMin;
            float dMax = MathF.Max(xDiff, yDiff);
            float fact = 1.0f / dMax;

            for (int i = 1; i <= vertexCount; i++)
            {
                ref Vector2 v = ref vertices[i];
                v.X = (v.X - xMin) * fact;
                v.Y = (v.Y - yMin) * fact;
            }

            int nDiv = (int)MathF.Round(MathF.Pow(vertexCount, 0.25f));
            float factX = nDiv / (xDiff * 1.01f / dMax);
            float factY = nDiv / (yDiff * 1.01f / dMax);

            int[] binKeys = new int[vertexCount + 1];
            for (int k = 1; k <= vertexCount; k++)
            {
                int i = (int)(vertices[k].Y * factY);
                int j = (int)(vertices[k].X * factX);

                if (i % 2 == 0) binKeys[k] = i * nDiv + j;
                else binKeys[k] = (i + 1) * nDiv - j - 1;
            }

            Array.Sort(listPoints, 0, vertexCount,
                Comparer<int>.Create((v1, v2) => binKeys[v1].CompareTo(binKeys[v2])));

            Triangle[] triangles = new Triangle[2 * vertexCount + 1 + 1];

            int numpts = vertexCount;

            int A, B, C;
            int L, R;
            int V1 = numpts + 1, V2 = numpts + 2, V3 = numpts + 3;
            triangles[1] = new Triangle(V1, V2, V3);
            vertices[V1] = new Vector2(-100, -100);
            vertices[V2] = new Vector2(100, -100);
            vertices[V3] = new Vector2(0, 100);

            Stack<int> stack = new();
            int numtri = 1;

            for (int q = 1; q <= vertexCount; q++)
            {
                int p = listPoints[q - 1];
                Vector2 point = vertices[p];

                int triangle = LocateTriangle(point, vertices, triangles, numtri);

                A = triangles[triangle].N1;
                B = triangles[triangle].N2;
                C = triangles[triangle].N3;

                V1 = triangles[triangle].V1;
                V2 = triangles[triangle].V2;
                V3 = triangles[triangle].V3;

                triangles[triangle].V1 = p;
                triangles[triangle].V2 = V1;
                triangles[triangle].V3 = V2;

                triangles[triangle].N1 = numtri + 2;
                triangles[triangle].N2 = A;
                triangles[triangle].N3 = numtri + 1;

                numtri++;
                triangles[numtri] = new(p, V2, V3, triangle, B, numtri + 1);

                numtri++;
                triangles[numtri] = new(p, V3, V1, numtri - 1, C, triangle); ;

                if (A != 0) stack.Push(triangle);
                if (B != 0)
                {
                    Triangle.SetNeighbor(ref triangles[B], FindAdjacentEdge(B, triangle, triangles), numtri - 1);
                    stack.Push(numtri - 1);
                }
                if (C != 0)
                {
                    Triangle.SetNeighbor(ref triangles[C], FindAdjacentEdge(C, triangle, triangles), numtri);
                    stack.Push(numtri);
                }

                while (stack.Count > 0)
                {
                    L = stack.Pop();
                    ref Triangle tL = ref triangles[L];
                    R = tL.N2;

                    int ERL = FindAdjacentEdge(R, L, triangles);
                    int ERA = Triangle.NextIndex(ERL);
                    int ERB = Triangle.NextIndex(ERA);

                    ref Triangle tR = ref triangles[R];

                    V1 = Triangle.GetVertex(tR, ERL);
                    V2 = Triangle.GetVertex(tR, ERA);
                    V3 = Triangle.GetVertex(tR, ERB);

                    if (PointInCircumcircle(vertices[V1], vertices[V2], vertices[V3], vertices[p]))
                    {
                        A = Triangle.GetNeighbor(tR, ERA);
                        B = Triangle.GetNeighbor(tR, ERB);
                        C = tL.N3;

                        tL.V3 = V3;
                        tL.N2 = A;
                        tL.N3 = R;

                        tR.V1 = p;
                        tR.V2 = V3;
                        tR.V3 = V1;
                        tR.N1 = L;
                        tR.N2 = B;
                        tR.N3 = C;

                        if (A != 0)
                        {
                            Triangle.SetNeighbor(ref triangles[A], FindAdjacentEdge(A, R, triangles), L);
                            stack.Push(L);
                        }
                        if (B != 0) stack.Push(R);
                        if (C != 0) Triangle.SetNeighbor(ref triangles[C], FindAdjacentEdge(C, L, triangles), R);
                    }
                }
            }

            int t;
            for (t = 1; t <= numtri; t++)
            {
                if (ConnectedToSuperTriangle(t, triangles, numpts))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        A = Triangle.GetNeighbor(triangles[t], i);
                        if (A != 0) Triangle.SetNeighbor(ref triangles[A], FindAdjacentEdge(A, t, triangles), 0);
                    }

                    break;
                }
            }

            int tStart = t + 1;
            int tStop = numtri;
            numtri = t - 1;

            for (t = tStart; t <= tStop; t++)
            {
                if (ConnectedToSuperTriangle(t, triangles, numpts))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        A = Triangle.GetNeighbor(triangles[t], i);
                        if (A != 0) Triangle.SetNeighbor(ref triangles[A], FindAdjacentEdge(A, t, triangles), 0);
                    }
                }
                else
                {
                    numtri++;
                    for (int i = 1; i <= 3; i++)
                    {
                        A = Triangle.GetNeighbor(triangles[t], i);
                        Triangle.SetNeighbor(ref triangles[numtri], i, A);
                        Triangle.SetVertex(ref triangles[numtri], i, Triangle.GetVertex(triangles[t], i));
                        if (A != 0) Triangle.SetNeighbor(ref triangles[A], FindAdjacentEdge(A, t, triangles), numtri);
                    }
                }
            }

            Triangle[] finalTriangles = new Triangle[numtri];
            Array.Copy(triangles, 1, finalTriangles, 0, numtri);

            int[] triangulation = new int[finalTriangles.Length * 3];

            for (int tr = 0, i = 0; tr < finalTriangles.Length; tr++, i += 3)
            {
                var tri = finalTriangles[tr];
                triangulation[i] = tri.V1 - 1;
                triangulation[i + 1] = tri.V2 - 1;
                triangulation[i + 2] = tri.V3 - 1;
            }

            return triangulation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LocateTriangle(Vector2 point, Vector2[] vertices, Triangle[] triangles, int numtri)
        {
            int triangle = numtri;

            for (int i = 1; i <= 3; i++)
            {
                Vector2 v1 = vertices[Triangle.GetVertex(triangles[triangle], i)];
                Vector2 v2 = vertices[Triangle.GetVertex(triangles[triangle], Triangle.NextIndex(i))];

                if ((v1.Y - point.Y) * (v2.X - point.X) > (v1.X - point.X) * (v2.Y - point.Y))
                {
                    triangle = Triangle.GetNeighbor(triangles[triangle], i);
                    i = 0;
                }
            }

            return triangle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindAdjacentEdge(int triangle, int adjacentTriangle, Triangle[] triangles)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (Triangle.GetNeighbor(triangles[triangle], i) == adjacentTriangle) return i;
            }

            throw new Exception("Triangles not adjacent");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInCircumcircle(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 point)
        {
            float x13 = v1.X - v3.X;
            float y13 = v1.Y - v3.Y;

            float x23 = v2.X - v3.X;
            float y23 = v2.Y - v3.Y;

            float x1p = v1.X - point.X;
            float y1p = v1.Y - point.Y;

            float x2p = v2.X - point.X;
            float y2p = v2.Y - point.Y;

            float cosA = x13 * x23 + y13 * y23;
            float cosB = x2p * x1p + y1p * y2p;

            if ((cosA >= 0f) && (cosB >= 0f)) return false;
            else if ((cosA < 0f) && (cosB < 0f)) return true;
            else
            {
                float sinA = x13 * y23 - x23 * y13;
                float sinB = x2p * y1p - x1p * y2p;

                return (sinA * cosB + sinB * cosA < 0f);
            }
        }

        public static bool ConnectedToSuperTriangle(int triangle, Triangle[] triangles, int numpts)
        {
            ref Triangle v = ref triangles[triangle];
            return v.V1 > numpts || v.V2 > numpts || v.V3 > numpts;
        }
    }
}
