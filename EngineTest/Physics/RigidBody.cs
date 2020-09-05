using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;

namespace EngineTest
{
    class RigidBody
    {
        public bool Static = false;
        private Triangle[] _poly;
        public Triangle[] polygon
        {
            get { return _poly; }
        }

        private Vector2 _pos;
        public Vector2 position
        {
            get { return _pos;}
            set
            {
                foreach (Triangle tri in polygon)
                {
                    tri.SetPosition(value);
                }
                _pos = value;
            }
        }

        private float _rot = 0f;
        public float rotation
        {
            get { return _rot; }
            set
            {
                _rot = value;
                while (_rot > (float)(2f * Math.PI)) _rot -= (float)(2f * Math.PI);
                while (_rot < 0) _rot += (float)(2f * Math.PI);
                foreach (Triangle tri in polygon)
                {
                    tri.SetRotation(_rot);
                    tri.SetPosition(position);
                }
            }
        }

        public float mass = 1f, elasticity = 1f;
        public Vector2 velocity = new Vector2();
        public float angular_velocity = 0f;
        public Vector2 center = new Vector2();

        public RigidBody(Triangle[] polygon, Vector2 position = new Vector2(), float rotation = 0f, float mass = 1f, float elasticity = 1f, bool Static = false)
        {
            _poly = polygon;
            _pos = position;
            _rot = rotation;
            this.mass = mass;
            this.Static = Static;
            this.elasticity = elasticity + 1;

            int vt = polygon.Length * 3;
            float xt = 0, yt = 0;
            foreach (Triangle tri in polygon)
            {
                foreach (Vector2 vertex in tri.vertexes)
                {
                    xt += vertex.x;
                    yt += vertex.y;
                }
            }

            center = new Vector2(xt / vt, yt / vt);

            foreach (Triangle tri in polygon)
            {
                tri.center = center;
                tri.SetRotation(_rot);
                tri.SetPosition(_pos);
            }
        }

        private Triangle.TriangleResults CheckForCollision(RigidBody body)
        {
            foreach (Triangle thisTri in _poly)
            {
                foreach (Triangle tri in body.polygon)
                {
                    Triangle.TriangleResults results = tri.TestForCollsion(velocity, angular_velocity, thisTri);

                    if (!results.contact) continue;
                    if (!Static)
                        AdjustForCollision(results.depth, body, results.side.Normal(), results.point);
                    else
                        AdjustForCollision(results.depth, this, results.side.Normal(), results.point);
                }
            }
            return new Triangle.TriangleResults { contact = false };
        }

        public void Update(RigidBody[] bodies, float dt)
        {
            foreach (RigidBody body in bodies)
            {
                if (body == this) continue;
                CheckForCollision(body);
            }

            if (Static) return;
            position += velocity * dt;
            rotation += angular_velocity * dt * elasticity;
        }

        public void AdjustForCollision(float depth, RigidBody body, Vector2 normal, Vector2 point)
        {
            if (velocity.Magnitude() > 0) position += -depth * velocity.Normalized();
            //else body.position += Vector2.Vector2FromMag(body.velocity.Angle(), -depth);

            if (!body.Static)
            {
                Vector2 resultv = new Vector2(normal.x * ((normal.x * velocity.x) + (normal.y * velocity.y)), normal.y * ((normal.x * velocity.x) + (normal.y * velocity.y)));
                velocity = new Vector2(velocity.x - (elasticity) * resultv.x, velocity.y - (elasticity) * resultv.y);
                //resultv = new Vector2(normal.x * ((normal.x * body.velocity.x) + (normal.y * body.velocity.y)), normal.y * ((normal.x * body.velocity.x) + (normal.y * body.velocity.y)));
                //body.velocity = new Vector2(body.velocity.x - (body.elasticity) * resultv.x, body.velocity.y - (body.elasticity) * resultv.y);
            }
            else
            {
                Vector2 newv = new Vector2(normal.x * ((normal.x * velocity.x) + (normal.y * velocity.y)), normal.y * ((normal.x * velocity.x) + (normal.y * velocity.y)));
                velocity -= new Vector2((elasticity) * newv.x, (elasticity) * newv.y);
                //angular_velocity +=  Vector2.DotProduct((position - point).Normal(), normal) * 0.008f;
                Debug.WriteLine(Vector2.DotProduct((position - point).Normal(), normal));
            }
        }
    }
}
