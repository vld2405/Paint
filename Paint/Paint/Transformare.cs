using System;
using System.Windows;

namespace Paint
{
    public class Transformare
    {
        public double[,] Matrice { get; private set; }

        public Transformare() => Resetare();

        public void Resetare()
        {
            Matrice = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        }

        private void InmultesteLaStanga(double[,] noua)
        {
            double[,] rezultat = new double[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        rezultat[i, j] += noua[i, k] * Matrice[k, j];
            Matrice = rezultat;
        }

        public void Rotatie0(double cosA, double sinA)
        {
            InmultesteLaStanga(new double[,] { { cosA, -sinA, 0 }, { sinA, cosA, 0 }, { 0, 0, 1 } });
        }

        public void Translatie(double tx, double ty)
        {
            InmultesteLaStanga(new double[,] { { 1, 0, tx }, { 0, 1, ty }, { 0, 0, 1 } });
        }

        public void Scalare0(double sx, double sy)
        {
            InmultesteLaStanga(new double[,] { { sx, 0, 0 }, { 0, sy, 0 }, { 0, 0, 1 } });
        }

        public void Simetrie0() => Scalare0(-1, -1);

        public void RotatieA(Point A, double cosA, double sinA)
        {
            Translatie(-A.X, -A.Y);
            Rotatie0(cosA, sinA);
            Translatie(A.X, A.Y);
        }

        public void ScalareA(Point A, double sx, double sy)
        {
            Translatie(-A.X, -A.Y);
            Scalare0(sx, sy);
            Translatie(A.X, A.Y);
        }

        public void SimetrieDreapta(Point A, Vector v)
        {
            Translatie(-A.X, -A.Y);

            double lungime = Math.Sqrt(v.X * v.X + v.Y * v.Y);
            double cosTheta = v.X / lungime;
            double sinTheta = v.Y / lungime;

            Rotatie0(cosTheta, -sinTheta);

            Scalare0(1, -1);

            Rotatie0(cosTheta, sinTheta);
            Translatie(A.X, A.Y);
        }

        public Point TransformaPunct(Point p)
        {
            double x = Matrice[0, 0] * p.X + Matrice[0, 1] * p.Y + Matrice[0, 2];
            double y = Matrice[1, 0] * p.X + Matrice[1, 1] * p.Y + Matrice[1, 2];
            return new Point(x, y);
        }
    }
}