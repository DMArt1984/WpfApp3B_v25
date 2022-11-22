using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Wpf3Dapp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string tblGroups = "Groups";
        public string tblPanels = "Panels";
        public string tblInfo = "Info";

        DispatcherTimer timerColor;
        DispatcherTimer timerAnim;

        // Declare scene objects.
        Viewport3D myViewport3D = new Viewport3D();
        Model3DGroup myModel3DGroup = new Model3DGroup();
        
        ModelVisual3D myModelVisual3D = new ModelVisual3D();
        // Defines the camera used to view the 3D object. In order to view the 3D object,
        // the camera must be positioned and pointed such that the object is within view 
        // of the camera.
        PerspectiveCamera myPCamera = new PerspectiveCamera();

        Point3D forCamera = new Point3D(0, 0, 0);
        int plusX = -1;
        int plusY = -1;
        int plusZ = -1;
        bool PlusMinusX = false;
        bool PlusMinusY = false;
        bool PlusMinusZ = false;
        double angX=0;
        double angY=0;
        double angZ=0;

        Point mouseRoll = new Point(0, 0);

        GeometryModel3D[] MX = new GeometryModel3D[50];

        List<SimpleBoxPanel> SBA = new List<SimpleBoxPanel>();
        List<SimpleTextRectPanel> SRA = new List<SimpleTextRectPanel>();
        List<SimpleTitlePanel> STA = new List<SimpleTitlePanel>();

        //---

        public MainWindow()
        {
            InitializeComponent();
            

            // SQL
            MSSQLClient.SQLConnect = "user id=Админ;password=Xibs95r8#;server=localhost\\SQLEXPRESS;Trusted_Connection=yes;database=Box;connection timeout=30";

            // чтение настроек
            try
            {
                string text = File.ReadAllText("Setting.txt");
                string[] args = text.Split(Convert.ToChar(13));
                if (args.Length >= 1)
                {
                    MSSQLClient.SQLConnect = args[0];
                    //Console.WriteLine(args[0]);
                    if (args.Length >= 2)
                    {
                        tblGroups = args[1];
                        if (args.Length >= 3)
                        {
                            tblPanels = args[2];
                            if (args.Length >= 4)
                            {
                                tblInfo = args[3];
                                if (args.Length >= 5)
                                {
                                    string[] size = args[4].Split('-');
                                    if (size.Length==2)
                                    {
                                        this.Width = Convert.ToDouble(size[0]);
                                        this.Height = Convert.ToDouble(size[1]);
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения параметров");
            }

            //  установка таймера обновления цветов
            timerColor = new DispatcherTimer();
            timerColor.Tick += new EventHandler(timerColor_Tick);
            timerColor.Interval = new TimeSpan(0, 0, 1);
            timerColor.Start();

            //  установка таймера обновления анимации
            timerAnim = new DispatcherTimer();
            timerAnim.Tick += new EventHandler(timerAnim_Tick);
            timerAnim.Interval = new TimeSpan(0, 0, 0,0, 10);
            timerAnim.Start();

            

            //DirectionalLight myDirectionalLight = new DirectionalLight();
            //myDirectionalLight.Color = Colors.White;
            //myDirectionalLight.Direction = new Vector3D(-0.1, -0.5, -0.6);
            //myModel3DGroup.Children.Add(myDirectionalLight);

            //PointLight myPointLight = new PointLight();
            //myPointLight.Color = Colors.White;
            //myPointLight.Range = 5;
            //myPointLight.Position = new Point3D(0,1,0.5);
            //myModel3DGroup.Children.Add(myPointLight);


            //---
            setCombo();
            // аргументы командной строки
            int indexarg = 0;
            int NumCBox = 0;
            foreach (string s in System.Environment.GetCommandLineArgs())
            {
                indexarg++;
                if (indexarg == 2)
                {
                    try
                    {
                        NumCBox = Convert.ToInt32(s);
                        ComboGroups.SelectedIndex = NumCBox - 1;
                        this.Title = "3DViewer-" + (ComboGroups.SelectedIndex + 1).ToString();
                        ComboGroups.IsEnabled = false;
                        break;
                    }
                    catch
                    {

                    }
                }
            }


            //BuildObject(ComboGroups.SelectedIndex+1);

            // Add the group of models to the ModelVisual3d.
            //myModelVisual3D.Content = myModel3DGroup;

            // 
            //myViewport3D.Children.Add(myModelVisual3D);

            // Apply the viewport to the page so it will be rendered.
            //G2.Children.Add(myViewport3D);

        }

        private void timerColor_Tick(object sender, EventArgs e)
        {
            // Box
            for (var j=0;j<SBA.Count;j++)
            {
                var ID = SBA[j].ID;
                if (ID > 0)
                {
                    string STR = MSSQLClient.get_Receive("Select top 1 * from " + tblInfo + " where ID_panel=" + ID.ToString() + " order by ID desc",-1,' ');
                    string[] Rows = STR.Split(';');
                    if (Rows.Length > 0)
                    {
                        string[] Values = Rows.Last().Split(' ');
                        for (var i = 1; i < Values.Length; i++)
                        {
                            double DValue = Convert.ToDouble(Values[i]);
                            //...
                            for (var k = 0; k < SBA[j].Num.Count; k++)
                            {
                                if (SBA[j].Num[k] == i - 1)
                                {
                                    if (Values[i] == null || Values[i] == "")
                                    {
                                        SBA[j].Box[k].Material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
                                    } else
                                    {
                                        SBA[j].Box[k].Material = new DiffuseMaterial(new SolidColorBrush(Work.GetFillColor(DValue)));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Rectangle
            for (var j = 0; j < SRA.Count; j++)
            {
                var ID = SRA[j].ID;
                if (ID > 0)
                {
                    string STR = MSSQLClient.get_Receive("Select top 1 * from " + tblInfo + " where ID_panel=" + ID.ToString() + " order by ID desc", -1, ' ');
                    string[] Rows = STR.Split(';');
                    if (Rows.Length > 0)
                    {
                        string[] Values = Rows.Last().Split(' ');
                        for (var i = 1; i < Values.Length; i++)
                        {
                            
                            //...
                            for (var k = 0; k < SRA[j].Num.Count; k++)
                            {
                                if (SRA[j].Num[k] == i - 1)
                                {

                                    if ( Values[i] == null || Values[i] == "")
                                    {
                                        SRA[j].DText[k].Text = "";
                                        SRA[j].DText[k].Foreground = new SolidColorBrush(Colors.White); // setting the text color
                                        SRA[j].DText[k].Background = new SolidColorBrush(Colors.Gray);
                                    }
                                    else
                                    {
                                        double DValue = Convert.ToDouble(Values[i]);
                                        if (SRA[j].EnableText == true)
                                        {
                                            SRA[j].DText[k].Text = DValue.ToString();
                                        } else
                                        {
                                            SRA[j].DText[k].Text = "";
                                        }
                                        SRA[j].DText[k].Foreground = new SolidColorBrush(Colors.White); // setting the text color
                                        SRA[j].DText[k].Background = new SolidColorBrush(Work.GetFillColor(DValue));
                                    }
  
                                }
                            }
                            
                        }
                    }
                }
            }

        }

        private void timerAnim_Tick(object sender, EventArgs e)
        {
            bool aX = Convert.ToBoolean(animX.IsChecked);
            bool aY = Convert.ToBoolean(animY.IsChecked);
            bool aZ = Convert.ToBoolean(animZ.IsChecked);

            SliderX.IsEnabled = !aX;
            SliderY.IsEnabled = !aY;
            SliderZ.IsEnabled = !aZ;

            if (aX)
            {
                SliderX.Value += 0.5*plusX;
                if ((!PlusMinusX && SliderX.Value >= 360) || ((PlusMinusX && SliderX.Value >= angX+45) || SliderX.Value >= 360))
                {
                    if (PlusMinusX)
                    {
                        plusX = -plusX;
                    }
                    else
                    {
                        SliderX.Value = 1;
                    }
                }
                if ((!PlusMinusX && SliderX.Value <= 0) || ((PlusMinusX && SliderX.Value <= angX - 45) || SliderX.Value<=-360))
                {
                    if (PlusMinusX)
                    {
                        plusX = -plusX;
                    }
                    else
                    {
                        SliderX.Value = 359;
                    }
                }
            }
            if (aY)
            {
                SliderY.Value += 0.5*plusY;
                if ((!PlusMinusY && SliderY.Value >= 360) || ((PlusMinusY && SliderY.Value >= angY + 45) || SliderY.Value>=360))
                {
                    if (PlusMinusY)
                    {
                        plusY = -plusY;
                    }
                    else
                    {
                        SliderY.Value = 1;
                    }
                }
                if ((!PlusMinusY && SliderY.Value <= 0) || ((PlusMinusY && SliderY.Value <= angY - 45) || SliderY.Value <=-360))
                {
                    if (PlusMinusY)
                    {
                        plusY = -plusY;
                    }
                    else
                    {
                        SliderY.Value = 359;
                    }
                }
            }
            if (aZ)
            {
                SliderZ.Value += 0.5*plusZ;
                if ((!PlusMinusZ && SliderZ.Value >= 360) || ((PlusMinusZ && SliderZ.Value >= angZ + 45) || SliderZ.Value>=360))
                {
                    if (PlusMinusZ)
                    {
                        plusZ = -plusZ;
                    }
                    else
                    {
                        SliderZ.Value = 1;
                    }
                }
                if ((!PlusMinusZ && SliderZ.Value <= 0) || ((PlusMinusZ && SliderZ.Value <= angZ - 45) || SliderZ.Value<=-360))
                {
                    if (PlusMinusZ)
                    {
                        plusZ = -plusZ;
                    }
                    else
                    {
                        SliderZ.Value = 359;
                    }
                }
            }

            CamX();

        }

        private void setCombo()
        {
            ComboGroups.Items.Clear();

            string STR = MSSQLClient.get_Receive("Select * from " + tblGroups, -1, '|', ';');

            if (STR != "")
            {
                string[] Groups = STR.Split(';');
                for (var p = 0; p < Groups.Length; p++)
                {
                    string[] Params = Groups[p].Split('|');
                    ComboGroups.Items.Add(Params[1]);
                }

                if (ComboGroups.Items.Count > 0)
                    ComboGroups.SelectedIndex = 0;
            }
        }

        private void BuildObject(int Group)
        {
            // ---------------------------------------------------------
            myViewport3D = new Viewport3D();
            myModel3DGroup = new Model3DGroup();
            myModelVisual3D = new ModelVisual3D();
            myPCamera = new PerspectiveCamera();

            myPCamera.Position = new Point3D(0, -0.5, 4);

            myPCamera.LookDirection = new Vector3D(0, 0, -3.5);

            myPCamera.FieldOfView = 70;

            myViewport3D.Camera = myPCamera;

            AmbientLight myAmbientLight = new AmbientLight();
            myAmbientLight.Color = Colors.White;
            myModel3DGroup.Children.Add(myAmbientLight);


            // -------------------------------------------------
            forCamera = new Point3D(0, 0, 0);
            string STR = MSSQLClient.get_Receive("Select * from " + tblPanels + " where num_group=" + Group.ToString(), -1, '|', ';');
            string[] Panels = STR.Split(';');

            G2.Children.Clear();

            SBA.Clear();
            SRA.Clear();
            STA.Clear();

            if (STR != "")
            {

                for (var p = 0; p < Panels.Length; p++)
                {
                    string[] Params = Panels[p].Split('|');
                    var ID = Convert.ToInt16(Params[0]);
                    var type_panel = Convert.ToInt16(Params[2]);
                    var cells_Cols = Convert.ToInt16(Params[3]);
                    var cells_Rows = Convert.ToInt16(Params[4]);
                    var point_X = Convert.ToDouble(Params[5]);
                    var point_Y = Convert.ToDouble(Params[6]);
                    var point_Z = Convert.ToDouble(Params[7]);
                    var panel_Width = Convert.ToDouble(Params[8]);
                    var panel_Height = Convert.ToDouble(Params[9]);
                    var panel_Thickness = Convert.ToDouble(Params[10]);
                    var vector_X = Convert.ToDouble(Params[11]);
                    var vector_Y = Convert.ToDouble(Params[12]);
                    var vector_Z = Convert.ToDouble(Params[13]);
                    var vector_Angle = Convert.ToDouble(Params[14]);
                    var Title = Params[15];
                    string[] Numbers = Params[16].Split('-');

                    switch (type_panel)
                    {
                        case 0:
                            SBA.Add(new SimpleBoxPanel(cells_Cols, cells_Rows, new Point3D(point_X, point_Y, point_Z), panel_Width, panel_Height, panel_Thickness, new Vector3D(vector_X, vector_Y, vector_Z), vector_Angle, Numbers, Colors.Gray));
                            SBA.Last().ID = ID;
                            myModel3DGroup.Children.Add(SBA.Last().Panel);
                            forCamera = (p == 0) ? SBA.Last().forCenter : forCamera = forCamera; // Work.Center_fromTwo(forCamera, SBA.Last().forCenter);
                            break;
                        case 1: // таблица с текстом
                            SRA.Add(new SimpleTextRectPanel(cells_Cols, cells_Rows, new Point3D(point_X, point_Y, point_Z), panel_Width, panel_Height, panel_Thickness, new Vector3D(vector_X, vector_Y, vector_Z), vector_Angle, Numbers, Colors.Gray, true));
                            SRA.Last().ID = ID;
                            myViewport3D.Children.Add(SRA.Last().Viewport);
                            forCamera = (p == 0) ? SRA.Last().forCenter : forCamera = forCamera; // Work.Center_fromTwo(forCamera, SRA.Last().forCenter);
                            break;
                        case 2:
                            STA.Add(new SimpleTitlePanel(Title, new Point3D(point_X, point_Y, point_Z), panel_Width, panel_Height, panel_Thickness, new Vector3D(vector_X, vector_Y, vector_Z), vector_Angle));
                            myViewport3D.Children.Add(STA.Last().Viewport);
                            //forCamera = (p == 0) ? STA.Last().forCenter : forCamera = forCamera; // Work.Center_fromTwo(forCamera, STA.Last().forCenter);
                            break;
                        case 3: // таблица без текста
                            SRA.Add(new SimpleTextRectPanel(cells_Cols, cells_Rows, new Point3D(point_X, point_Y, point_Z), panel_Width, panel_Height, panel_Thickness, new Vector3D(vector_X, vector_Y, vector_Z), vector_Angle, Numbers, Colors.Gray, false));
                            SRA.Last().ID = ID;
                            myViewport3D.Children.Add(SRA.Last().Viewport);
                            forCamera = (p == 0) ? SRA.Last().forCenter : forCamera = forCamera; // Work.Center_fromTwo(forCamera, SRA.Last().forCenter);
                            break;

                    }

                }
                myPCamera.Position = new Point3D(forCamera.X, forCamera.Y, 8);
            }

            // Add the group of models to the ModelVisual3d.
            myModelVisual3D.Content = myModel3DGroup;

            myViewport3D.Children.Add(myModelVisual3D);

            // Apply the viewport to the page so it will be rendered.
            G2.Children.Add(myViewport3D);

            //========
            CamX();

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MX[0].Material = Work.setMaterial(Color.FromArgb(255, 255, 0, 0));
            //---
            //Random rnd = new Random(DateTime.Now.Millisecond);
            //SB.Box[0].Material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(rnd.Next(0, 255)), Convert.ToByte(rnd.Next(0, 255)), Convert.ToByte(rnd.Next(0, 255)))));
        }

        private void MySlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            CamX();

        }

        private void CamX()
        {
            var rX = SliderX.Value;
            var rY = SliderY.Value;
            var rZ = SliderZ.Value;

            var cX = SliderX1.Value;
            var cY = SliderY1.Value;
            var cZ = SliderZ1.Value;

            BarInfo.Content = $"положение ( {Math.Round(cX*10)/10} ; {Math.Round(cY*10)/10} ; {Math.Round(cZ*10)/10} )   поворот ( {Math.Round(rX)} ; {Math.Round(rY)} ; {Math.Round(rZ)} )";

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            RotateTransform3D RX = new RotateTransform3D
            {
                CenterX = forCamera.X,
                CenterY = forCamera.Y,
                CenterZ = forCamera.Z,
                Rotation = new AxisAngleRotation3D
                {
                    Angle = rX,
                    Axis = new Vector3D(1, 0, 0)
                }
            };
            myTransform3DGroup.Children.Add(RX);

            RotateTransform3D RY = new RotateTransform3D
            {
                CenterX = forCamera.X,
                CenterY = forCamera.Y,
                CenterZ = forCamera.Z,
                Rotation = new AxisAngleRotation3D
                {
                    Angle = rY,
                    Axis = new Vector3D(0, 1, 0)
                }
            };
            myTransform3DGroup.Children.Add(RY);

            RotateTransform3D RZ = new RotateTransform3D
            {
                CenterX = forCamera.X,
                CenterY = forCamera.Y,
                CenterZ = forCamera.Z,
                Rotation = new AxisAngleRotation3D
                {
                    Angle = rZ,
                    Axis = new Vector3D(0, 0, 1)
                }
            };
            myTransform3DGroup.Children.Add(RZ);

            myPCamera.Transform = myTransform3DGroup;

            //---
            myPCamera.Position = new Point3D(forCamera.X+cX, forCamera.Y+ cY, myPCamera.Position.Z);

            //---
            myPCamera.FieldOfView = cZ;
        }

        private void ComboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void ComboGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BuildObject(ComboGroups.SelectedIndex + 1);
                //this.Title = "3DViewer-"+(ComboGroups.SelectedIndex + 1).ToString();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка построения 3D объектов");
            }
        }

        private void animX_Click(object sender, RoutedEventArgs e)
        {
            plusX = -plusX;
            angX = SliderX.Value;
                PlusMinusX = !PlusMinusX;
        }

        private void animY_Click(object sender, RoutedEventArgs e)
        {
            plusY = -plusY;
            angY = SliderY.Value;
                PlusMinusY = !PlusMinusY;
        }

        private void animZ_Click(object sender, RoutedEventArgs e)
        {
            plusZ = -plusZ;
            angZ = SliderZ.Value;
                PlusMinusZ = !PlusMinusZ;
        }

        private void G2_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SliderZ1.Value += e.Delta*0.01;
        }

        private void G2_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // поворот
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pnt = e.GetPosition(this);
                SliderY.Value -= (pnt.X - mouseRoll.X)*0.2;
                SliderX.Value -= (pnt.Y - mouseRoll.Y)*0.2;
                mouseRoll = e.GetPosition(this);
            } else
            {
                // положение
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    Point pnt = e.GetPosition(this);
                    SliderX1.Value -= (pnt.X - mouseRoll.X) * 0.01;
                    SliderY1.Value += (pnt.Y - mouseRoll.Y) * 0.01;
                    mouseRoll = e.GetPosition(this);
                    //Console.WriteLine("right - " + SliderY1.Value.ToString() + " - " + ((pnt.X - mouseRoll.X) * 0.1).ToString());
                }
                else
                {
                    mouseRoll = e.GetPosition(this);
                }
            }

            



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                BuildObject(ComboGroups.SelectedIndex + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка построения 3D объектов");
            }
        }
    }
}
