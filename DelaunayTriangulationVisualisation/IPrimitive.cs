using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace DelaunayTriangulationVisualisation
{
    public interface IPrimitive
    {
        VertexPositionColor[] Vertices { get; set; }
        int[] Indices { get; set; }
    }
}
