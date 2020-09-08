using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Composition;
using Windows.UI.ViewManagement.Core;

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
        private Vector2 displacement = new Vector2();

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

        private Triangle.TriangleResults CheckForCollision(RigidBody body, float dt)
        {
            foreach (Triangle thisTri in _poly)
            {
                bool contacted = false;
                foreach (Triangle tri in body.polygon)
                {
                    Triangle.TriangleResults[] results = tri.TestForCollsion(thisTri);
                    foreach (Triangle.TriangleResults result in results)
                    {
                        if (!result.contact) continue;
                        AdjustForCollision(body, result.normal, result.point, dt);

                        if (!contacted)
                        {
                            displacement += -result.depth * result.normal;
                            contacted = true;
                        }
                    }
                }
            }
            return new Triangle.TriangleResults { contact = false };
        }

        public void Update(RigidBody[] bodies, float dt)
        {
            if (Static) return;

            foreach (RigidBody body in bodies)
            {
                if (body == this) continue;
                CheckForCollision(body, dt);
                position += displacement;
                displacement = new Vector2();
            }

            position += velocity * dt;
            rotation += angular_velocity * dt * 0.1f;
        }

        public void AdjustForCollision(RigidBody body, Vector2 normal, Vector2 point, float dt)
        {
            if (!body.Static)
            {
                Vector2 dv = velocity;
                Vector2 dvb = body.velocity;

                Vector2 nv = new Vector2(((mass - body.mass) / (mass + body.mass)) * velocity.x + ((2 * mass) / (mass + body.mass)) * body.velocity.x, ((mass - body.mass) / (mass + body.mass)) * velocity.y + ((2 * mass) / (mass + body.mass)) * body.velocity.y);
                Vector2 nvb = new Vector2(((2 * body.mass) / (mass + body.mass)) * velocity.x - ((mass - body.mass) / (mass + body.mass)) * body.velocity.x, ((2 * body.mass) / (mass + body.mass)) * velocity.y - ((mass - body.mass) / (mass + body.mass)) * body.velocity.y);

                Vector2 rv = new Vector2(normal.x * ((normal.x * nv.x) + (normal.y * nv.y)), normal.y * ((normal.x * nv.x) + (normal.y * nv.y)));
                Vector2 rvb = new Vector2(-normal.x * ((-normal.x * nvb.x) + (-normal.y * nvb.y)), -normal.y * ((-normal.x * nvb.x) + (-normal.y * nvb.y)));

                velocity = elasticity * rv;
                body.velocity = body.elasticity * rvb;

                Vector2 r = (point - position);
                dv -= velocity;
                Vector2 f = dv.Magnitude() * normal;
                angular_velocity -= ((r.x * f.y - r.y * f.x) / mass) * dt;

                Vector2 rb = (point - body.position);
                dvb -= body.velocity;
                Vector2 fb = dvb.Magnitude() * -normal;
                body.angular_velocity -= ((rb.x * fb.y - rb.y * fb.x) / body.mass) * dt;
            }
            else
            {
                Vector2 dv = velocity;
                Vector2 newv = new Vector2(normal.x * ((normal.x * velocity.x) + (normal.y * velocity.y)), normal.y * ((normal.x * velocity.x) + (normal.y * velocity.y)));
                velocity -= elasticity * newv; 
                Vector2 r = (point - position);
                dv -= velocity;
                Vector2 f = dv.Magnitude() * normal;
                angular_velocity -= (r.x * f.y - r.y * f.x) * dt;
            }
        }
    }
}
