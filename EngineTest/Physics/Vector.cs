using System;
using System.Diagnostics;

namespace EngineTest
{
    public struct Vector2
    {
        public float x;

        public float y;

        public Vector2(float x = 0.0f, float y = 0.0f)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator System.Numerics.Vector2(Vector2 v)
        {
            return new System.Numerics.Vector2(v.x, v.y);
        }

        public bool IsEmpty()
        {
            return (int)x == 0 || (int)y == 0;
        }

        public float DistanceFrom(Vector2 pointB)
        {
            double d1 = x - pointB.x;
            double d2 = y - pointB.y;

            float dist = (float)Math.Sqrt(d1 * d1 + d2 * d2);
            return dist;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            var vec2 = new Vector2
            {
                x = a.x - b.x,
                y = a.y - b.y,
            };
            return vec2;
        }

        public static Vector2 operator -(Vector2 a)
        {
            var vec2 = new Vector2
            {
                x = -1 * a.x,
                y = -1 * a.y,
            };
            return vec2;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            var vec2 = new Vector2
            {
                x = a.x + b.x,
                y = a.y + b.y,
            };
            return vec2;
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            var vec2 = new Vector2
            {
                x = a.x * b,
                y = a.y * b,
            };
            return vec2;
        }

        public static Vector2 operator *(float b, Vector2 a)
        {
            var vec2 = new Vector2
            {
                x = a.x * b,
                y = a.y * b,
            };
            return vec2;
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            var vec2 = new Vector2
            {
                x = a.x / b,
                y = a.y / b,
            };
            return vec2;
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            var vec2 = new Vector2
            {
                x = a.x * b.x,
                y = a.y * b.y,
            };
            return vec2;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt((x*x) + (y*y));
        }

        public Vector2 Normal()
        {
            return new Vector2(-y, x).Normalized();
        }

        public Vector2 Normalized()
        {
            float m = Magnitude();
            if (m != 0) return new Vector2(x / m, y / m);
            else return new Vector2();
        }

        public float Angle()
        {
            return (float)Math.Atan2(y, x);
        }

        public float AngleBetween(Vector2 b)
        {
            return (float)Math.Acos(DotProduct(this, b) / (Magnitude() * b.Magnitude()));
        }

        public Vector2 Interpolate(Vector2 b, float amount, bool safety = false)
        {
            if (amount >= 1) amount = (safety) ? 0.85f : 1;
            if (amount <= 0) amount = (safety) ? 0.15f : 0;
            return new Vector2(x + (b.x - x) * amount, y + (b.y - y) * amount);
        }

        public static Vector2 Vector2FromMag(float angle, float magnitude)
        {
            return new Vector2((float)Math.Cos(angle) * magnitude , (float)Math.Sin(angle) * magnitude);
        }

        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public override string ToString()
        {
            return $"{x},{y}";
        }
    }

    public struct Vector3
    {
        public float x;

        public float y;

        public float z;

        public Vector3(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3();
            }
        }

        public float Length()
        {
            return (float)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public bool IsEmpty()
        {
            return (int)x == 0 && (int)y == 0 && (int)z == 0;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vec3 = a - b;
            float single = (float)Math.Sqrt(vec3.x * vec3.x + vec3.y * vec3.y + vec3.z * vec3.z);
            return single;
        }

        public float DistanceFrom(Vector3 vec)
        {
            return Distance(this, vec);
        }

        public static float Dot(Vector3 left, Vector3 right)
        {
            float single = left.x * right.x + left.y * right.y + left.z * right.z;
            return single;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            var vec3 = new Vector3
            {
                x = a.x + b.x,
                y = a.y + b.y,
                z = a.z + b.z
            };
            return vec3;
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            var vec3 = new Vector3
            {
                x = a.x * b,
                y = a.y * b,
                z = a.z * b
            };
            return vec3;
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            var vec3 = new Vector3
            {
                x = a.x + b,
                y = a.y + b,
                z = a.z + b
            };
            return vec3;
        }

        public static Vector3 operator /(Vector3 value, float scale)
        {
            return new Vector3(value.x / scale, value.y / scale, value.z / scale);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            var vec3 = new Vector3
            {
                x = a.y * b.z - a.z * b.y,
                y = a.z * b.x - a.x * b.z,
                z = a.x * b.y - a.y * b.x
            };
            return vec3;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            var vec3 = new Vector3
            {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };
            return vec3;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public override string ToString()
        {
            return $"{x},{y},{z}";
        }
    }
}