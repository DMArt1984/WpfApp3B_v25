using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wpf3Dapp
{
    // Text field
    class SimpleTextRectPanel
    {
        public List<TextBlock> DText = new List<TextBlock>(); // object
        public List<int> Num = new List<int>(); // the number of the received value
        public Viewport2DVisual3D Viewport = new Viewport2DVisual3D();
        public Point3D forCenter = new Point3D(0, 0, 0);
        public int ID = 0;
        public bool EnableText;

        // creation
        public SimpleTextRectPanel(int Cols, int Rows, Point3D Pos, double Width, double Height, double Thickness, Vector3D Vector, double itAngle, string[] Numbers, Color itColor, bool useText)
        {
            EnableText = useText;
            if (Cols > 0 && Rows > 0)
            {
                double SqrH = Width / Cols;
                double SqrV = Height / Rows;
                Thickness = (Thickness == 0) ? Width * 0.02 : Thickness;

                Viewport = new Viewport2DVisual3D();
                DText.Clear();
                Num.Clear();

                Random rnd = new Random(DateTime.Now.Millisecond);

                // Table
                Grid myGrid = new Grid();
                myGrid.ShowGridLines = false;
                myGrid.Background = new SolidColorBrush(Colors.Black);

                // Columns
                for (int Col = 1; Col <= Cols; Col++)
                    //OLD: myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SqrH)});
                    myGrid.ColumnDefinitions.Add(new ColumnDefinition());

                // Rows
                for (int Row = 1; Row <= Rows; Row++)
                    myGrid.RowDefinitions.Add(new RowDefinition());

                // Cells for text
                int Index = 0;
                for (int Row = 1; Row <= Rows; Row++)
                {
                    for (int Col = 1; Col <= Cols; Col++)
                    {
                        Index++;

                        DText.Add(new TextBlock() {Width = 40}); // !!!
                        if (EnableText)
                        {
                            DText.Last().Text = Index.ToString();
                        } else
                        {
                            DText.Last().Text = "";
                        }

                        byte BT = Convert.ToByte(rnd.Next(50, 100));
                        itColor = Color.FromRgb(BT, BT, BT);

                        DText.Last().Background = new SolidColorBrush(itColor);
                        DText.Last().Foreground = new SolidColorBrush(Colors.Black); // setting the text color
                        // not use: DText.Last().Margin = new Thickness(3,3,3,3);
                        DText.Last().Padding = new Thickness(5, 5, 5, 5);
                        DText.Last().TextAlignment = TextAlignment.Center;


                        Grid.SetRow(DText.Last(), Row - 1);
                        Grid.SetColumn(DText.Last(), Col - 1);

                        myGrid.Children.Add(DText.Last());

                        int Val = Index;
                        if (Index < Numbers.Length)
                        {
                            Val = Convert.ToInt16(Numbers[Index-1]);
                        }
                        Num.Add(Val);
                    }
                }

                // Plane
                //OLD: var mesh = Work.BuildMesh("", Work.GetPointersRectangle(Pos, new Point3D(Height,Width,1)), "0;1;2 0;2;3", Colors.Aqua);
                var mesh = Work.BuildMesh("", Work.GetPointersBox2(Pos, new Point3D(Width, Height, Thickness)), "0;1;2 0;2;3 6;5;4 7;6;4 4;1;0 4;5;1 3;2;6 3;6;7 7;4;0 7;0;3 5;2;1 6;2;5", itColor);
                mesh.TextureCoordinates = new PointCollection(new Point[] {
                            new Point(0, 0),
                            new Point(0, 1),
                            new Point(1, 1),
                            new Point(1, 0),

                            new Point(0, 0),
                            new Point(0, 1),
                            new Point(1, 1),
                            new Point(1, 0),
                                                                       });

                Viewport.Geometry = mesh;
                var material = new DiffuseMaterial { Brush = Brushes.Gray };
                Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);
                Viewport.Material = material;

                // Center point
                forCenter = new Point3D(Pos.X + Width / 2, Pos.Y - Height / 2, Pos.Z + Thickness/2);

                // Rotate
                Viewport.Transform = new RotateTransform3D
                {
                    CenterX = Pos.X,
                    CenterY = Pos.Y,
                    CenterZ = Pos.Z,
                    Rotation = new AxisAngleRotation3D
                    {
                        Angle = itAngle,
                        Axis = Vector
                    }
                };

                // Visual
                Viewport.Visual = myGrid;
            }

        }
    }
}
