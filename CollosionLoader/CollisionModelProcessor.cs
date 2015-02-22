using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;
using TypeReaders.CollisionPipelineRuntimeHelper;

namespace CollisionLoaderPipeline
{
    /// <summary>
    /// Model processzor fõhajókhoz és ûrállomásokhoz, normal mapping támogatással
    /// </summary>
    [ContentProcessor]
    public class CollisionModelProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input,
            ContentProcessorContext context)
        {
            List<Triangle[]> modelTriangles = new List<Triangle[]>();
            modelTriangles = AddModelMeshTriangleArrayToList(input, modelTriangles);

            ModelContent usualModel = base.Process(input, context);

            //List<Triangle> modelTrianglesInList = new List<Triangle>();

            int i = 0;
            foreach (ModelMeshContent mesh in usualModel.Meshes)
            {
                List<Triangle> modelMeshTriangles = new List<Triangle>();
                foreach (ModelMeshPartContent part in mesh.MeshParts)
                {
                    //modelTrianglesInList.AddRange(modelTriangles[i]);
                    modelMeshTriangles.AddRange(modelTriangles[i]);
                    ++i;
                }
                Triangle[] ts = modelMeshTriangles.ToArray();

                // Bounding Box kialakítása a COctree eslõ szintjének paraméterezéséhez!
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float minZ = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float maxZ = float.MinValue;
                foreach (Triangle t in ts)
                {
                    // minX
                    if (t.P0.X < minX)
                        minX = t.P0.X;
                    if (t.P1.X < minX)
                        minX = t.P1.X;
                    if (t.P2.X < minX)
                        minX = t.P2.X;

                    // maxX
                    if (t.P0.X > maxX)
                        maxX = t.P0.X;
                    if (t.P1.X > maxX)
                        maxX = t.P1.X;
                    if (t.P2.X > maxX)
                        maxX = t.P2.X;

                    // minY
                    if (t.P0.Y < minY)
                        minY = t.P0.Y;
                    if (t.P1.Y < minY)
                        minY = t.P1.Y;
                    if (t.P2.Y < minY)
                        minY = t.P2.Y;

                    // maxY
                    if (t.P0.Y > maxY)
                        maxY = t.P0.Y;
                    if (t.P1.Y > maxY)
                        maxY = t.P1.Y;
                    if (t.P2.Y > maxY)
                        maxY = t.P2.Y;

                    // minZ
                    if (t.P0.Z < minZ)
                        minZ = t.P0.Z;
                    if (t.P1.Z < minZ)
                        minZ = t.P1.Z;
                    if (t.P2.Z < minZ)
                        minZ = t.P2.Z;

                    // maxZ
                    if (t.P0.Z > maxZ)
                        maxZ = t.P0.Z;
                    if (t.P1.Z > maxZ)
                        maxZ = t.P1.Z;
                    if (t.P2.Z > maxZ)
                        maxZ = t.P2.Z;
                }
                float eps = 0.1f;   // Számítási hibák elkerülése végett!
                BoundingBox bigBox = new BoundingBox(new Vector3(minX - eps, minY - eps, minZ - eps), new Vector3(maxX + eps, maxY + eps, maxZ + eps));
                COctree ct = new COctree(ts, bigBox, 5, 0, 25);
                mesh.Tag = ct;
                //mesh.Tag = modelMeshTriangles.ToArray();
            }

            //usualModel.Tag = modelTrianglesInList.ToArray();

            return usualModel;
        }

        private List<Triangle[]> AddModelMeshTriangleArrayToList(NodeContent node, List<Triangle[]> triangleList)
        {
            foreach (NodeContent child in node.Children)
                triangleList = AddModelMeshTriangleArrayToList(child, triangleList);

            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                foreach (GeometryContent geo in mesh.Geometry)
                {
                    int triangles = geo.Indices.Count / 3;
                    List<Triangle> nodeTriangles = new List<Triangle>();

                    for (int currentTriangle = 0; currentTriangle < triangles; currentTriangle++)
                    {
                        int index0 = geo.Indices[currentTriangle * 3 + 0];
                        int index1 = geo.Indices[currentTriangle * 3 + 1];
                        int index2 = geo.Indices[currentTriangle * 3 + 2];

                        Vector3 v0 = geo.Vertices.Positions[index0];
                        Vector3 v1 = geo.Vertices.Positions[index1];
                        Vector3 v2 = geo.Vertices.Positions[index2];

                        Triangle newTriangle = new Triangle(v0, v1, v2);
                        nodeTriangles.Add(newTriangle);
                    }
                    triangleList.Add(nodeTriangles.ToArray());
                }
            }

            return triangleList;
        }
    }

    [ContentTypeWriter]
    public class COctreeTypeWriter : ContentTypeWriter<COctree>
    {
        protected override void Write(ContentWriter output, COctree value)
        {
            output.WriteObject<bool>(value.isSplit);
            output.WriteObject<Triangle[]>(value.myTriangles);
            output.WriteObject<COctree[]>(value.childs);
            output.WriteObject<BoundingBox>(value.box);
            output.WriteObject<int>(value.triNum);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(COctreeTypeReader).AssemblyQualifiedName;
        }
    }

    [ContentTypeWriter]
    public class TriangleTypeWriter : ContentTypeWriter<Triangle>
    {
        protected override void Write(ContentWriter output, Triangle value)
        {
            output.WriteObject<Vector3>(value.P0);
            output.WriteObject<Vector3>(value.P1);
            output.WriteObject<Vector3>(value.P2);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TriangleTypeReader).AssemblyQualifiedName;
        }
    }
}