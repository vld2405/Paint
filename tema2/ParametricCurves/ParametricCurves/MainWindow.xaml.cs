using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ParametricCurves
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CmbCurves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtA == null || TxtB == null) return;

            switch (CmbCurves.SelectedIndex)
            {
                case 0:
                    TxtA.Text = "0";
                    TxtB.Text = (2 * Math.PI).ToString("F2");
                    break;
                case 1:
                    TxtA.Text = "0";
                    TxtB.Text = "20";
                    break;
                case 2:
                    TxtA.Text = "-2";
                    TxtB.Text = "2";
                    break;
            }
        }

        private void BtnDraw_Click(object sender, RoutedEventArgs e)
        {
            DesenareCurba();
        }

        private void DrawCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                DesenareCurba();
            }
        }

        private void DesenareCurba()
        {
            DrawCanvas.Children.Clear();

            if (!double.TryParse(TxtA.Text, out double a)) a = 0;
            if (!double.TryParse(TxtB.Text, out double b)) b = Math.PI * 2;
            if (!int.TryParse(TxtN.Text, out int n) || n <= 0) n = 100;

            Func<double, double> fx = null;
            Func<double, double> fy = null;

            switch (CmbCurves.SelectedIndex)
            {
                case 0:
                    fx = u => Math.Cos(u);
                    fy = u => 2 * Math.Sin(u);
                    break;
                case 1:
                    fx = u => u * Math.Cos(u);
                    fy = u => u * Math.Sin(u);
                    break;
                case 2:
                    fx = u => u;
                    fy = u => (u * u) + 1;
                    break;
            }

            if (fx == null || fy == null) return;

            List<Point> puncteMatematice = new List<Point>();
            double pas = (b - a) / n;
            for (int i = 0; i <= n; i++)
            {
                double u = a + i * pas;
                puncteMatematice.Add(new Point(fx(u), fy(u)));
            }

            double minX = 0, maxX = 0, minY = 0, maxY = 0;
            foreach (var p in puncteMatematice)
            {
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            double lățimeMatematică = maxX - minX;
            double înălțimeMatematică = maxY - minY;
            if (lățimeMatematică == 0) lățimeMatematică = 1;
            if (înălțimeMatematică == 0) înălțimeMatematică = 1;

            minX -= lățimeMatematică * 0.1;
            maxX += lățimeMatematică * 0.1;
            minY -= înălțimeMatematică * 0.1;
            maxY += înălțimeMatematică * 0.1;

            double canvasW = DrawCanvas.ActualWidth;
            double canvasH = DrawCanvas.ActualHeight;
            if (canvasW == 0 || canvasH == 0) return;

            Func<double, double, Point> CoordEcran = (x, y) =>
            {
                double screenX = (x - minX) / (maxX - minX) * canvasW;
                double screenY = canvasH - (y - minY) / (maxY - minY) * canvasH;
                return new Point(screenX, screenY);
            };

            Point xAxisStart = CoordEcran(minX, 0);
            Point xAxisEnd = CoordEcran(maxX, 0);
            Point yAxisStart = CoordEcran(0, minY);
            Point yAxisEnd = CoordEcran(0, maxY);

            DrawCanvas.Children.Add(new Line { X1 = xAxisStart.X, Y1 = xAxisStart.Y, X2 = xAxisEnd.X, Y2 = xAxisEnd.Y, Stroke = Brushes.Gray, StrokeThickness = 1 });
            DrawCanvas.Children.Add(new Line { X1 = yAxisStart.X, Y1 = yAxisStart.Y, X2 = yAxisEnd.X, Y2 = yAxisEnd.Y, Stroke = Brushes.Gray, StrokeThickness = 1 });

            Polyline polilinie = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            foreach (var p in puncteMatematice)
            {
                polilinie.Points.Add(CoordEcran(p.X, p.Y));
            }

            DrawCanvas.Children.Add(polilinie);
        }
    }
}