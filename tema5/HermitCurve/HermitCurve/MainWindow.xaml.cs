using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HermitCurve
{
    public partial class MainWindow : Window
    {
        private List<Point> puncteControl = new List<Point>();
        private bool introducereFinalizata = false;
        private int indexPunctSelectat = -1;

        private Polyline liniaCurbei = new Polyline
        {
            Stroke = Brushes.Red,
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);

            if (!introducereFinalizata)
            {
                puncteControl.Add(p);
                Deseneaza();
            }
            else
            {
                for (int i = 0; i < puncteControl.Count; i++)
                {
                    if (System.Math.Abs(puncteControl[i].X - p.X) < 10 &&
                        System.Math.Abs(puncteControl[i].Y - p.Y) < 10)
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
            if (!introducereFinalizata && puncteControl.Count >= 4 && puncteControl.Count % 2 == 0)
            {
                introducereFinalizata = true;
                InstructionsText.Text = "Mod Editare: Trage de puncte (Click stânga + Drag) pentru a le muta.\nSe observă controlul local al curbei.";
                Deseneaza();
            }
            else if (!introducereFinalizata)
            {
                MessageBox.Show("Vă rugăm să introduceți un număr par de puncte (minim 4) înainte de a finaliza!");
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (introducereFinalizata && indexPunctSelectat != -1)
            {
                Point p = e.GetPosition(MainCanvas);
                puncteControl[indexPunctSelectat] = p; 
                Deseneaza(); 
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (indexPunctSelectat != -1)
            {
                indexPunctSelectat = -1;
                MainCanvas.ReleaseMouseCapture();
            }
        }

        private void Deseneaza()
        {
            MainCanvas.Children.Clear();

            for (int i = 0; i < puncteControl.Count - 1; i += 2)
            {
                Line linieTangenta = new Line
                {
                    X1 = puncteControl[i].X,
                    Y1 = puncteControl[i].Y,
                    X2 = puncteControl[i + 1].X,
                    Y2 = puncteControl[i + 1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                MainCanvas.Children.Add(linieTangenta);
            }

            MainCanvas.Children.Add(liniaCurbei);
            ActualizeazaCurba();

            for (int i = 0; i < puncteControl.Count; i++)
            {
                Ellipse el = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    Stroke = Brushes.DarkRed,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(el, puncteControl[i].X - 5);
                Canvas.SetTop(el, puncteControl[i].Y - 5);
                MainCanvas.Children.Add(el);

                TextBlock text = new TextBlock
                {
                    Text = $"P{i}",
                    Foreground = Brushes.Black,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(text, puncteControl[i].X + 8);
                Canvas.SetTop(text, puncteControl[i].Y - 12);
                MainCanvas.Children.Add(text);
            }
        }

        private void ActualizeazaCurba()
        {
            liniaCurbei.Points.Clear();
            int n = puncteControl.Count;

            if (n >= 4 && n % 2 == 0)
            {
                for (int i = 0; i <= n - 4; i += 2)
                {
                    Vector a = puncteControl[i + 1] - puncteControl[i];
                    Vector b = puncteControl[i + 3] - puncteControl[i + 2];

                    for (double u = 0; u <= 1; u += 0.01)
                    {
                        Point p = Hermite(u, puncteControl[i], a, puncteControl[i + 2], b);
                        liniaCurbei.Points.Add(p);
                    }
                }
            }
        }

        private Point Hermite(double u, Point A, Vector a, Point B, Vector b)
        {
            double u2 = u * u;
            double u3 = u2 * u;

            double F1 = 2 * u3 - 3 * u2 + 1;
            double F2 = -2 * u3 + 3 * u2;
            double F3 = u3 - 2 * u2 + u;
            double F4 = u3 - u2;

            double x = F1 * A.X + F2 * B.X + F3 * a.X + F4 * b.X;
            double y = F1 * A.Y + F2 * B.Y + F3 * a.Y + F4 * b.Y;

            return new Point(x, y);
        }
    }
}