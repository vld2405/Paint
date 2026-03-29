using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BSpline
{
    public partial class MainWindow : Window
    {
        private List<Point> puncteControl = new List<Point>();
        private bool introducereFinalizata = false;
        private int indexPunctSelectat = -1;
        private List<Color> culoriCurbe = new List<Color>
        {
            Colors.Red, Colors.Gray, Colors.Blue, Colors.Orange, Colors.Purple, Colors.Brown
        };

        public MainWindow() => InitializeComponent();

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            if (!introducereFinalizata)
            {
                puncteControl.Add(p);
                ActualizeazaDesen();
            }
            else
            {
                for (int i = 0; i < puncteControl.Count; i++)
                {
                    if (Math.Abs(puncteControl[i].X - p.X) < 10 && Math.Abs(puncteControl[i].Y - p.Y) < 10)
                    {
                        indexPunctSelectat = i;
                        MainCanvas.CaptureMouse();
                        break;
                    }
                }
            }
        }

        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (puncteControl.Count > 2)
            {
                introducereFinalizata = true;
                ActualizeazaDesen();
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (introducereFinalizata && indexPunctSelectat != -1)
            {
                puncteControl[indexPunctSelectat] = e.GetPosition(MainCanvas);
                ActualizeazaDesen();
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            indexPunctSelectat = -1;
            MainCanvas.ReleaseMouseCapture();
        }

        private void ActualizeazaDesen()
        {
            MainCanvas.Children.Clear();
            int n = puncteControl.Count - 1;

            // 1. Desenăm liniile de contur (poligonul de control)
            if (puncteControl.Count > 1)
            {
                Polyline controlPolygon = new Polyline
                {
                    // Setăm culoarea gri cu opacitate 50% (Alpha = 128 din 255)
                    Stroke = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128)),
                    StrokeThickness = 2
                };
                foreach (var p in puncteControl) controlPolygon.Points.Add(p);
                MainCanvas.Children.Add(controlPolygon);
            }

            // 2. Desenăm punctele de control (cercurile roșii)
            foreach (var p in puncteControl)
            {
                Ellipse el = new Ellipse { Width = 8, Height = 8, Fill = Brushes.Red };
                Canvas.SetLeft(el, p.X - 4); Canvas.SetTop(el, p.Y - 4);
                MainCanvas.Children.Add(el);
            }

            // 3. Desenăm curbele B-Spline
            if (introducereFinalizata && n >= 2)
            {
                for (int k = 1; k < n; k++)
                {
                    Polyline curveLine = new Polyline
                    {
                        Stroke = new SolidColorBrush(culoriCurbe[k % culoriCurbe.Count]),
                        StrokeThickness = 2
                    };

                    int m = n + k + 1;
                    double[] t = new double[m + 1];
                    for (int i = 0; i <= m; i++) t[i] = i;

                    for (double u = t[k]; u <= t[n + 1]; u += 0.05)
                    {
                        curveLine.Points.Add(CalculeazaBSpline(u, k, puncteControl, t));
                    }
                    MainCanvas.Children.Add(curveLine);
                }
            }
        }

        private Point CalculeazaBSpline(double u, int k, List<Point> P, double[] t)
        {
            int n = P.Count - 1;
            double x = 0, y = 0;
            for (int i = 0; i <= n; i++)
            {
                double b = N(i, k, u, t);
                x += b * P[i].X;
                y += b * P[i].Y;
            }
            return new Point(x, y);
        }

        private double N(int i, int k, double u, double[] t)
        {
            if (k == 0) return (u >= t[i] && u < t[i + 1]) ? 1.0 : 0.0;
            double den1 = t[i + k] - t[i];
            double den2 = t[i + k + 1] - t[i + 1];
            double term1 = (den1 == 0) ? 0 : ((u - t[i]) / den1) * N(i, k - 1, u, t);
            double term2 = (den2 == 0) ? 0 : ((t[i + k + 1] - u) / den2) * N(i + 1, k - 1, u, t);
            return term1 + term2;
        }
    }
}