using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;
using Windows.System;

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
        public static Vector2 pab, pac, pbc;
        public static Vector2 origin;
        float t = 0;
        static bool debug_drawing = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void InitializeBodies()
        {
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -30), new Vector2(-30, 0) }), new Triangle(new Vector2[] { new Vector2(-30, 0), new Vector2(-30, -30), new Vector2(0, -30) }) }, new Vector2(0, 0), 0f, 1f, 0f, false));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -35), new Vector2(-35, 0) })}, new Vector2(33, 150), 0, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -25), new Vector2(-45, 0) }) }, new Vector2(-15, 180), 0, 99f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -55), new Vector2(15, 0) }) }, new Vector2(-195, 100), 0, 99f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -25), new Vector2(45, 0) }) }, new Vector2(-155, 180), 0, 99f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -95), new Vector2(-16, 0) }) }, new Vector2(60f, -80), -0.1f, 99f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -55), new Vector2(60, 0) }) }, new Vector2(-280f, 240), 0f, 99f, true));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(0, 300), t, 10f, 0f, true));
            bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, 50), new Vector2(480, 0) }), new Triangle(new Vector2[] { new Vector2(480, 0), new Vector2(480, 50), new Vector2(0, 50) }) }, new Vector2(0, -300), 0f, 10f, 0f, true));
            //bodies.Add(new RigidBody(new Triangle[] { new Triangle(new Vector2[] { new Vector2(), new Vector2(0, -30), new Vector2(-30, 0) }), new Triangle(new Vector2[] { new Vector2(-30, 0), new Vector2(-30, -30), new Vector2(0, -30) }) }, new Vector2(150, 250), 0f, 1f, true));
            bodies[0].velocity.y = 100f;
        }

        private void Initialized(object sender, RoutedEventArgs e)
        {
            InitializeBodies();
            origin = new Vector2((float)ActualWidth / 2f, (float)ActualHeight / 2f);
        }

        private void FixedUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            bodies[0].velocity.y += 1.2f;
            foreach (RigidBody body in bodies)
            {
                body.Update(bodies.ToArray(), args.Timing.ElapsedTime.Milliseconds * 0.001f);
            }
        }

        private void Update(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs screen)
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
                screen.DrawingSession.DrawLine(lines[i] + origin, lines[i+1] + origin, Colors.Green);
            }

            screen.DrawingSession.DrawCircle(pab + origin, 2f, Colors.Cyan);
            screen.DrawingSession.DrawCircle(pac + origin, 2f, Colors.Cyan);
            screen.DrawingSession.DrawCircle(pbc + origin, 2f, Colors.Cyan);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            debug_drawing = !debug_drawing;
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            t += 0.1f;
            bodies.Clear();
            InitializeBodies();
            p2d.Clear();
            lines.Clear();
        }

        private void keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.A) bodies[0].velocity.x -= 10f;
            if (e.Key == VirtualKey.D) bodies[0].velocity.x += 10f;
        }
    }
}
