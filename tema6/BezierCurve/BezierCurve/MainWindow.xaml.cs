using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierCurve
{
    public partial class MainWindow : Window
    {
        private List<Point> puncteControl = new List<Point>();
        private bool introducereFinalizata = false;
        private int indexPunctSelectat = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        
        private double Combinari(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;
            if (k > n / 2) k = n - k;
            double res = 1;
            for (int i = 1; i <= k; i++) res = res * (n - i + 1) / i;
            return res;
        }

        private Point CalcularBezier(double u, List<Point> P)
        {
            double x = 0, y = 0;
            int n = P.Count - 1;
            for (int i = 0; i <= n; i++)
            {
                double B = Combinari(n, i) * Math.Pow(u, i) * Math.Pow(1 - u, n - i);
                x += B * P[i].X;
                y += B * P[i].Y;
            }
            return new Point(x, y);
        }

        private void ActualizeazaDesen()
        {
            var elipseDeSters = MainCanvas.Children.OfType<Ellipse>().ToList();
            foreach (var el in elipseDeSters) MainCanvas.Children.Remove(el);

            
            for (int i = 0; i < puncteControl.Count; i++)
            {
                Ellipse punctVizual = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    Stroke = Brushes.DarkRed
                };
                Canvas.SetLeft(punctVizual, puncteControl[i].X - 5);
                Canvas.SetTop(punctVizual, puncteControl[i].Y - 5);
                MainCanvas.Children.Add(punctVizual);
            }

            
            liniaCurbei.Points.Clear();
            if (puncteControl.Count >= 2)
            {
                for (double u = 0; u <= 1.0; u += 0.005)
                {
                    liniaCurbei.Points.Add(CalcularBezier(u, puncteControl));
                }
                
                liniaCurbei.Points.Add(puncteControl.Last());
            }
        }

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
            if (!introducereFinalizata && puncteControl.Count >= 2)
            {
                introducereFinalizata = true;
                InstructionsText.Text = "Mod Editare: Trage de punctele roșii pentru a redesena curba.";
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
    }
}