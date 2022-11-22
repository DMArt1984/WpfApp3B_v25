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
    static class Work
    {
        static public DiffuseMaterial solid_material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255,0,255,11)));

        static public Point3D Center_fromTwo(Point3D one, Point3D two)
        {
            Point3D forCenter = new Point3D(0, 0, 0);
            forCenter.X = one.X + (two.X - one.X) / 2;
            forCenter.Y = one.Y + (two.Y - one.Y) / 2;
            forCenter.Z = one.Z + (two.Z - one.Z) / 2;
            return forCenter;
        }

        static public MeshGeometry3D BuildMesh(string itemNormals, string itemPositions, string itemTriangles, Color itemColor)
        {
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();
            
            // 1) Empty
            // 2) Create a collection of vertex positions for the MeshGeometry3D. 
            Point3DCollection myPositionCollection = new Point3DCollection();
            string[] Pos = itemPositions.Split(' ');
            foreach (string Item in Pos)
            {
                string[] TX = Item.Split(';');
                if (TX.Length >= 3)
                    myPositionCollection.Add(new Point3D(float.Parse(TX[0]), float.Parse(TX[1]), float.Parse(TX[2])));
            }
            myMeshGeometry3D.Positions = myPositionCollection;

            // 3) Create a collection of triangle indices for the MeshGeometry3D.
            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            string[] Tri = itemTriangles.Split(' ');
            foreach (string Item in Tri)
            {
                string[] TX = Item.Split(';');
                foreach (string Item2 in TX)
                {
                    myTriangleIndicesCollection.Add(int.Parse(Item2));
                }
            }
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            return myMeshGeometry3D;
        }

        static public GeometryModel3D BuildONE_GM(string itemNormals, string itemPositions, string itemTriangles, Color itemColor)
        {
            GeometryModel3D myGeometryModel = new GeometryModel3D();

            MeshGeometry3D myMeshGeometry3D = BuildMesh(itemNormals, itemPositions, itemTriangles, itemColor);

            // 4) Apply the mesh to the geometry model.
            myGeometryModel.Geometry = myMeshGeometry3D;

            // 5) 
            SolidColorBrush solid_brush = new SolidColorBrush(itemColor);
            DiffuseMaterial solid_material = new DiffuseMaterial(solid_brush);

            //OLD: Viewport2DVisual3D.SetIsVisualHostMaterial(solid_material, true);

            myGeometryModel.Material = solid_material;

            // Return the geometry model to the model group.
            return myGeometryModel;
        }

        static public DiffuseMaterial setMaterial(Color color)
        {
            SolidColorBrush solid_brush = new SolidColorBrush(color);
            DiffuseMaterial solid_material = new DiffuseMaterial(solid_brush);
            return solid_material;
        }

        static public void Transform3dGroup(ref Model3DGroup Obj, Vector3D objRotate, double Angle, Point3D Center, Vector3D objScale)
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            myRotateTransform3D.CenterX = Center.X;
            myRotateTransform3D.CenterY = Center.Y;
            myRotateTransform3D.CenterZ = Center.Z;
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D(objRotate,Angle);
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            ScaleTransform3D myScaleTransform3D = new ScaleTransform3D(objScale);
            myTransform3DGroup.Children.Add(myScaleTransform3D);

            Obj.Transform = myTransform3DGroup;
        }

        static public string GetPointersCube(Point3D Pos, Point3D Size)
        {
            string OUT; // = "0,0,0 1,0,0 0,1,0 1,1,0 0,0,1 1,0,1 0,1,1 1,1,1";
            OUT = "";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";

            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString();

            return OUT;
        }

        static public string GetPointersBox2(Point3D Pos, Point3D Size)
        {
            Pos = new Point3D(Pos.X, Pos.Y - Size.Y, Pos.Z);
            string OUT;
            OUT = "";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";

            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z - Size.Z).ToString();

            return OUT;
        }

        static public string GetPointersRectangleT1(Point3D Pos, Point3D Size)
        {
            string OUT; // = "-1;1;0 -1;-1;0 1;-1;0 1;1;0";
            OUT = "";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString();
            return OUT;
        }

        static public string GetPointersRectangleT2(Point3D Pos, Point3D Size)
        {
            string OUT; // = "-1;1;0 -1;-1;0 1;-1;0 1;1;0";
            OUT = "";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z).ToString();

            OUT += (Pos.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z-0.1).ToString() + " ";
            OUT += (Pos.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z-0.1).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y).ToString() + ";" + (Pos.Z-0.1).ToString() + " ";
            OUT += (Pos.X + Size.X).ToString() + ";" + (Pos.Y + Size.Y).ToString() + ";" + (Pos.Z-0.1).ToString();
            return OUT;
        }

        static public GeometryModel3D SimpleBOX(Point3D Pos, Point3D Size, Color itColor)
        {
            return Work.BuildONE_GM("1;0;0 1;0;0 1;0;0 1;0;0 1;0;0 1;0;0 1;0;0 1;0;0", Work.GetPointersCube(Pos, Size), "0;1;2 0;2;3 6;5;4 7;6;4 4;1;0 4;5;1 3;2;6 3;6;7 7;4;0 7;0;3 5;2;1 6;2;5", itColor);
        }



        static public Model3DGroup OnePanel(int Cols, int Rows, Point3D Pos, double Sqr, Vector3D Vector, float Angle)
        {
            Model3DGroup myModel3DGroup = new Model3DGroup();
            GeometryModel3D myModel = new GeometryModel3D();

            Random rnd = new Random();

            for (int Row = 1; Row <= Rows; Row++)
            {
                for (int Col = 1; Col <= Cols; Col++)
                {
                    myModel = Work.SimpleBOX(new Point3D(Pos.X + (Col-1) * Sqr * 1.0, Pos.Y - (Row) * Sqr * 1.0, Pos.Z), new Point3D(Sqr, Sqr, Sqr * 0.1), Color.FromArgb(255, Convert.ToByte(rnd.Next(0, 255)), Convert.ToByte(rnd.Next(0, 255)), Convert.ToByte(rnd.Next(0, 255))));
                    myModel3DGroup.Children.Add(myModel);
                }
            }

            Transform3dGroup(ref myModel3DGroup, Vector, Angle, new Point3D(Pos.X, Pos.Y, Pos.Z), new Vector3D(1, 1, 1));

            return myModel3DGroup;
        }

        static public Viewport2DVisual3D GetMegaText(Point3D Pos, Point3D Size, Color itColor)
        {
            var view = new Viewport2DVisual3D();
            var mesh = BuildMesh("", GetPointersRectangleT1(Pos, Size), "0;1;2 0;2;3", itColor);
            // Sample:
            //var mesh = BuildMesh("", "-1;1;0 -1;-1;0 1;-1;0 1;1;0","0;1;2 0;2;3", itColor);
            //var mesh = BuildMesh("", "-1;1;0 -1;-1;0 1;-1;0 1;1;0 -1;1;-0,5 -1;-1;-0,5 1;-1;-0,5 1;1;-0,5", "0;1;2 0;2;3 6;5;4 7;6;4 4;1;0 4;5;1", itColor);

            mesh.TextureCoordinates = new PointCollection(new Point[] {
                            new Point(0, 0),
                            new Point(0, 1),
                            new Point(1, 1),
                            new Point(1, 0),

                            new Point(1, 0),
                            new Point(1, 1),
                            new Point(0, 1),
                            new Point(0, 0),
                                                                       });
            view.Geometry = mesh;
            var material = new DiffuseMaterial { Brush = Brushes.Green };
            Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);
            view.Material = material;

            view.Visual = new Button { Content = "Testing", Background = Brushes.Red };

            TextBlock textblock = new TextBlock();
            textblock.Text = " -87.0 ";
            textblock.Background = new SolidColorBrush(Colors.Red);
            textblock.Foreground = new SolidColorBrush(Colors.White);
            textblock.FontFamily = new FontFamily("Arial");

            view.Visual = textblock;


            return view;
        }

        static public Viewport2DVisual3D GetMegaBoxOld()
        {
            var view = new Viewport2DVisual3D();
            var mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection {
                            new Point3D(-1, 1, 0),
                            new Point3D(-1, -1, 0),
                            new Point3D(1, -1, 0),
                            new Point3D(1, 1, 0)
                                                   };
            mesh.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2, 0, 2, 3 });
            mesh.TextureCoordinates = new PointCollection(new Point[] {
                            new Point(0, 0),
                            new Point(0, 1),
                            new Point(1, 1),
                            new Point(1, 0)
                                                                       });
            view.Geometry = mesh;
            var material = new DiffuseMaterial { Brush = Brushes.White };
            Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);
            view.Material = material;
            view.Transform = new RotateTransform3D
            {
                Rotation = new AxisAngleRotation3D
                {
                    Angle = 40,
                    Axis = new Vector3D(0, 1, 0)
                }
            };
            view.Visual = new Button { Content = "Testing", Background = Brushes.Aqua };

            return view;
        }

        //==========================

        static public Color GetFillColor(double PV)
        {
            double dVal = 0.0;
            double dVal2 = 0.0;
            long iVal = 0;
            Color tmpClr;
            int R;
            int G = 0;
            int B = 0;

            var clrTeMIN = 30;
            var clrTeMAX = 150;

            dVal = (PV - clrTeMIN) * (410.0 - 0) / (clrTeMAX - clrTeMIN);

            if (dVal > 410.0) dVal = 410.0;
            if (dVal < 0.0) dVal = 0.0;
            if (dVal <= 205.0)
            {
                B = 255;
                R = Convert.ToInt16(dVal + 50);
            }
            else
            {
                dVal2 = 255 - (dVal - 205.0) * (255.0 - 50.0) / (410.0 - 205.0) + 50.0;

                B = Convert.ToInt16(dVal2);
                R = 255;
            }

            if (B > 255) B = 255;
            if (R > 255) R = 255;


            // my help:
            //iVal = Convert.ToInt64(dVal);
            //int al = 255;
            //R = Convert.ToInt16((iVal & 0xFF0000)  >> 16);
            //G = Convert.ToInt16((iVal & 0xFF00) >> 8);
            //B = Convert.ToInt16((iVal & 0xFF) );

            tmpClr = Color.FromArgb(255, Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B));

            return tmpClr;
        }

    }
}
