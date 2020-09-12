using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Enumeration.Pnp;
using Windows.Devices.SmartCards;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Composition;
using Windows.UI.ViewManagement.Core;
using Windows.UI.Xaml.Media;

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

        private bool CheckForCollision(RigidBody body, float dt, bool applyForce)
        {
            List<Vector2> checkedPoints = new List<Vector2>();
            List<Triangle.TriangleResults> collisions = new List<Triangle.TriangleResults>();
            foreach (Triangle thisTri in _poly)
            {
                foreach (Triangle tri in body.polygon)
                {
                    Triangle.TriangleResults[] results = tri.TestForCollsion(thisTri);
                    foreach (Triangle.TriangleResults result in results)
                    {
                        if (!result.contact || checkedPoints.Contains(result.point)) continue;
                        if (result.sameSide)
                        {
                            checkedPoints.Add(result.p1);
                            checkedPoints.Add(result.p2);
                            if (applyForce) ApplyCollision(body, result.normal, result.point, dt);
                            displacement += -result.depth * result.normal;
                            return true;
                        }
                        else checkedPoints.Add(result.point);

                        collisions.Add(result);
                    }
                }
            }

            foreach (Triangle.TriangleResults result in collisions)
            {
                if (applyForce) ApplyCollision(body, result.normal, result.point, dt);
                displacement += -result.depth * result.normal;
            }

            return collisions.Count > 0;
        }

        public bool ResolveCollisions(RigidBody[] bodies, float dt, bool applyForce)
        {
            bool collision = false;
            foreach (RigidBody body in bodies)
            {
                if (body == this) continue;
                bool result = CheckForCollision(body, dt, applyForce);
                collision = (result) ? true : collision;
            }
            position += displacement * 1.3f;
            displacement = new Vector2();

            return collision;
        }

        public void Update(float dt)
        {
            if (Static) return;

            position += velocity * dt;
            rotation += angular_velocity * dt;
            angular_velocity *= 0.999f;
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
                Vector2 r = (point - position);
                Vector2 f1 = (r.Normal(position, point, false) * angular_velocity * elasticity * dt) / mass;
                Vector2 friction = Vector2.DotProduct(normal.Normal(), velocity + f1) * normal.Normal();
                Vector2 newv = Vector2.DotProduct(normal, velocity + f1) * normal;
                velocity -= elasticity * newv;
                velocity -= 0.01f * friction;
                Vector2 f2 = (elasticity * newv * dt) / mass;
                angular_velocity -= Vector2.CrossProduct(r, f2) * dt;
            }
        }

        public void ApplyCollision(RigidBody body, Vector2 normal, Vector2 point, float dt)
        {
            Vector2 r = (point - position);
            Vector2 force = mass * (Vector2.DotProduct(normal, (velocity + (angular_velocity * r.Normalized())) / dt) * normal);
            force *= elasticity;
            ApplyForce(force, point, dt);
            if (!body.Static) body.ApplyForce(-force, point, dt);
        }

        public void ApplyForce(Vector2 force, Vector2 point, float dt)
        {
            Vector2 r = (point - position);
            angular_velocity += Vector2.CrossProduct(r.Normalized(), -force) / (mass * r.Magnitude()) * dt;
            velocity += (-force / mass) * dt;
        }
    }
}
