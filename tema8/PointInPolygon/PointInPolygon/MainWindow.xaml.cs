using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PointInPolygon
{
    public partial class MainWindow : Window
    {
        private List<Point> varfuri = new List<Point>();
        private bool poligonFinalizat = false;
        private Point ultimaPozitieMouse;
        private Polygon poligonVizual = new Polygon();

        public MainWindow()
        {
            InitializeComponent();
            poligonVizual.Stroke = Brushes.Black;
            poligonVizual.StrokeThickness = 1;
            MainCanvas.Children.Add(poligonVizual);
        }

        private double F(Point A, Point B, Point C) =>
            (B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X);

        private int Sgn(Point A, Point B, Point C)
        {
            double val = F(A, B, C);
            return val > 0 ? 1 : (val < 0 ? -1 : 0);
        }

        private bool PunctInPoligon(Point P, List<Point> puncte)
        {
            bool interior = false;
            int j = puncte.Count - 1;
            for (int i = 0; i < puncte.Count; i++)
            {
                if (((puncte[i].Y > P.Y) != (puncte[j].Y > P.Y)) &&
                    (P.X < (puncte[j].X - puncte[i].X) * (P.Y - puncte[i].Y) / (puncte[j].Y - puncte[i].Y) + puncte[i].X))
                {
                    interior = !interior;
                }
                j = i;
            }
            return interior;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);

            if (!poligonFinalizat)
            {
                if (e.ClickCount == 2 && varfuri.Count > 2)
                {
                    poligonFinalizat = true;
                    StatusText.Text = "Status: Poligon finalizat. Colorez interiorul...";
                    ActualizeazaDesen();
                }
                else if (e.ClickCount == 1)
                {
                    varfuri.Add(p);
                    DesenareContur();
                }
            }
            else
            {
                ultimaPozitieMouse = p;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (poligonFinalizat && e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Point pozitieCurenta = e.GetPosition(MainCanvas);
                double dx = pozitieCurenta.X - ultimaPozitieMouse.X;
                double dy = pozitieCurenta.Y - ultimaPozitieMouse.Y;

                for (int i = 0; i < varfuri.Count; i++)
                {
                    varfuri[i] = new Point(varfuri[i].X + dx, varfuri[i].Y + dy);
                }

                ultimaPozitieMouse = pozitieCurenta;
                ActualizeazaDesen();
            }
        }

        private void DesenareContur()
        {
            poligonVizual.Points.Clear();
            foreach (var v in varfuri) poligonVizual.Points.Add(v);
        }

        private void ActualizeazaDesen()
        {
            var deSters = MainCanvas.Children.OfType<Rectangle>().ToList();
            foreach (var r in deSters) MainCanvas.Children.Remove(r);

            DesenareContur();

            if (varfuri.Count < 3) return;

            double minX = varfuri.Min(p => p.X);
            double maxX = varfuri.Max(p => p.X);
            double minY = varfuri.Min(p => p.Y);
            double maxY = varfuri.Max(p => p.Y);

            for (double x = minX; x <= maxX; x += 3)
            {
                for (double y = minY; y <= maxY; y += 3)
                {
                    Point testPoint = new Point(x, y);
                    if (PunctInPoligon(testPoint, varfuri))
                    {
                        Rectangle pixel = new Rectangle { Width = 3, Height = 3, Fill = Brushes.Green };
                        Canvas.SetLeft(pixel, x);
                        Canvas.SetTop(pixel, y);
                        MainCanvas.Children.Add(pixel);
                    }
                }
            }
        }
    }
}