using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TIE_Fighter_Forever.GameComponents.ParticleSystem
{
    /// <summary>
    /// Vertex deklaráció robbanások részecskéihez
    /// </summary>
    struct ExplosionVertex : IVertexType
    {
        public Vector3 position;
        public Vector4 texAndData;
        public Vector4 deltaMoveAndRand;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
