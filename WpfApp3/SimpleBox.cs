using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf3Dapp
{
    // Simple Box
    class SimpleBoxPanel
    {
        public List<GeometryModel3D> Box = new List<GeometryModel3D>(); // object
        public List<int> Num = new List<int>(); // the number of the received value
        public Model3DGroup Panel = new Model3DGroup();
        public Point3D forCenter = new Point3D(0,0,0);
        public int ID = 0;

        // creation
        public SimpleBoxPanel(int Cols, int Rows, Point3D Pos, double Width, double Height, double Thickness, Vector3D Vector, double Angle, string[] Numbers, Color itColor)
        {
            if (Cols > 0 && Rows > 0)
            {
                double SqrH = Width / Cols;
                double SqrV = Height / Rows;
                Thickness = (Thickness == 0) ? Width * 0.02 : Thickness;

                Panel = new Model3DGroup();
                Box.Clear();
                Num.Clear();

                Random rnd = new Random(DateTime.Now.Millisecond);

                int Index = 0;
                for (int Row = 1; Row <= Rows; Row++)
                {
                    for (int Col = 1; Col <= Cols; Col++)
                    {
                        Index++;
                        double X = Pos.X + (Col - 1) * SqrH * 1.0;
                        double Y = Pos.Y - Row * SqrV * 1.0;
                        double Z = Pos.Z;
                        
                        byte BT = Convert.ToByte(rnd.Next(150, 200));
                        itColor = Color.FromArgb(100,BT, BT, BT);
                        Box.Add(Work.SimpleBOX(new Point3D(X, Y, Z), new Point3D(SqrH, SqrV, Thickness), itColor));
                        Panel.Children.Add(Box.Last());

                        int Val = Index;
                        if (Index < Numbers.Length)
                        {
                            Val = Convert.ToInt16(Numbers[Index-1]);
                        }
                        Num.Add(Val);

                    }
                }

                Work.Transform3dGroup(ref Panel, Vector, Angle, new Point3D(Pos.X, Pos.Y, Pos.Z), new Vector3D(1, 1, 1));

                forCenter = new Point3D(Pos.X + Width / 2, Pos.Y - Height / 2, Pos.Z + Thickness / 2);

            }
        }

    }
}
