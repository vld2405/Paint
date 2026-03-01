using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Paint
{
    public partial class MainWindow : Window
    {
        private List<Point> puncteOriginale = new List<Point>();
        private Transformare t = new Transformare();
        private Polygon poligon = new Polygon();
        private Point ultimaPozitieMouse;
        private bool poligonFinalizat = false;

        private bool modSelectieDreapta = false;
        private Point? primulPunctDreapta = null;
        private Line linieVizualaDreapta = new Line
        {
            Stroke = Brushes.Red,
            StrokeDashArray = new DoubleCollection() { 4, 2 },
            Visibility = Visibility.Collapsed
        };

        public MainWindow()
        {
            InitializeComponent();
            poligon.Stroke = Brushes.Blue;
            poligon.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            poligon.StrokeThickness = 2;
            MainCanvas.Children.Add(poligon);

            MainCanvas.Children.Add(linieVizualaDreapta);
        }

        private void BtnSimetrieDreapta_Click(object sender, RoutedEventArgs e)
        {
            if (!poligonFinalizat) return;
            modSelectieDreapta = true;
            primulPunctDreapta = null;
            MessageBox.Show("Selectați două puncte pe canvas pentru a defini dreapta de simetrie.");
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (modSelectieDreapta)
            {
                Point p = e.GetPosition(MainCanvas);
                if (primulPunctDreapta == null)
                {
                    primulPunctDreapta = p;
                    linieVizualaDreapta.X1 = p.X;
                    linieVizualaDreapta.Y1 = p.Y;
                    linieVizualaDreapta.Visibility = Visibility.Visible;
                }
                else
                {
                    Vector v = p - primulPunctDreapta.Value;
                    if (v.Length > 0)
                    {
                        t.SimetrieDreapta(primulPunctDreapta.Value, v);
                        ActualizeazaDesen();
                    }

                    modSelectieDreapta = false;
                    linieVizualaDreapta.Visibility = Visibility.Collapsed;
                }
                return;
            }

            if (poligonFinalizat)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    ultimaPozitieMouse = e.GetPosition(MainCanvas);
                    return;
                }
            }

            if (!poligonFinalizat)
            {
                if (e.ClickCount == 2)
                {
                    poligonFinalizat = true;
                    return;
                }
                puncteOriginale.Add(e.GetPosition(MainCanvas));
                ActualizeazaDesen();
            }
        }


        private Point GetCentruPoligon()
        {
            if (puncteOriginale.Count == 0) return new Point(0, 0);

            double sumX = 0, sumY = 0;
            foreach (var p in puncteOriginale)
            {
                Point pTransformat = t.TransformaPunct(p);
                sumX += pTransformat.X;
                sumY += pTransformat.Y;
            }

            return new Point(sumX / puncteOriginale.Count, sumY / puncteOriginale.Count);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (modSelectieDreapta && primulPunctDreapta != null)
            {
                Point p = e.GetPosition(MainCanvas);
                linieVizualaDreapta.X2 = p.X;
                linieVizualaDreapta.Y2 = p.Y;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pozitieCurenta = e.GetPosition(MainCanvas);
                double deltaX = pozitieCurenta.X - ultimaPozitieMouse.X;

                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    double dy = pozitieCurenta.Y - ultimaPozitieMouse.Y;
                    t.Translatie(deltaX, dy);
                }
                
                else if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    double unghiGrade = deltaX * 0.5;
                    double rad = unghiGrade * Math.PI / 180;

                    Point centru = GetCentruPoligon();
                    t.RotatieA(centru, Math.Cos(rad), Math.Sin(rad));
                }

                ultimaPozitieMouse = pozitieCurenta;
                ActualizeazaDesen();
            }
        }

        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double factor = (e.Delta > 0) ? 1.1 : 0.9;
                Point centru = GetCentruPoligon();

                t.ScalareA(centru, factor, factor);
                ActualizeazaDesen();
            }
        }

        private void ActualizeazaDesen()
        {
            poligon.Points.Clear();
            foreach (var p in puncteOriginale)
            {
                poligon.Points.Add(t.TransformaPunct(p));
            }
        }


        private void BtnSimetrie_Click(object sender, RoutedEventArgs e)
        {
            t.Simetrie0();
            ActualizeazaDesen();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            t.Resetare();
            puncteOriginale.Clear();
            poligonFinalizat = false;
            ActualizeazaDesen();
        }
    }
}