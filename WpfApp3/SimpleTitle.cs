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
    // Title
    class SimpleTitlePanel
    {
        public TextBlock DText = new TextBlock();
        public Viewport2DVisual3D Viewport = new Viewport2DVisual3D();
        public Point3D forCenter = new Point3D(0, 0, 0);

        // creation
        public SimpleTitlePanel(string Title, Point3D Pos, double Width, double Height, double Thickness, Vector3D Vector, double itAngle)
        {
            Thickness = (Thickness == 0) ? Width * 0.02 : Thickness;

            Viewport = new Viewport2DVisual3D();

            // Table
            Grid myGrid = new Grid();
            myGrid.ShowGridLines = false;
            myGrid.VerticalAlignment = VerticalAlignment.Center;
            myGrid.HorizontalAlignment = HorizontalAlignment.Center;
            myGrid.Background = new SolidColorBrush(Colors.White);

            // Columns
            myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            // Rows
            myGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Cells for text
            DText = new TextBlock();
            DText.Text = Title;
            // not use: DText.FontWeight = FontWeights.Bold;
            DText.Background = new SolidColorBrush(Colors.LightYellow);
            DText.Foreground = new SolidColorBrush(Colors.Black); // setting the text color
            // not use: DText.Margin = new Thickness(3,3,3,3);
            DText.Padding = new Thickness(5, 5, 5, 5);
            // not use: DText.TextAlignment = TextAlignment.Center;
            DText.HorizontalAlignment = HorizontalAlignment.Center;
            DText.VerticalAlignment = VerticalAlignment.Center;

            // OLD: 
            //Grid.SetRow(DText, 0);
            //Grid.SetColumn(DText, 0);
            //myGrid.Children.Add(DText);
            //Console.WriteLine("X");

            // Plane
            var mesh = Work.BuildMesh("", Work.GetPointersBox2(Pos, new Point3D(Width, Height, Thickness)), "0;1;2 0;2;3 6;5;4 7;6;4", Colors.Aqua);
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

            Viewport.Geometry = mesh;
            var material = new DiffuseMaterial { Brush = Brushes.Green };
            Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);
            Viewport.Material = material;

            // Center point
            forCenter = new Point3D(Pos.X + Width / 2, Pos.Y + Height / 2, Pos.Z+ Thickness/2);

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
            Viewport.Visual = DText; // or myGrid;
        }

    }
}
