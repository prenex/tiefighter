using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace TypeReaders.CollisionPipelineRuntimeHelper
{
    public class COctree
    {
        public bool isSplit;
        public Triangle[] myTriangles;
        public COctree[] childs;
        public BoundingBox box;
        public int triNum;

        /// <summary>
        /// Létrehoz egy üres COctree-t
        /// </summary>
        public COctree() { }

        /// <summary>
        /// Létrehoz egy adott méretű és paraméterezésű COctree-t(Collision Octree) és feltölti a megfelelő háromszögekkel
        /// </summary>
        /// <param name="ts">Háromszögek, melyekből válogathat a sajátjai közé a COctree</param>
        /// <param name="cullingBox">Doboz a háromszögek vágására: ami nem része a doboznak, nem kerül be a COctree-be</param>
        /// <param name="maxLevel">A fa maximális magassága</param>
        /// <param name="level">A fa kezdőszintje(kissebb mint maxLevel)</param>
        /// <param name="maxTrianglesInBoxes">Ha ennél több háromszög van egy csúcspontban(dobozban) és még nem értük el a maximális szintet, akkor vágás lesz!</param>
        public COctree(Triangle[] ts, BoundingBox cullingBox, int maxLevel, int level, int maxTrianglesInBoxes)
        {
            // Alapbeállítások
            isSplit = false;
            box = cullingBox;
            triNum = 0;

            // Tartalmazó háromszögek meghatározása és megszámolása
            List<Triangle> tlist = new List<Triangle>();
            foreach(Triangle t in ts)
            {
                if(t.collides(box))
                {
                    tlist.Add(t);
                    ++triNum;
                }
            }
            myTriangles = tlist.ToArray();

            // Ha kell és lehet, akkor háromszögek szétszórása fabővítéssel gyerekekbe
            if ((triNum > maxTrianglesInBoxes) && (level < maxLevel))
            {
                // 8-fele particionálunk, 8 gyerek kell
                childs = new COctree[8];
                // szükségünk van a jelenlegi doboz középpontjának kiszámítására!
                Vector3 middlePoint = (box.Min + box.Max) / 2;

                // Alsó gyermekdobozok
                // bal-alsó-első child
                childs[0] = new COctree(myTriangles, new BoundingBox(box.Min, middlePoint), maxLevel, level + 1, maxTrianglesInBoxes);
                // bal-alsó-hátsó child
                childs[1] = new COctree(myTriangles, new BoundingBox(new Vector3(box.Min.X, box.Min.Y, middlePoint.Z), new Vector3(middlePoint.X, middlePoint.Y, box.Max.Z)), maxLevel, level + 1, maxTrianglesInBoxes);
                // jobb-alsó-hátsó child
                childs[2] = new COctree(myTriangles, new BoundingBox(new Vector3(middlePoint.X, box.Min.Y, middlePoint.Z), new Vector3(box.Max.X, middlePoint.Y, box.Max.Z)), maxLevel, level + 1, maxTrianglesInBoxes);
                // jobb-alsó-első child
                childs[3] = new COctree(myTriangles, new BoundingBox(new Vector3(middlePoint.X, box.Min.Y, box.Min.Z), new Vector3(box.Max.X, middlePoint.Y, middlePoint.Z)), maxLevel, level + 1, maxTrianglesInBoxes);

                // felső gyermekdobozok
                // bal-felső-első child
                childs[4] = new COctree(myTriangles, new BoundingBox(new Vector3(box.Min.X, middlePoint.Y, box.Min.Z),new Vector3(middlePoint.X, box.Max.Y, middlePoint.Z)), maxLevel, level + 1, maxTrianglesInBoxes);
                // bal-felső-hátsó child
                childs[5] = new COctree(myTriangles, new BoundingBox(new Vector3(box.Min.X, middlePoint.Y, middlePoint.Z), new Vector3(middlePoint.X, box.Max.Y, box.Max.Z)), maxLevel, level + 1, maxTrianglesInBoxes);
                // jobb-felső-hátsó child
                childs[6] = new COctree(myTriangles, new BoundingBox(middlePoint, box.Max), maxLevel, level + 1, maxTrianglesInBoxes);
                // jobb-felső-első child
                childs[7] = new COctree(myTriangles, new BoundingBox(new Vector3(middlePoint.X, middlePoint.Y, box.Min.Z), new Vector3(box.Max.X, box.Max.Y, middlePoint.Z)), maxLevel, level + 1, maxTrianglesInBoxes);

                isSplit = true;
                myTriangles = null;
                //triNum = 0;
            }
        }

        /// <summary>
        /// Megvizsgálja, hogy az adott irányított szakasz ütközik-e valamelyik
        /// háromszöggel az octree-ben.
        /// </summary>
        /// <param name="ray">Az irányított szakasz sugárral van megadva, a kezdőpont az egyik pont, kezdőpont+irányvektor a másik!</param>
        /// <returns>Igaz, ha történt ütközés</returns>
        public bool collisionTest(Ray ray, BoundingBox rayBox)
        {
            if (!this.box.Intersects(rayBox))
            {
                return false;
            }
            else
            {
                bool collision = false;

                if (this.isSplit)
                {
                    foreach (COctree child in childs)
                    {
                        if(child.box.Intersects(rayBox))
                            collision = collision || child.collisionTest(ray, rayBox);
                    }
                    return collision;
                }
                else
                {
                    foreach (Triangle tri in myTriangles)
                    {
                        Plane trianglePlane = new Plane(tri.P0, tri.P1, tri.P2);
                        float? d = ray.Intersects(trianglePlane);

                        float distanceOnRay;
                        Vector3 intersectionPoint;
                        if (d == null)
                        {
                            distanceOnRay = float.NaN;
                            intersectionPoint = ray.Position + distanceOnRay * ray.Direction;
                        }
                        else
                        {
                            distanceOnRay = (float)d;
                            intersectionPoint = ray.Position + distanceOnRay * ray.Direction;
                            Vector3 v1 = ray.Position - intersectionPoint;
                            Vector3 v2 = (ray.Position + ray.Direction) - intersectionPoint;
                            bool signSameSoNotCollide = (CompareSigns(v1, v2));
                            if (signSameSoNotCollide)
                            {
                                distanceOnRay = float.NaN;
                                intersectionPoint = ray.Position + distanceOnRay * ray.Direction;
                            }
                        }
                        //float distanceOnRay2 = RayPlaneIntersection(invRay, trianglePlane);


                        if (PointInsideTriangle(tri.P0, tri.P1, tri.P2, intersectionPoint))
                            collision = true;
                    }
                }
                return collision;
            }
        }

        private bool PointInsideTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 point)
        {
            if (float.IsNaN(point.X)) return false;

            Vector3 A0 = point - p0;
            Vector3 B0 = p1 - p0;
            Vector3 cross0 = Vector3.Cross(A0, B0);

            Vector3 A1 = point - p1;
            Vector3 B1 = p2 - p1;
            Vector3 cross1 = Vector3.Cross(A1, B1);

            Vector3 A2 = point - p2;
            Vector3 B2 = p0 - p2;
            Vector3 cross2 = Vector3.Cross(A2, B2);

            if (CompareSigns(cross0, cross1) && CompareSigns(cross0, cross2))
                return true;
            else
                return false;
        }

        private bool CompareSigns(Vector3 first, Vector3 second)
        {
            if (Vector3.Dot(first, second) > 0)
                return true;
            else
                return false;
        }
    }

    public class COctreeTypeReader : ContentTypeReader<COctree>
    {
        protected override COctree Read(ContentReader input, COctree existingInstance)
        {

            bool isSplit = input.ReadObject<bool>();
            Triangle[] myTriangles = input.ReadObject<Triangle[]>();
            COctree[] childs = input.ReadObject<COctree[]>();
            BoundingBox box = input.ReadObject<BoundingBox>();
            int triNum = input.ReadObject<int>();

            COctree ct = new COctree();
            ct.isSplit = isSplit;
            ct.myTriangles = myTriangles;
            ct.childs = childs;
            ct.box = box;
            ct.triNum = triNum;

            return ct;
        }
    }

    /// <summary>
    /// Egyszerű, egy háromszög csúcskoordinátáit tartalmazó osztály
    /// Azért került megvalósításra, hogy a betöltött nagyhajó modellek
    /// modelmesh objektumainak háromszögeit leírhassuk!
    /// </summary>
    public class Triangle
    {
        private Vector3[] points;
        private BoundingBox boundingBox;
        private BoundingBox proximityBox;

        public Triangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // Alapadat inicializáció
            points = new Vector3[3];
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;

            // A háromszög bounding box előállítása
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;
            for (int i = 0; i < 3; ++i)
            {
                if (points[i].X < minX)
                    minX = points[i].X;
                if (points[i].Y < minY)
                    minY = points[i].Y;
                if (points[i].Z < minZ)
                    minZ = points[i].Z;

                if (points[i].X > maxX)
                    maxX = points[i].X;
                if (points[i].Y > maxY)
                    maxY = points[i].Y;
                if (points[i].Z > maxZ)
                    maxZ = points[i].Z;
            }
            boundingBox = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            proximityBox = new BoundingBox(new Vector3(minX - 10.0f, minY - 10.0f, minZ - 10.0f), new Vector3(maxX + 10.0f, maxY + 10.0f, maxZ + 10.0f));
        }

        public Vector3[] Points { get { return points; } }
        public Vector3 P0 { get { return points[0]; } }
        public Vector3 P1 { get { return points[1]; } }
        public Vector3 P2 { get { return points[2]; } }
        /// <summary>
        /// Visszaadja a háromszög befoglaló téglatestét
        /// </summary>
        /// <returns></returns>
        public BoundingBox getBoundingBox()
        {
            return boundingBox;
        }
        /// <summary>
        /// Visszaadja a háromszög közelségi téglatestét
        /// A közelségi téglatest, minden irányban 10.0f egységgel nagyobb a
        /// befoglaló dobozhoz képest!
        /// </summary>
        /// <returns></returns>
        public BoundingBox getProximityBox()
        {
            return proximityBox;
        }

        /// <summary>
        /// Megadja, hogy a háromszög - mint térbeli felület, rendelkezik-e
        /// metszettel a megadott bounding boxban(azt mint kitöltött tér tekintve). 
        /// Azaz megadja, hogy a két térbeli objektum metszi-e egymást, ütköznek-e.
        /// Megj.: A módszeren lehetne gyorsítani, nem ajánlott realtime használni, 
        /// csak átalakításokkal, vagy ritkán!
        /// </summary>
        /// <param name="box">Befoglaló doboz a teszthez</param>
        /// <returns>Van-e ütközés</returns>
        public bool collides(BoundingBox box)
        {
            // X koordinátatengely menti tesztelés
            if (((box.Min.X > this.boundingBox.Max.X) || (this.boundingBox.Min.X > box.Max.X)))
                return false;

            // Y koordinátatengely menti tesztelés
            if (((box.Min.Y > this.boundingBox.Max.Y) || (this.boundingBox.Min.Y > box.Max.Y)))
                return false;

            // Z koordinátatengely menti tesztelés
            if (((box.Min.Z > this.boundingBox.Max.Z) || (this.boundingBox.Min.Z > box.Max.Z)))
                return false;

            // Doboz vertexeinek előállítása
            Vector3[] boxVertices = new Vector3[8];
            boxVertices[0] = box.Min;
            boxVertices[1] = new Vector3(box.Min.X, box.Min.Y, box.Max.Z);
            boxVertices[2] = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
            boxVertices[3] = new Vector3(box.Max.X, box.Min.Y, box.Min.Z);
            boxVertices[4] = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
            boxVertices[5] = new Vector3(box.Min.X, box.Max.Y, box.Max.Z);
            boxVertices[6] = box.Max;
            boxVertices[7] = new Vector3(box.Max.X, box.Max.Y, box.Min.Z);

            // Háromszög normálvektorának koordinátatengelye mentén történő tesztelés
            float[] boxVertN = projectToAxis(Vector3.Normalize(Vector3.Cross(this.P1 - this.P0, this.P2 - this.P0)), boxVertices);
            float[] triVertN = projectToAxis(Vector3.Normalize(Vector3.Cross(this.P1 - this.P0, this.P2 - this.P0)), this.points);
            if (!oneDimensionalCollision(boxVertN, triVertN))
                return false;

            // Ellenőrzés az élekhez tartozó 9 tengelyre(SAT alapján ezek még kellenek!)
            Vector3 v, vtemp;
            float[] projectedBoxVertices;
            float[] projectedTriVertices;
            // Első háromszögtengely
            // tengely = (X tengely 'direktszorzás' háromszög első tengelye)
            vtemp = Vector3.Cross(this.P1 - this.P0, Vector3.UnitX);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Y tengely 'direktszorzás' háromszög első tengelye)
            vtemp = Vector3.Cross(this.P1 - this.P0, Vector3.UnitY);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Z tengely 'direktszorzás' háromszög első tengelye)
            vtemp = Vector3.Cross(this.P1 - this.P0, Vector3.UnitZ);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }

            // Második háromszögtengely
            // tengely = (X tengely 'direktszorzás' háromszög 2. tengelye)
            vtemp = Vector3.Cross(this.P2 - this.P1, Vector3.UnitX);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Y tengely 'direktszorzás' háromszög 2. tengelye)
            vtemp = Vector3.Cross(this.P2 - this.P1, Vector3.UnitY);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Z tengely 'direktszorzás' háromszög 2. tengelye)
            vtemp = Vector3.Cross(this.P2 - this.P1, Vector3.UnitZ);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }

            // Harmadik háromszögtengely
            // tengely = (X tengely 'direktszorzás' háromszög 3. tengelye)
            vtemp = Vector3.Cross(this.P0 - this.P2, Vector3.UnitX);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Y tengely 'direktszorzás' háromszög 3. tengelye)
            vtemp = Vector3.Cross(this.P0 - this.P2, Vector3.UnitY);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }
            // tengely = (Z tengely 'direktszorzás' háromszög 3. tengelye)
            vtemp = Vector3.Cross(this.P0 - this.P2, Vector3.UnitZ);
            if (vtemp != Vector3.Zero)
            {
                v = Vector3.Normalize(vtemp);
                projectedBoxVertices = projectToAxis(v, boxVertices);
                projectedTriVertices = projectToAxis(v, this.points);
                if (!oneDimensionalCollision(projectedBoxVertices, projectedTriVertices))
                    return false;
            }

            // Ha semmi se fogott ki a háromszögön, akkor:
            return true;
        }

        /// <summary>
        /// Adott bemeneti vertexkoordinátákból előállítja a megadott tengelyre
        /// vetített koordinátákat
        /// </summary>
        /// <param name="axis">Vetítési tengely irányvektora</param>
        /// <param name="vertices">Bemeneti háromdimenziós koordinták</param>
        /// <returns>Kimeneti, a tengelyre vetített egydimenziós koordináták</returns>
        public float[] projectToAxis(Vector3 axis, Vector3[] vertices)
        {
            float[] projectedPoints = new float[vertices.Length];

            for (int i = 0; i < vertices.Length; ++i)
            {
                // Az egységvektorral történő skalárszorzat megadja a másik vektor, 
                // az egységvektor által meghatározott és irányított tengelyére
                // vett vetítésének a koordinátáit
                projectedPoints[i] = Vector3.Dot(axis, vertices[i]);
            }

            return projectedPoints;
        }

        /// <summary>
        /// Két darab, koordinátahalmazukkal definiált egydimenziós objektumról 
        /// eldönti, hogy ütköznek(átfednek)-e.
        /// </summary>
        /// <param name="object1">Egyik objektum</param>
        /// <param name="object2">Másik Objektum</param>
        /// <returns>Van-e átfedés</returns>
        public bool oneDimensionalCollision(float[] object1, float[] object2)
        {
            // Egydimenziós objektumok szélső pontjainak a meghatározása
            float minObj1 = float.MaxValue;
            float minObj2 = float.MaxValue;
            float maxObj1 = float.MinValue;
            float maxObj2 = float.MinValue;
            foreach (float f in object1)
            {
                if (f < minObj1) minObj1 = f;
                if (f > maxObj1) maxObj1 = f;
            }

            foreach (float f in object2)
            {
                if (f < minObj2) minObj2 = f;
                if (f > maxObj2) maxObj2 = f;
            }

            // A szélső pontok összehasonlításával ütközés(átfedés) ellenőrzése

            return !((maxObj1 < minObj2) || (maxObj2 < minObj1));
        }
    }

    /// <summary>
    /// Custom typereader háromszögek beolvasására
    /// Az osztály megvalósít egy saját típusbeolvasót, ami a content pipelineból
    /// kikerülő modellek bináris reprezentációjából olvas.
    /// Ez a betöltő, a MotherShipEffect1ContentPipeline miatt került
    /// megvalósításra, annak ütközésvizsgálati kiterejesztése során
    /// </summary>
    public class TriangleTypeReader : ContentTypeReader<Triangle>
    {
        protected override Triangle Read(ContentReader input, Triangle existingInstance)
        {
            Vector3 p0 = input.ReadObject<Vector3>();
            Vector3 p1 = input.ReadObject<Vector3>();
            Vector3 p2 = input.ReadObject<Vector3>();

            Triangle newTriangle = new Triangle(p0, p1, p2);

            return newTriangle;
        }
    }
}
