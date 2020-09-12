using System;
using System.Diagnostics;
using Windows.ApplicationModel.Chat;

namespace EngineTest
{
    class Triangle
    {
        private Vector2[] _vertexes = new Vector2[3];

        private Vector2[] _vert = new Vector2[3];
        public Vector2[] vertexes
        {
            get { return _vert; }
        }

        public Vector2 position = new Vector2();
        public Vector2 center = new Vector2();

        public Triangle(Vector2[] vertexes)
        {
            _vertexes = vertexes;
            Array.Copy(vertexes, _vert, vertexes.Length);
        }

        public void SetRotation(float angle)
        {
            float x = _vertexes[0].x, y = _vertexes[0].y, r = center.DistanceFrom(_vertexes[0]);
            _vert[0] = new Vector2(center.x - (r * (float)Math.Cos(angle + (float)Math.Atan2(center.y - y, center.x - x))), center.y - (r * (float)Math.Sin(angle + (float)Math.Atan2(center.y - y, center.x - x))));
            x = _vertexes[1].x;
            y = _vertexes[1].y;
            r = center.DistanceFrom(_vertexes[1]);
            _vert[1] = new Vector2(center.x - (r * (float)Math.Cos(angle + (float)Math.Atan2(center.y - y, center.x - x))), center.y - (r * (float)Math.Sin(angle + (float)Math.Atan2(center.y - y, center.x - x))));
            x = _vertexes[2].x;
            y = _vertexes[2].y;
            r = center.DistanceFrom(_vertexes[2]);
            _vert[2] = new Vector2(center.x - (r * (float)Math.Cos(angle + (float)Math.Atan2(center.y - y, center.x - x))), center.y - (r * (float)Math.Sin(angle + (float)Math.Atan2(center.y - y, center.x - x))));
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position - center;
        }

        public struct TriangleResults
        {
            public bool contact, sameSide;
            public float depth;
            public Vector2 p1, p2;
            public Vector2 normal;
            public Vector2 point;
        };

        public TriangleResults[] TestForCollsion(Triangle b)
        {
            int i = 0;
            TriangleResults[] contacts = new TriangleResults[6];
            foreach (Vector2 pb in b.vertexes)
            {
                contacts[i] = PointOfCollision(pb + b.position);
                if (i != 0 && contacts[i - 1].contact && contacts[i].contact)
                {
                    contacts[i].contact = false;
                    contacts[i - 1].p1 = contacts[i].point;
                    contacts[i - 1].p2 = contacts[i - 1].point;
                    contacts[i - 1].point += contacts[i].point;
                    contacts[i - 1].point /= 2;
                    contacts[i - 1].sameSide = true;
                }
                i++;
            }

            foreach (Vector2 pa in vertexes)
            {
                TriangleResults results = b.PointOfCollision(pa + position);
                results.normal *= -1;
                contacts[i] = results;
                if (i != 4 && contacts[i - 1].contact && contacts[i].contact)
                {
                    contacts[i].contact = false;
                    contacts[i - 1].p1 = contacts[i].point;
                    contacts[i - 1].p2 = contacts[i - 1].point;
                    contacts[i - 1].point += contacts[i].point;
                    contacts[i - 1].point /= 2;
                    contacts[i - 1].sameSide = true;
                }
                i++;
            }

            return contacts;
        }

        public TriangleResults PointOfCollision(Vector2 vertex)
        {
            Vector2 A, B, C, v0, v1, v2;

            A = vertexes[0] + position;
            B = vertexes[1] + position;
            C = vertexes[2] + position;

            v0 = C - A;
            v1 = B - A;
            v2 = vertex - A;

            float d00, d01, d02, d11, d12;

            d00 = Vector2.DotProduct(v0, v0);
            d01 = Vector2.DotProduct(v0, v1);
            d02 = Vector2.DotProduct(v0, v2);
            d11 = Vector2.DotProduct(v1, v1);
            d12 = Vector2.DotProduct(v1, v2);

            float denom, u, v, w;
            denom = d00 * d11 - d01 * d01;
            u = (d11 * d02 - d01 * d12) / denom;
            v = (d00 * d12 - d01 * d02) / denom;
            w = 1 - u - v;

            TriangleResults results = new TriangleResults { contact = false };
            if ((u > 0) && (v > 0) && (u + v < 1)) results.contact = true;
            else return results;

            int p1, p2;
            if (u < v && u < w) { p1 = 0; p2 = 1; }
            else if (v < u && v < w) { p1 = 0; p2 = 2; }
            else { p1 = 1; p2 = 2; }

            Vector2 vp1, vp2;
            vp1 = vertexes[p1] + position;
            vp2 = vertexes[p2] + position;
            results.normal = (vp2 - vp1).Normal();

            if (PointOfIntersection(vp1, vp2, vertex, vertex - results.normal, out results.point)) results.depth = results.point.DistanceFrom(vertex);
            else results.depth = 0f;
            results.normal = (vp2 - vp1).Normal(position + center, results.point);

            return results;
        }
        private bool PointOfIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out Vector2 point)
        {
            point = new Vector2();

            if (Math.Abs(((B.y - A.y) / (B.x - A.x)) - ((D.y - C.y) / (D.x - C.x))) < 0.001f) return false;

            float x, y, u, v, denom;
            u = A.x * B.y - A.y * B.x;
            v = C.x * D.y - C.y * D.x;
            denom = (A.x - B.x) * (C.y - D.y) - (A.y - B.y) * (C.x - D.x);
            x = (u * (C.x - D.x) - v * (A.x - B.x)) / denom;
            y = (u * (C.y - D.y) - v * (A.y - B.y)) / denom;

            point = new Vector2(x, y);

            return true;
        }

        public System.Numerics.Vector2[] GetVertexArray(Vector2 origin)
        {
            origin += position;
            return new System.Numerics.Vector2[] { vertexes[0] + origin, vertexes[1] + origin, vertexes[2] + origin };
        }
    }
}
