using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace tema3_4
{
    public partial class MainWindow : Window
    {
        private List<Point> puncteControl = new List<Point>();
        private bool introducereFinalizata = false;
        private int indexPunctSelectat = -1;

        private double[] coeficientiNewton;

        private Polyline liniaCurbei = new Polyline
        {
            Stroke = Brushes.Blue,
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round
        };

        public MainWindow()
        {
            InitializeComponent();
            MainCanvas.Children.Add(liniaCurbei);
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);

            if (!introducereFinalizata)
            {
                if (puncteControl.Count == 0 || p.X > puncteControl.Last().X)
                {
                    puncteControl.Add(p);
                    DeseneazaPuncteControl();
                }
                else
                {
                    MessageBox.Show("Punctele trebuie introduse strict de la stânga la dreapta (X crescător)!");
                }
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
            if (!introducereFinalizata && puncteControl.Count > 1)
            {
                introducereFinalizata = true;
                InstructionsText.Text = "Trage de puncte (Click stânga + Drag) pentru a le muta.\nMișcarea pe orizontală este limitată de vecini.";
                DeseneazaCurba();
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (introducereFinalizata && indexPunctSelectat != -1)
            {
                Point p = e.GetPosition(MainCanvas);

                double minX = (indexPunctSelectat > 0) ? puncteControl[indexPunctSelectat - 1].X + 1 : 0;
                double maxX = (indexPunctSelectat < puncteControl.Count - 1) ? puncteControl[indexPunctSelectat + 1].X - 1 : MainCanvas.ActualWidth;

                if (p.X < minX) p.X = minX;
                if (p.X > maxX) p.X = maxX;

                puncteControl[indexPunctSelectat] = p;

                DeseneazaPuncteControl();
                DeseneazaCurba();
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

        private void CmbMetoda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (introducereFinalizata)
            {
                DeseneazaCurba();
            }
        }

        private void DeseneazaPuncteControl()
        {
            var elipse = MainCanvas.Children.OfType<Ellipse>().ToList();
            foreach (var elipsa in elipse)
            {
                MainCanvas.Children.Remove(elipsa);
            }

            foreach (var p in puncteControl)
            {
                Ellipse el = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    Stroke = Brushes.DarkRed,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(el, p.X - 5);
                Canvas.SetTop(el, p.Y - 5);
                MainCanvas.Children.Add(el);
            }
        }

        private void DeseneazaCurba()
        {
            liniaCurbei.Points.Clear();
            if (puncteControl.Count < 2) return;

            bool esteNewton = CmbMetoda.SelectedIndex == 1;
            if (esteNewton)
            {
                CalculeazaCoeficientiNewton();
            }

            double startX = puncteControl.First().X;
            double endX = puncteControl.Last().X;

            for (double x = startX; x <= endX; x += 1.0)
            {
                double y = esteNewton ? EvalueazaNewton(x) : CalculeazaLagrange(x);
                liniaCurbei.Points.Add(new Point(x, y));
            }
        }

        private double CalculeazaLagrange(double x)
        {
            double rezultat = 0;
            int n = puncteControl.Count;

            for (int i = 0; i < n; i++)
            {
                double termen = puncteControl[i].Y;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        termen *= (x - puncteControl[j].X) / (puncteControl[i].X - puncteControl[j].X);
                    }
                }
                rezultat += termen;
            }

            return rezultat;
        }

        private void CalculeazaCoeficientiNewton()
        {
            int n = puncteControl.Count;
            double[,] a = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                a[i, i] = puncteControl[i].Y;
            }

            for (int h = n - 2; h >= 0; h--)
            {
                for (int k = h + 1; k < n; k++)
                {
                    a[h, k] = (a[h + 1, k] - a[h, k - 1]) / (puncteControl[k].X - puncteControl[h].X);
                }
            }

            coeficientiNewton = new double[n];
            for (int i = 0; i < n; i++)
            {
                coeficientiNewton[i] = a[0, i];
            }
        }

        private double EvalueazaNewton(double x)
        {
            int n = puncteControl.Count;
            double rezultat = coeficientiNewton[0];
            double produs = 1.0;

            for (int i = 1; i < n; i++)
            {
                produs *= (x - puncteControl[i - 1].X);
                rezultat += coeficientiNewton[i] * produs;
            }

            return rezultat;
        }
    }
}