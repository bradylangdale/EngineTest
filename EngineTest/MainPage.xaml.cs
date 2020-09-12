using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;
using Windows.System;
using Windows.Devices.Display.Core;
using System;

namespace EngineTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<RigidBody> bodies = new List<RigidBody>();
        public static List<Vector2> p2d = new List<Vector2>();
        public static List<Vector2> lines = new List<Vector2>();
        //public static Vector2 pab, pac, pbc;
        public static Vector2 origin;
        float t = 0;
        static bool debug_drawing = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void InitializeBodies()
        {
            bodies.Add(new RigidBody(new Triangle[] {
                new Triangle(new Vector2[] { new Vector2(), new Vector2(30, 0), new Vector2(0, -90) }),
                new Triangle(new Vector2[] { new Vector2(30, 0), new Vector2(30, -90), new Vector2(0, -90) })}, new Vector2(-60, 150), 0, 3f, 0.5f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -60), new Vector2(-60, 0) }), new Triangle(new Vector2[] { new Vector2(-60, 0), new Vector2(-60, -60), new Vector2(0, -60) }) }, new Vector2(0, 240), 0f, 3f, 0.5f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -60), new Vector2(-60, 0) }), new Triangle(new Vector2[] { new Vector2(-60, 0), new Vector2(-60, -60), new Vector2(0, -60) }) }, new Vector2(80, 240), 0f, 3f, 0.5f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -60), new Vector2(-60, 0) }), new Triangle(new Vector2[] { new Vector2(-60, 0), new Vector2(-60, -60), new Vector2(0, -60) }) }, new Vector2(-160f, 179f), 0f, 3f, 0.5f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -60), new Vector2(-60, 0) }), new Triangle(new Vector2[] { new Vector2(-60, 0), new Vector2(-60, -60), new Vector2(0, -60) }) }, new Vector2(160f, 179f), 0f, 3f, 0.5f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -200), new Vector2(-360, 0) })}, new Vector2(440, 220), 0, 10f, 0f, true));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -200), new Vector2(360, 0) }) }, new Vector2(-440, 220), 0, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 200), new Vector2(-360, 0) }) }, new Vector2(440, -220), 0, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 200), new Vector2(360, 0) }) }, new Vector2(-440, -220), 0, 10f, 0f, true));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(0, 300), 0, 10f, 0f, true));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(0, -300), 0f, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(550, 0), (float)Math.PI / 2, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(-550, 0), (float)Math.PI / 2, 10f, 0f, true));
            //bodies[0].velocity.y = 10f;
        }

        private void Initialized(object sender, RoutedEventArgs e)
        {
            InitializeBodies();
            origin = new Vector2((float)ActualWidth / 2f, (float)ActualHeight / 2f);
        }

        private void FixedUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            try
            {
                if (bodies[0].position.x + origin.x < 0) bodies[0].position = new Vector2(origin.x, bodies[0].position.y);
                if (bodies[0].position.x + origin.x > origin.x * 2) bodies[0].position = new Vector2(-origin.x, bodies[0].position.y);
                if (bodies[0].position.y + origin.y < 0) bodies[0].position = new Vector2(bodies[0].position.x, origin.y);
                if (bodies[0].position.y + origin.y > origin.y * 2) bodies[0].position = new Vector2(bodies[0].position.x, -origin.y);

                if (bodies[1].position.x + origin.x < 0) bodies[1].position = new Vector2(origin.x, bodies[1].position.y);
                if (bodies[1].position.x + origin.x > origin.x * 2) bodies[1].position = new Vector2(-origin.x, bodies[1].position.y);
                if (bodies[1].position.y + origin.y < 0) bodies[1].position = new Vector2(bodies[1].position.x, origin.y);
                if (bodies[1].position.y + origin.y > origin.y * 2) bodies[1].position = new Vector2(bodies[1].position.x, -origin.y);

                if (bodies[2].position.x + origin.x < 0) bodies[2].position = new Vector2(origin.x, bodies[2].position.y);
                if (bodies[2].position.x + origin.x > origin.x * 2) bodies[2].position = new Vector2(-origin.x, bodies[2].position.y);
                if (bodies[2].position.y + origin.y < 0) bodies[2].position = new Vector2(bodies[2].position.x, origin.y);
                if (bodies[2].position.y + origin.y > origin.y * 2) bodies[2].position = new Vector2(bodies[2].position.x, -origin.y);

                bodies[0].velocity.y += 1.2f;
                bodies[1].velocity.y += 1.2f;
                bodies[2].velocity.y += 1.2f;
                bodies[3].velocity.y += 1.2f;
                bodies[4].velocity.y += 1.2f;

                int collisions = 1;
                int loops = 0;
                while (collisions > 0 && loops < 3)
                {
                    collisions = 0;
                    foreach (RigidBody body in bodies)
                    {
                        if (body.Static) continue;
                        if (body.ResolveCollisions(bodies.ToArray(), args.Timing.ElapsedTime.Milliseconds * 0.001f, loops == 0)) collisions++;
                    }
                    loops++;
                }

                foreach (RigidBody body in bodies)
                {
                    body.Update(args.Timing.ElapsedTime.Milliseconds * 0.001f);
                }
            }
            catch { }
        }

        private void Update(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs screen)
        {
            try
            {
                foreach (RigidBody body in bodies)
                {
                    foreach (Triangle tri in body.polygon)
                    {
                        screen.DrawingSession.FillGeometry(CanvasGeometry.CreatePolygon(canvas.Device, tri.GetVertexArray(origin)), (body.velocity.Magnitude() <= 0) ? Colors.RoyalBlue : Colors.OrangeRed);
                    }
                }

                if (!debug_drawing) return;

                foreach (Vector2 point in p2d)
                {
                    screen.DrawingSession.DrawCircle(point + origin, 2f, Colors.Red);
                }

                for (int i = 0; i < lines.Count; i += 2)
                {
                    screen.DrawingSession.DrawLine(lines[i] + origin, lines[i + 1] + origin, Colors.Green);
                }

                //screen.DrawingSession.DrawCircle(pab + origin, 2f, Colors.Cyan);
                //screen.DrawingSession.DrawCircle(pac + origin, 2f, Colors.Cyan);
                //screen.DrawingSession.DrawCircle(pbc + origin, 2f, Colors.Cyan);
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            debug_drawing = !debug_drawing;
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            //t += 0.1f;
            bodies.Clear();
            InitializeBodies();
            p2d.Clear();
            lines.Clear();
        }

        private void keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.A) bodies[0].velocity.x -= 10f;
            if (e.Key == VirtualKey.D) bodies[0].velocity.x += 10f;
            if (e.Key == VirtualKey.W) bodies[0].velocity.y -= 100f;
        }
    }
}
