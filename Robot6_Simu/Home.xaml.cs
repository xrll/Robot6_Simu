using Robot6_Simu.Domain;
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
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shaps = System.Windows.Shapes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Drawing.Drawing2D;
using Utilities;
using Robot6_Simu.Controls;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO.MemoryMappedFiles;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;
using Xceed.Document.NET;
using System.Drawing;
using System.Numerics;
using static System.Net.WebRequestMethods;
using System.Net;
using Xceed.Words.NET;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Runtime.Intrinsics.X86;

namespace Robot6_Simu
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : System.Windows.Controls.UserControl
    {


        bool switchingJoint = false;
        public static bool isAnimating = false;

        GeometryModel3D oldSelectedModel = null;
        string basePath = "";
        ModelVisual3D visual;
        double LearningRate = 0.02;

        Transform3DGroup F1;
        Transform3DGroup F2;
        Transform3DGroup F3;
        Transform3DGroup F4;
        Transform3DGroup F5;
        Transform3DGroup F6;
        Transform3DGroup F7;
        RotateTransform3D R;
        TranslateTransform3D T;

        System.Windows.Forms.Timer timer;

        //      "T201_with_cover_v02", "V301_with_cover", "V302", "T401_with_cover", "V501_10L", "V502", "V5030" 
        Point3D center = new Point3D(); 

        public Home()
        {
            InitializeComponent();
            Loaded += Home_Loaded;
            viewPort3d.RotateGesture = new MouseGesture(MouseAction.RightClick);
            viewPort3d.PanGesture = new MouseGesture(MouseAction.LeftClick);


            //var builder = new MeshBuilder(true, true);
            //var position = new Point3D(0, 0, 0);
            //builder.AddSphere(position, 40, 20, 20);
            //geom = new GeometryModel3D(builder.ToMesh(), Materials.Brown);
            //visual = new ModelVisual3D();
            //visual.Content = geom;
            Initialize_Robot();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 15;
            timer.Tick += new System.EventHandler(timer_Tick);
            A1 = new JointClass6((float)AD[0][0], 0, 1, 0);
            A2 = new JointClass6((float)AD[1][0], 0, 2, 0);
            A3 = new JointClass6((float)AD[2][0], 0, 3, 0);
            A4 = new JointClass6(0, (float)AD[3][1], 4, 0);
            A5 = new JointClass6(0, 0, 5, 0);
            A6 = new JointClass6((float)AD[5][0], (float)AD[5][1], 6, 0);

            wps2 = new Point3DCollection();
            //for (int i = 0; i <= 360; i++)
            //{
            //    double a = i / 57.29578;
            //    wps2.Add(new Point3D(10 * Math.Cos(a), 10 * Math.Sin(a), 10));
            //}
            c30.Path = wps2;
            wct = Math.Cos(cAng);
            wst = Math.Sin(cAng);
            ct = 0.866;
            st = 0.5;
            //ota2 = new double[] { -st, 0, -ct };
            //otn2 = new double[] { 0, 1, 0 };
            //oto2 = new double[] { ct, 0, -st };
            ta2 = new double[] { -st*wct, -st*wst, -ct };
            tn2 = new double[] { -wst, wct, 0 };
            to2 = new double[] { ct * wct, ct * wst, -st };
            TPoint6 = new double[] { radius * wct + 600, radius * wst + 300, 405 };
            TPoint5 = new double[] { radius * wct + 600 - pL * ta2[0], radius * wst + 300 - pL * ta2[1], 405 - pL * ta2[2] };
            TAngle5 = JointClass6.InverseCal(tn2, to2, ta2, TPoint5, AD, null);
            TAngle6 = JointClass6.InverseCal(tn2, to2, ta2, TPoint6, AD, null);


            Matrix4x4 mx = new Matrix4x4(1, 0, 0, 0, 0, (float)ct, (float)st, 0, 0, -(float)st, (float)ct, 0, 0, 0, 0, 1);
            //Matrix4x4 mz = new Matrix4x4((float)ct, -(float)st, 0, 0, (float)st, (float)ct, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            Matrix4x4 m = mx;

            float Tz = MainWindowViewModel.Tz;
            float ntx = 600;
            float nty = -m.M22 * 100 - m.M23 * 30;
            float ntz = -m.M32 * 100 - m.M33 * 30 + Tz;
            float mty = -m.M22 * 700 - m.M23 * 30;
            float mtz = -m.M32 * 700 - m.M33 * 30 + Tz;

            tn1 = new double[] { 0.866, st * 0.5, ct * 0.5 };
            to1 = new double[] { 0, -ct, st };
            ta1 = new double[] { 0.5, -st * 0.866, -ct * 0.866 };
            TPoint4 = new double[] { ntx - ta1[0] * pL, nty - ta1[1] * pL, ntz - ta1[2] * pL };
            TPoint3 = new double[] { ntx, nty, ntz };
            TPoint2 = new double[] { ntx, mty, mtz };
            TPoint1 = new double[] { ntx - ta1[0] * pL, mty - ta1[1] * pL, mtz - ta1[2] * pL };
            wps1 = new Point3DCollection();
            wps1.Add(new Point3D(600, -700, 520));
            wps1.Add(new Point3D(600, -700, 520));
            c20.Path = wps1;


            TAngle1 = JointClass6.InverseCal(tn1, to1, ta1, TPoint1,AD,null);
            TAngle2 = JointClass6.InverseCal(tn1, to1, ta1, TPoint2, AD, null);
            TAngle3 = JointClass6.InverseCal(tn1, to1, ta1, TPoint3, AD, null);
            TAngle4 = JointClass6.InverseCal(tn1, to1, ta1, TPoint4, AD, null);

            lSPoint = new Point3D(ntx,  nty, ntz);
            lEPoint = new Point3D(ntx, mty, mtz);

        }
        Point3DCollection wps1, wps2;
        double lt = 0;
        double pL = 200, wL = 600;//预位、提枪高度
        double[] tn1, to1, ta1, tn2, to2, ta2,tp2, tn3, to3, ta3, tp3, ltn3, lto3, lta3, ltp3;
        double[] TPoint1, TPoint2, TPoint3, TPoint4, TPoint5, TPoint6;
        double[] TAngle1, TAngle2, TAngle3, TAngle4, TAngle5, TAngle6;
        Point3D lSPoint, lEPoint,cSpoint;
        double radius = 100, cAng = Math.Atan2(3, 6), wAng = 0,wct=0,wst=0,ct=0,st=0;
        int Process = 0;

        private void Initialize_Robot()
        {
            try
            {
               string basePath = AppDomain.CurrentDomain.BaseDirectory + "STL\\";
                ModelImporter import = new ModelImporter();
                int i = 0;
                foreach (string modelName in arr)
                {
                    //var materialGroup = new MaterialGroup();
                    //Color mainColor = Colors.White;
                    //EmissiveMaterial emissMat = new EmissiveMaterial(new SolidColorBrush(mainColor));
                    //DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(mainColor));
                    //SpecularMaterial specMat = new SpecularMaterial(new SolidColorBrush(mainColor), 200);
                    //materialGroup.Children.Add(emissMat);
                    //materialGroup.Children.Add(diffMat);
                    //materialGroup.Children.Add(specMat);
                    MaterialGroup materialGroup;
                    System.Windows.Media.Color DiffuseColor, SpecularColor, EmissiveColor;
                    if (i == 2 || i == 4 || i == 5)
                    {
                        materialGroup = new MaterialGroup();
                        DiffuseColor = ToColor(1.0, 0.964706, 0.0, 1.0);
                        SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0);
                        EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0);
                        materialGroup.Children.Add(new EmissiveMaterial(new SolidColorBrush(EmissiveColor)));
                        materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(DiffuseColor)));
                        materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(SpecularColor), 12.8));
                    }
                    else if (i == 7)
                    {
                        materialGroup = new MaterialGroup();
                        DiffuseColor = ToColor(0.5508, 0.2118, 0.066, 1.0);
                        SpecularColor = ToColor(0.580594, 0.223257, 0.0695701, 1.0);
                        EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0);
                        materialGroup.Children.Add(new EmissiveMaterial(new SolidColorBrush(EmissiveColor)));
                        materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(DiffuseColor)));
                        materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(SpecularColor), 51.2f));
                    }
                    else
                    {
                        materialGroup = new MaterialGroup();
                        DiffuseColor = ToColor(0.01, 0.01, 0.01, 1.0);
                        SpecularColor = ToColor(0.4, 0.4, 0.4, 1.0);
                        EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0);
                        materialGroup.Children.Add(new EmissiveMaterial(new SolidColorBrush(EmissiveColor)));
                        materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(DiffuseColor)));
                        materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(SpecularColor), 10));
                    }
                    var link = import.Load(basePath + modelName + ".stl");
                    GeometryModel3D model = link.Children[0] as GeometryModel3D;
                    model.Material = materialGroup;
                    model.BackMaterial = materialGroup;
                    if (i == 0)
                        MainWindowViewModel.R0 = model;
                    else if (i == 1)
                        MainWindowViewModel.R1 = model;
                    else if (i == 2)
                        MainWindowViewModel.R2 = model;
                    else if (i == 3)
                        MainWindowViewModel.R3 = model;
                    else if (i == 4)
                        MainWindowViewModel.R4 = model;
                    else if (i == 5)
                        MainWindowViewModel.R5 = model;
                    else if (i == 6)
                        MainWindowViewModel.R6 = model;
                    else if (i == 7)
                        MainWindowViewModel.R7 = model;
                    i++;
                }


                //RA.Children.Add(joints[0].model);
                //RA.Children.Add(joints[1].model);
                //RA.Children.Add(joints[2].model);
                //RA.Children.Add(joints[3].model);
                //RA.Children.Add(joints[4].model);
                //RA.Children.Add(joints[5].model);
                //RA.Children.Add(joints[6].model);

                //joints[0].angleMin = -180;
                //joints[0].angleMax = 180;
                //joints[0].rotAxisX = 0;
                //joints[0].rotAxisY = 0;
                //joints[0].rotAxisZ = 1;
                //joints[0].rotPointX = 0;
                //joints[0].rotPointY = 0;
                //joints[0].rotPointZ = 0;

                //joints[1].angleMin = -100;
                //joints[1].angleMax = 60;
                //joints[1].rotAxisX = 0;
                //joints[1].rotAxisY = 1;
                //joints[1].rotAxisZ = 0;
                //joints[1].rotPointX = 75;
                //joints[1].rotPointY = 0;
                //joints[1].rotPointZ = 450;

                //joints[2].angleMin = -90;
                //joints[2].angleMax = 90;
                //joints[2].rotAxisX = 0;
                //joints[2].rotAxisY = 1;
                //joints[2].rotAxisZ = 0;
                //joints[2].rotPointX = 75;
                //joints[2].rotPointY = 0;
                //joints[2].rotPointZ = 1090;

                //joints[3].angleMin = -180;
                //joints[3].angleMax = 180;
                //joints[3].rotAxisX = 1;
                //joints[3].rotAxisY = 0;
                //joints[3].rotAxisZ = 0;
                //joints[3].rotPointX = 0;
                //joints[3].rotPointY = 0;
                //joints[3].rotPointZ = 1285;

                //joints[4].angleMin = -115;
                //joints[4].angleMax = 115;
                //joints[4].rotAxisX = 0;
                //joints[4].rotAxisY = 1;
                //joints[4].rotAxisZ = 0;
                //joints[4].rotPointX = 975;
                //joints[4].rotPointY = 0;
                //joints[4].rotPointZ = 1285;

                //joints[5].angleMin = -180;
                //joints[5].angleMax = 180;
                //joints[5].rotAxisX = 1;
                //joints[5].rotAxisY = 0;
                //joints[5].rotAxisZ = 0;
                //joints[5].rotPointX = 0;
                //joints[5].rotPointY = 0;
                //joints[5].rotPointZ = 1285;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Exception Error:" + e.StackTrace);
            }
        }


        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            Window? window = Window.GetWindow(this);
            var mwv = this.DataContext as MainWindowViewModel;
            mwv!.mainW = this;
        }
        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        public void Simu_Click(object sender, RoutedEventArgs e)
        {
            if (timer.Enabled)
            {
                //button.Content = "Go to position";
                isAnimating = false;
                timer.Stop();
                V6.OnJointValueChanged += Rot_OnRotValueChanged;
                //mainPara.IsEnabled = true;
            }
            else
            {
                //V6.IsEnabled = false;
                //geom.Transform = new TranslateTransform3D(reachingPoint);
                //button.Content = "STOP";
                //mainPara.IsEnabled = false;
                V6.OnJointValueChanged -= Rot_OnRotValueChanged;
                if (Process == 0)
                {
                    if (wps1.Count > 1)
                        wps1[1] = wps1[0];
                    wps2.Clear();
                    fstp = true;
                }
                isAnimating = true;
                timer.Start();
            }
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            Move();
        }
        int c = 0;
        private void Move()
        {
            if (Process == 0)
            {
                MainWindowViewModel.JointAngles.CopyTo(CurAngles, 0);
                bool tog = false;
                TargetAngs = TAngle1;
                TargetN = tn1;
                TargetO = to1;
                TargetA = ta1;
                double[] angles = PGInverseKinematics(TPoint1, CurAngles, ref tog);
                //MainWindowViewModel.JointAngles = new ObservableCollection<double>(angles.ToList());
                ForwardKinematics(angles);
                MainWindowViewModel.JointAngles[0] = angles[0];
                MainWindowViewModel.JointAngles[1] = angles[1];
                MainWindowViewModel.JointAngles[2] = angles[2];
                MainWindowViewModel.JointAngles[3] = angles[3];
                MainWindowViewModel.JointAngles[4] = angles[4];
                MainWindowViewModel.JointAngles[5] = angles[5];
                if (tog)
                {
                    Process = 1;
                    lt = 0;
                }
            }
            else if (Process == 1)
            {
                lt += stp * 2;
                if (lt > pL)
                {
                    if (c == 40)
                    {
                        Process = 2;
                        lt = 0;
                        c = 0;
                    }
                    c++;
                }
                else
                {
                    MainWindowViewModel.Dxyz[0] = stp * ta1[0];
                    MainWindowViewModel.Dxyz[1] = stp * ta1[1];
                    MainWindowViewModel.Dxyz[2] = stp * ta1[2];
                    MainWindowViewModel.Dxyz[3] = 0;
                    MainWindowViewModel.Dxyz[4] = 0;
                    MainWindowViewModel.Dxyz[5] = 0;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { MainWindowViewModel.Dxyz[0], MainWindowViewModel.Dxyz[1], MainWindowViewModel.Dxyz[2], 0, 0, 0 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }
                    double[] target = new double[] { TPoint1[0] + lt * ta1[0], TPoint1[1] + lt * ta1[1], TPoint1[2] + lt * ta1[2] };
                    double[] angles = JointClass6.InverseCal(tn1, to1, ta1, target, AD, MainWindowViewModel.JointAngles.ToArray());
                    ForwardKinematics(angles);
                    MainWindowViewModel.JointAngles[0] = angles[0];
                    MainWindowViewModel.JointAngles[1] = angles[1];
                    MainWindowViewModel.JointAngles[2] = angles[2];
                    MainWindowViewModel.JointAngles[3] = angles[3];
                    MainWindowViewModel.JointAngles[4] = angles[4];
                    MainWindowViewModel.JointAngles[5] = angles[5];
                }
            }
            else if (Process == 2)
            {
                lt += stp;
                if (lt > wL)
                {
                    if (c == 40)
                    {
                        Process = 3;
                        lt = 0;
                        c = 0;
                    }
                    c++;
                }
                else
                {
                    MainWindowViewModel.Dxyz[0] = -stp * to1[0];
                    MainWindowViewModel.Dxyz[1] = -stp * to1[1];
                    MainWindowViewModel.Dxyz[2] = -stp * to1[2];
                    MainWindowViewModel.Dxyz[3] = 0;
                    MainWindowViewModel.Dxyz[4] = 0;
                    MainWindowViewModel.Dxyz[5] = 0;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { MainWindowViewModel.Dxyz[0], MainWindowViewModel.Dxyz[1], MainWindowViewModel.Dxyz[2], 0, 0, 0 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }
                    double[] target = new double[] { TPoint2[0] - lt * to1[0], TPoint2[1] - lt * to1[1], TPoint2[2] - lt * to1[2] };
                    wps1[1] = new Point3D(600, -700 + lt, 520);
                    double[] angles = JointClass6.InverseCal(tn1, to1, ta1, target, AD, MainWindowViewModel.JointAngles.ToArray());
                    ForwardKinematics(angles);
                    MainWindowViewModel.JointAngles[0] = angles[0];
                    MainWindowViewModel.JointAngles[1] = angles[1];
                    MainWindowViewModel.JointAngles[2] = angles[2];
                    MainWindowViewModel.JointAngles[3] = angles[3];
                    MainWindowViewModel.JointAngles[4] = angles[4];
                    MainWindowViewModel.JointAngles[5] = angles[5];
                }
            }
            else if (Process == 3)
            {
                lt += stp * 2;
                if (lt > pL / 2)
                {
                    Process = 4;
                    lt = 0;
                    fstp = true;
                }
                else
                {
                    MainWindowViewModel.Dxyz[0] = -stp * ta1[0];
                    MainWindowViewModel.Dxyz[1] = -stp * ta1[1];
                    MainWindowViewModel.Dxyz[2] = -stp * ta1[2];
                    MainWindowViewModel.Dxyz[3] = 0;
                    MainWindowViewModel.Dxyz[4] = 0;
                    MainWindowViewModel.Dxyz[5] = 0;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { MainWindowViewModel.Dxyz[0], MainWindowViewModel.Dxyz[1], MainWindowViewModel.Dxyz[2], 0, 0, 0 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }
                    double[] target = new double[] { TPoint3[0] - lt * ta1[0], TPoint3[1] - lt * ta1[1], TPoint3[2] - lt * ta1[2] };
                    double[] angles = JointClass6.InverseCal(tn1, to1, ta1, target, AD, MainWindowViewModel.JointAngles.ToArray());
                    ForwardKinematics(angles);
                    MainWindowViewModel.JointAngles[0] = angles[0];
                    MainWindowViewModel.JointAngles[1] = angles[1];
                    MainWindowViewModel.JointAngles[2] = angles[2];
                    MainWindowViewModel.JointAngles[3] = angles[3];
                    MainWindowViewModel.JointAngles[4] = angles[4];
                    MainWindowViewModel.JointAngles[5] = angles[5];
                }
            }
            else if (Process == 4)
            {
                MainWindowViewModel.JointAngles.CopyTo(CurAngles, 0);
                bool tog = false;
                TargetAngs = TAngle5;
                TargetN = tn2;
                TargetO = to2;
                TargetA = ta2;
                double[] angles = PGInverseKinematics(TPoint5, CurAngles, ref tog);
                ForwardKinematics(angles);
                MainWindowViewModel.JointAngles[0] = angles[0];
                MainWindowViewModel.JointAngles[1] = angles[1];
                MainWindowViewModel.JointAngles[2] = angles[2];
                MainWindowViewModel.JointAngles[3] = angles[3];
                MainWindowViewModel.JointAngles[4] = angles[4];
                MainWindowViewModel.JointAngles[5] = angles[5];
                if (tog)
                {
                    Process = 5;
                    lt = 0;
                }
            }
            else if (Process == 5)
            {
                lt += stp * 2;
                if (lt > pL)
                {
                    if (c == 40)
                    {
                        Process = 6;
                        lt = 0;
                        wAng = 0;
                        c = 0;
                    }
                    c++;
                }
                else
                {
                    MainWindowViewModel.Dxyz[0] = stp * ta2[0];
                    MainWindowViewModel.Dxyz[1] = stp * ta2[1];
                    MainWindowViewModel.Dxyz[2] = stp * ta2[2];
                    MainWindowViewModel.Dxyz[3] = 0;
                    MainWindowViewModel.Dxyz[4] = 0;
                    MainWindowViewModel.Dxyz[5] = 0;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { MainWindowViewModel.Dxyz[0], MainWindowViewModel.Dxyz[1], MainWindowViewModel.Dxyz[2], 0, 0, 0 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }

                    double[] target = new double[] { TPoint5[0] + lt * ta2[0], TPoint5[1] + lt * ta2[1], TPoint5[2] + lt * ta2[2] };
                    double[] angles = JointClass6.InverseCal(tn2, to2, ta2, target, AD, MainWindowViewModel.JointAngles.ToArray());
                    ForwardKinematics(angles);
                    MainWindowViewModel.JointAngles[0] = angles[0];
                    MainWindowViewModel.JointAngles[1] = angles[1];
                    MainWindowViewModel.JointAngles[2] = angles[2];
                    MainWindowViewModel.JointAngles[3] = angles[3];
                    MainWindowViewModel.JointAngles[4] = angles[4];
                    MainWindowViewModel.JointAngles[5] = angles[5];
                }
            }
            else if (Process == 6)
            {
                double cct = Math.Cos(cAng - wAng);
                double sct = Math.Sin(cAng - wAng);
                ta3 = new double[] { -st * cct, -st * sct, -ct };
                tp3 = new double[] { cct * radius + 600, sct * radius + 300, 405 };
                if (wAng < Math.PI / 2)
                {
                    tn3 = new double[] { -sct, cct, 0 };
                    to3 = new double[] { ct * cct, ct * sct, -st };
                }
                else if (wAng < Math.PI * 1.5)
                {
                    double ca = Math.Cos(wAng - Math.PI * 0.5);
                    double sa = Math.Sin(wAng - Math.PI * 0.5);

                    double[] tn = new double[] { ca, ct * sa, st * sa };
                    double[] to = new double[] { sa, -ct * ca, -st * ca };

                    double[] tn1 = new double[] { ca * ca + ct * sa * sa, -ca * sa + ct * sa * ca, st * sa };
                    double[] to1 = new double[] { ca * sa - ct * ca * sa, -sa * sa - ct * ca * ca, -st * ca };

                    tn3 = new double[] { tn1[0] * wct - tn1[1] * wst, tn1[0] * wst + tn1[1] * wct, st * sa };
                    to3 = new double[] { to1[0] * wct - to1[1] * wst, to1[0] * wst + to1[1] * wct, -st * ca };
                }
                else
                {
                    tn3 = new double[] { sct, -cct, 0 };
                    to3 = new double[] { -ct * cct, -ct * sct, st };
                }
                if(ltn3!=null&&lto3!=null&&lta3!=null)
                {
                    double d1 = tp3[0] - ltp3[0];
                    double d2 = tp3[1] - ltp3[1];
                    double d3 = tp3[2] - ltp3[2];

                    double d4 = tn3[2] * ltn3[1] + to3[2] * lto3[1] + ta3[2] * lta3[1];//𝛿_𝑥=o1 x a2  r3-c2
                    double d5 = tn3[0] * ltn3[2] + to3[0] * lto3[2] + ta3[0] * lta3[2];//𝛿_𝑦 a1 x n2  r1-c3
                    double d6 = tn3[1] * ltn3[0] + to3[1] * lto3[0] + ta3[1] * lta3[0];//𝛿_𝑧 n1 x o2  r2-c1

                    MainWindowViewModel.Dxyz[0] = d1;
                    MainWindowViewModel.Dxyz[1] = d2;
                    MainWindowViewModel.Dxyz[2] = d3;
                    MainWindowViewModel.Dxyz[3] = d4;
                    MainWindowViewModel.Dxyz[4] = d5;
                    MainWindowViewModel.Dxyz[5] = d6;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { d1, d2, d3, d4, d5, d6 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }
                }
                wps2.Add(new Point3D(cct * radius, sct * radius, 405));
                double[] angles = JointClass6.InverseCal(tn3, to3, ta3, tp3, AD, MainWindowViewModel.JointAngles.ToArray());
                ltn3 = tn3;
                lto3 = to3;
                lta3 = ta3;
                ltp3 = tp3;
                ForwardKinematics(angles);
                MainWindowViewModel.JointAngles[0] = angles[0];
                MainWindowViewModel.JointAngles[1] = angles[1];
                MainWindowViewModel.JointAngles[2] = angles[2];
                MainWindowViewModel.JointAngles[3] = angles[3];
                MainWindowViewModel.JointAngles[4] = angles[4];
                MainWindowViewModel.JointAngles[5] = angles[5];
                if (Math.Abs(wAng - Math.PI * 2) < 1e-4)
                {
                    if (c == 40)
                    {
                        Process = 7;
                        lt = 0;
                        c = 0;
                    }
                    c++;
                }
                wAng += 0.8 * stp / radius;
                if (wAng > 2 * Math.PI)
                    wAng = 2 * Math.PI;
            }
            else if (Process == 7)
            {
                lt += stp * 2;
                if (lt > pL)
                {
                    if (MainWindowViewModel.JointAngles[1] < -30)
                    {
                        isAnimating = false;
                        timer.Stop();
                        V6.OnJointValueChanged += Rot_OnRotValueChanged;
                        Process = 0;
                        lt = 0;
                        wAng = 0;
                    }
                    MainWindowViewModel.JointAngles[1] -= 0.2;
                    double[] angles = new double[6];
                    MainWindowViewModel.JointAngles.CopyTo(angles, 0);
                    ForwardKinematics(angles);
                }
                else
                {
                    MainWindowViewModel.Dxyz[0] = -stp * ta2[0];
                    MainWindowViewModel.Dxyz[1] = -stp * ta2[1];
                    MainWindowViewModel.Dxyz[2] = -stp * ta2[2];
                    MainWindowViewModel.Dxyz[3] = 0;
                    MainWindowViewModel.Dxyz[4] = 0;
                    MainWindowViewModel.Dxyz[5] = 0;
                    if (MainWindowViewModel.Jacobian[36] != 0)
                    {
                        double[] diff = JointClass6.Differential(J, new double[] { MainWindowViewModel.Dxyz[0], MainWindowViewModel.Dxyz[1], MainWindowViewModel.Dxyz[2], 0, 0, 0 });
                        MainWindowViewModel.DAngle[0] = diff[0] * 57.29578;
                        MainWindowViewModel.DAngle[1] = diff[1] * 57.29578;
                        MainWindowViewModel.DAngle[2] = diff[2] * 57.29578;
                        MainWindowViewModel.DAngle[3] = diff[3] * 57.29578;
                        MainWindowViewModel.DAngle[4] = diff[4] * 57.29578;
                        MainWindowViewModel.DAngle[5] = diff[5] * 57.29578;
                    }
                    double[] target = new double[] { TPoint6[0] - lt * ta2[0], TPoint6[1] - lt * ta2[1], TPoint6[2] - lt * ta2[2] };
                    double[] angles = JointClass6.InverseCal(tn3, to3, ta3, target, AD, MainWindowViewModel.JointAngles.ToArray());
                    ForwardKinematics(angles);
                    MainWindowViewModel.JointAngles[0] = angles[0];
                    MainWindowViewModel.JointAngles[1] = angles[1];
                    MainWindowViewModel.JointAngles[2] = angles[2];
                    MainWindowViewModel.JointAngles[3] = angles[3];
                    MainWindowViewModel.JointAngles[4] = angles[4];
                    MainWindowViewModel.JointAngles[5] = angles[5];
                }
            }
        }
        double[] TargetAngs, TargetN, TargetO, TargetA;
        double SamplingDistance = 0.02;
        double DistanceThreshold = 5.00;
        double stp = 0.5;
        bool fstp = true;
        double vstp = 0.2;
        public double[] PGInverseKinematics(double[] target, double[] angles,ref bool tog)
        {
            double dpv=0,dnv = 0, dav = 0, dov = 0, nnx = 0, nny = 0, nnz = 0, nax = 0, nay = 0, naz = 0, nox = 0, noy = 0, noz = 0, drx = 0, dry = 0, drz = 0;
            double dpx=0,dpy=0,dpz=0,dnx = 0, dny = 0, dnz = 0, dax = 0, day = 0, daz = 0, dox = 0, doy = 0, doz = 0;
            double[] oldAngles = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            angles.CopyTo(oldAngles, 0);
            double[] diff = new double[6];
            dpv = DistanceFromTarget(target, angles);
            if (dpv < DistanceThreshold || fstp == false)
            {
                fstp = false;
            }
            if (!fstp)
            {
                dpx = target[0] - r6p[0];
                dpy = target[1] - r6p[1];
                dpz = target[2] - r6p[2];
                dnx = TargetN[0] - r6n[0];
                dny = TargetN[1] - r6n[1];
                dnz = TargetN[2] - r6n[2];
                dax = TargetA[0] - r6a[0];
                day = TargetA[1] - r6a[1];
                daz = TargetA[2] - r6a[2];
                dox = TargetO[0] - r6o[0];
                doy = TargetO[1] - r6o[1];
                doz = TargetO[2] - r6o[2];

                //dpv = Math.Sqrt(dpx * dpx + dpy * dpy + dpz * dpz);
                dnv = Math.Sqrt(dnx * dnx + dny * dny + dnz * dnz);
                dav = Math.Sqrt(dax * dax + day * day + daz * daz);
                dov = Math.Sqrt(dox * dox + doy * doy + doz * doz);

                if (dpv > stp)
                {
                    dpx /= dpv / stp;
                    dpy /= dpv / stp;
                    dpz /= dpv / stp;
                }
                else if (dpv > stp/2)
                {
                    dpx /= dpv / stp * 2.0;
                    dpy /= dpv / stp * 2.0;
                    dpz /= dpv / stp * 2.0;
                }
                if (dnv > vstp)
                {
                    dnx /= dnv / vstp;
                    dny /= dnv / vstp;
                    dnz /= dnv / vstp;
                }
                else if (dnv > vstp / 2)
                {
                    dnx /= dnv / vstp * 2.0;
                    dny /= dnv / vstp * 2.0;
                    dnz /= dnv / vstp * 2.0;
                }
                if (dav > vstp)
                {
                    dax /= dav / vstp;
                    day /= dav / vstp;
                    daz /= dav / vstp;
                }
                else if (dav > vstp / 2)
                {
                    dax /= dav / vstp * 2.0;
                    day /= dav / vstp * 2.0;
                    daz /= dav / vstp * 2.0;
                }
                if (dov > vstp)
                {
                    dox /= dov / vstp;
                    doy /= dov / vstp;
                    doz /= dov / vstp;
                }
                else if (dov > vstp / 2.0)
                {
                    dox /= dov / vstp * 2.0;
                    doy /= dov / vstp * 2.0;
                    doz /= dov / vstp * 2.0;
                }
                if (dnv > vstp/2 || dav > vstp/2 || dov > vstp / 2.0)
                {
                    drx = dnz * r6n[1] + doz * r6o[1] + daz * r6a[1];
                    dry = dnx * r6n[2] + dox * r6o[2] + dax * r6a[2];
                    drz = dny * r6n[0] + doy * r6o[0] + day * r6a[0];
                    diff = JointClass6.Differential(J, new double[] { dpx, dpy, dpz, drx, dry, drz });
                }
            }
            for (int i = 0; i <= 5; i++)
            {
                double da = (TargetAngs[i] - angles[i]);
                if (da == 0)
                    continue;

                // Gradient Descent
                double gradient = PartialGradient(target, angles, i);
                if (fstp)
                    angles[i] -= LearningRate * gradient;
                else if (dnv > vstp/2 || dav > vstp/2.0 || dov > vstp / 2.0)
                {
                    angles[i] += diff[i];
                }
                else
                {
                    if ((i < 3 && Math.Abs(da) < 0.02) || (i > 2 && Math.Abs(da) < 0.1))
                        angles[i] = TargetAngs[i];
                    else
                    {
                        double dda = LearningRate * da;
                        if (i < 3)
                            dda = Math.Abs(dda) > 0.08 ? 0.08 * Math.Sign(dda) : dda;
                        else if (i == 4)
                            dda = Math.Abs(dda) > 0.1 ? 0.1 * Math.Sign(dda) : dda;
                        else
                            dda = Math.Abs(dda) > 0.1 ? 0.1 * Math.Sign(dda) : dda;
                        angles[i] += dda;
                    }
                }
            }
            if ((DistanceFromTarget(target, angles) < DistanceThreshold && IsTarget(TargetAngs, angles)) || checkAngles(oldAngles, angles))
            {
                tog = true;
                return angles;
            }
            return angles;           
        }
        public bool checkAngles(double[] oldAngles, double[] angles)
        {
            for (int i = 0; i <= 5; i++)
            {
                if (oldAngles[i] != angles[i])
                    return false;
            }

            return true;
        }
        public bool IsTarget(double[] tAngles, double[] angles)
        {
            for (int i = 0; i <= 5; i++)
            {
                if (Math.Abs(tAngles[i] - angles[i]) > 0.01)
                    return false;
            }

            return true;
        }

        public double PartialGradient(double[] target, double[] angles, int i)
        {
            double angle = angles[i];
            double f_x = DistanceFromTarget(target, angles);

            angles[i] += SamplingDistance;
            double f_x_plus_d = DistanceFromTarget(target, angles);

            double gradient = (f_x_plus_d - f_x) / SamplingDistance;

            angles[i] = angle;
            return gradient;
        }


        public double DistanceFromTarget(double[] target, double[] angles)
        {
            double[] point = JointClass6.R6P(AD, new double[] { angles[0]/57.29578, angles[1] / 57.29578, angles[2] / 57.29578, angles[3] / 57.29578, angles[4] / 57.29578, angles[5] / 57.29578, });
//            double[] point = ForwardKinematics(angles);
            return Math.Sqrt(Math.Pow((point[0] - target[0]), 2.0) + Math.Pow((point[1] - target[1]), 2.0) + Math.Pow((point[2] - target[2]), 2.0));
        }
        private void rPara_OnJointValueChanged()
        {
            if (isAnimating)
                return;
        }
        JointClass6 A1, A2, A3, A4, A5, A6;
        object obj = new object();

        string[] arr = { "T201_with_cover_v02", "V301_with_cover", "V302", "T401_with_cover", "V501_10L", "V502", "V5033", "T103" };
        public double[] ForwardKinematics(double[] angles)
        {
            lock (obj)
            {
                F1 = new Transform3DGroup();
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angles[0]), new Point3D(0, 0, 0));
                F1.Children.Add(R);

                F2 = new Transform3DGroup();
                T = new TranslateTransform3D(0, 0, 0);
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angles[1]), MainWindowViewModel.J21);
                F2.Children.Add(T);
                F2.Children.Add(R);
                F2.Children.Add(F1);

                F3 = new Transform3DGroup();
                T = new TranslateTransform3D(0, 0, 0);
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angles[2]), MainWindowViewModel.J31);
                F3.Children.Add(T);
                F3.Children.Add(R);
                F3.Children.Add(F2);

                //as before
                F4 = new Transform3DGroup();
                T = new TranslateTransform3D(0, 0, 0); //1500, 650, 1650
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[3]), MainWindowViewModel.J41);
                F4.Children.Add(T);
                F4.Children.Add(R);
                F4.Children.Add(F3);

                F5 = new Transform3DGroup();
                T = new TranslateTransform3D(0, 0, 0);
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angles[4]), MainWindowViewModel.J51);
                F5.Children.Add(T);
                F5.Children.Add(R);
                F5.Children.Add(F4);

                F6 = new Transform3DGroup();
                T = new TranslateTransform3D(0, 0, 0);
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[5]), MainWindowViewModel.J61);
                F6.Children.Add(T);
                F6.Children.Add(R);
                F6.Children.Add(F5);

                F7 = new Transform3DGroup();
                T = new TranslateTransform3D(-60, 0, 0);
                R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angles[5]), MainWindowViewModel.J61);
                F7.Children.Add(T);
                F7.Children.Add(R);
                F7.Children.Add(F5);

                r1.Transform = F1;
                r2.Transform = F2;
                r3.Transform = F3;
                r4.Transform = F4;
                //v45.Transform = F4;
                r5.Transform = F5;
                r6.Transform = F6;
                r7.Transform = F7;


                n6.Transform = F6;
                o6.Transform = F6;
                a6.Transform = F6;
                v7.Transform = F6;

                double[] Ang = new double[6] { angles[0] / 57.29578, angles[1] / 57.29578, angles[2] / 57.29578, angles[3] / 57.29578, angles[4] / 57.29578, angles[5] / 57.29578 };
                r6p = JointClass6.R6P(AD, Ang);
                MainWindowViewModel.EndP[0] = r6p[0];
                MainWindowViewModel.EndP[1] = r6p[1];
                MainWindowViewModel.EndP[2] = r6p[2];
                r6n = JointClass6.R6N(Ang);
                MainWindowViewModel.EndN[0] = r6n[0];
                MainWindowViewModel.EndN[1] = r6n[1];
                MainWindowViewModel.EndN[2] = r6n[2];
                r6o = JointClass6.R6O(Ang);
                MainWindowViewModel.EndO[0] = r6o[0];
                MainWindowViewModel.EndO[1] = r6o[1];
                MainWindowViewModel.EndO[2] = r6o[2];
                r6a = JointClass6.R6A(Ang);
                MainWindowViewModel.EndA[0] = r6a[0];
                MainWindowViewModel.EndA[1] = r6a[1];
                MainWindowViewModel.EndA[2] = r6a[2];

                r5p = JointClass6.R5P(AD, Ang);
                MainWindowViewModel.J5P[0] = r5p[0];
                MainWindowViewModel.J5P[1] = r5p[1];
                MainWindowViewModel.J5P[2] = r5p[2];
                r5n = JointClass6.R5N(Ang);
                MainWindowViewModel.J5N[0] = r5n[0];
                MainWindowViewModel.J5N[1] = r5n[1];
                MainWindowViewModel.J5N[2] = r5n[2];
                r5o = JointClass6.R5O(Ang);
                MainWindowViewModel.J5O[0] = r5o[0];
                MainWindowViewModel.J5O[1] = r5o[1];
                MainWindowViewModel.J5O[2] = r5o[2];
                r5a = JointClass6.R5A(Ang);
                MainWindowViewModel.J5A[0] = r5a[0];
                MainWindowViewModel.J5A[1] = r5a[1];
                MainWindowViewModel.J5A[2] = r5a[2];


                r4p = JointClass6.R4P(AD, Ang);
                MainWindowViewModel.J4P[0] = r4p[0];
                MainWindowViewModel.J4P[1] = r4p[1];
                MainWindowViewModel.J4P[2] = r4p[2];
                r4n = JointClass6.R4N(Ang);
                MainWindowViewModel.J4N[0] = r4n[0];
                MainWindowViewModel.J4N[1] = r4n[1];
                MainWindowViewModel.J4N[2] = r4n[2];
                r4o = JointClass6.R4O(Ang);
                MainWindowViewModel.J4O[0] = r4o[0];
                MainWindowViewModel.J4O[1] = r4o[1];
                MainWindowViewModel.J4O[2] = r4o[2];
                r4a = JointClass6.R4A(Ang);
                MainWindowViewModel.J4A[0] = r4a[0];
                MainWindowViewModel.J4A[1] = r4a[1];
                MainWindowViewModel.J4A[2] = r4a[2];

                r3p = JointClass6.R3P(AD, Ang);
                MainWindowViewModel.J3P[0] = r3p[0];
                MainWindowViewModel.J3P[1] = r3p[1];
                MainWindowViewModel.J3P[2] = r3p[2];
                r3n = JointClass6.R3N(Ang);
                MainWindowViewModel.J3N[0] = r3n[0];
                MainWindowViewModel.J3N[1] = r3n[1];
                MainWindowViewModel.J3N[2] = r3n[2];
                r3o = JointClass6.R3O(Ang);
                MainWindowViewModel.J3O[0] = r3o[0];
                MainWindowViewModel.J3O[1] = r3o[1];
                MainWindowViewModel.J3O[2] = r3o[2];
                r3a = JointClass6.R3A(Ang);
                MainWindowViewModel.J3A[0] = r3a[0];
                MainWindowViewModel.J3A[1] = r3a[1];
                MainWindowViewModel.J3A[2] = r3a[2];

                r2p = JointClass6.R2P(AD, Ang);
                MainWindowViewModel.J2P[0] = r2p[0];
                MainWindowViewModel.J2P[1] = r2p[1];
                MainWindowViewModel.J2P[2] = r2p[2];
                r2n = JointClass6.R2N(Ang);
                MainWindowViewModel.J2N[0] = r2n[0];
                MainWindowViewModel.J2N[1] = r2n[1];
                MainWindowViewModel.J2N[2] = r2n[2];
                r2o = JointClass6.R2O(Ang);
                MainWindowViewModel.J2O[0] = r2o[0];
                MainWindowViewModel.J2O[1] = r2o[1];
                MainWindowViewModel.J2O[2] = r2o[2];
                r2a = JointClass6.R2A(Ang);
                MainWindowViewModel.J2A[0] = r2a[0];
                MainWindowViewModel.J2A[1] = r2a[1];
                MainWindowViewModel.J2A[2] = r2a[2];

                r1p = JointClass6.R1P(AD, Ang);
                MainWindowViewModel.J1P[0] = r1p[0];
                MainWindowViewModel.J1P[1] = r1p[1];
                MainWindowViewModel.J1P[2] = r1p[2];
                r1n = JointClass6.R1N(Ang);
                MainWindowViewModel.J1N[0] = r1n[0];
                MainWindowViewModel.J1N[1] = r1n[1];
                MainWindowViewModel.J1N[2] = r1n[2];
                r1o = JointClass6.R1O(Ang);
                MainWindowViewModel.J1O[0] = r1o[0];
                MainWindowViewModel.J1O[1] = r1o[1];
                MainWindowViewModel.J1O[2] = r1o[2];
                r1a = JointClass6.R1A(Ang);
                MainWindowViewModel.J1A[0] = r1a[0];
                MainWindowViewModel.J1A[1] = r1a[1];
                MainWindowViewModel.J1A[2] = r1a[2];

                iangs = JointClass6.InverseCal(r6n, r6o, r6a, r6p, AD, iangs);
                MainWindowViewModel.IJointAngles[0] = iangs[0];
                MainWindowViewModel.IJointAngles[1] = iangs[1];
                MainWindowViewModel.IJointAngles[2] = iangs[2];
                MainWindowViewModel.IJointAngles[3] = iangs[3];
                MainWindowViewModel.IJointAngles[4] = iangs[4];
                MainWindowViewModel.IJointAngles[5] = iangs[5];
                J = JointClass6.Jacobian(AD, Ang);
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < 6; j++)
                        MainWindowViewModel.Jacobian[i * 6 + j] = J[i, j];
                MainWindowViewModel.Jacobian[36] = JointClass6.Det(J);
                A1.CurAngle= Ang[0];
                A2.CurAngle = Ang[1];
                A3.CurAngle = Ang[2];
                A4.CurAngle = Ang[3];
                A5.CurAngle = Ang[4];
                A6.CurAngle = Ang[5];

                Matrix4x4 t1 = A1.T;
                Matrix4x4 t2 = t1 * A2.T;
                Matrix4x4 t3 = t2 * A3.T;
                Matrix4x4 t4 = t3 * A4.T;
                Matrix4x4 t5 = t4 * A5.T;
                Matrix4x4 t6 = t5 * A6.T;

                //float x = 300;
                //float y = 400;
                //float z = 500;

                //double rotX = 30;
                //double rotY = 45;
                //double rotZ = 150;

                //float cx = (float)Math.Cos(rotX / 57.29578);
                //float sx = (float)Math.Sin(rotX / 57.29578);
                //float cy = (float)Math.Cos(rotY / 57.29578);
                //float sy = (float)Math.Sin(rotY / 57.29578);
                //float cz = (float)Math.Cos(rotZ / 57.29578);
                //float sz = (float)Math.Sin(rotZ / 57.29578);

                //float px = 30;
                //float py = 200;
                //float pz = 20;
                //float nx = -0.7071f;
                //float ny = 0;
                //float nz = 0.7071f;
                //float ox = 0;
                //float oy = 1;
                //float oz = 0;
                //float ax = -0.7071f;
                //float ay = 0;
                //float az = -0.7071f;

                //Matrix4x4 rx = new Matrix4x4(1, 0, 0, 0, 0, cx, -sx, 0, 0, sx, cx, 0, 0, 0, 0, 1);
                //Matrix4x4 ry = new Matrix4x4(cy, 0, sy, 0, 0, 1, 0, 0, -sy, 0, cy, 0, 0, 0, 0, 1);
                //Matrix4x4 rz = new Matrix4x4(cz, -sz, 0, 0, sz, cz, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
                //Matrix4x4 txyz = new Matrix4x4(1, 0, 0, x, 0, 1, 0, y, 0, 0, 1, z, 0, 0, 0, 1);
                //Matrix4x4 m4 = txyz * rz * ry * rx;

                //double dx = m4.M11 * px + m4.M12 * py + m4.M13 * pz+m4.M14;
                //double dy = m4.M21 * px + m4.M22 * py + m4.M23 * pz + m4.M24;
                //double dz = m4.M31 * px + m4.M32 * py + m4.M33 * pz + m4.M34;

                //rx = new Matrix4x4(1, 0, 0, 0, 0, cx, sx, 0, 0, -sx, cx, 0, 0, 0, 0, 1);
                //ry = new Matrix4x4(cy, 0, -sy, 0, 0, 1, 0, 0, sy, 0, cy, 0, 0, 0, 0, 1);
                //rz = new Matrix4x4(cz, sz, 0, 0, -sz, cz, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
                //txyz = new Matrix4x4(1, 0, 0, -x, 0, 1, 0, -y, 0, 0, 1, -z, 0, 0, 0, 1);

                //m4 = rx * ry * rz * txyz;

                //double fx = m4.M11 * dx + m4.M12 * dy + m4.M13 * dz + m4.M14;
                //double fy = m4.M21 * dx + m4.M22 * dy + m4.M23 * dz + m4.M24;
                //double fz = m4.M31 * dx + m4.M32 * dy + m4.M33 * dz + m4.M34;


                //double[] tmpM = { 1, 0.5, 1, 0.5, 0.5, 1 };

                //double[] euler = JointClass6.VectorToEuler(r6n, r6o, r6a);
                ////MainWindowViewModel.EndEAng[0] = euler[0];
                ////MainWindowViewModel.EndEAng[1] = euler[1];
                ////MainWindowViewModel.EndEAng[2] = euler[2];
                //double[,] matrix = JointClass6.EulerToVector(euler);
            }
            return r6p;
        }
        double[] r6n, r6o, r6a, r6p, r5n, r5o, r5a, r5p, r4n, r4o, r4a, r4p, r3n, r3o, r3a, r3p, r2n, r2o, r2a, r2p, r1n, r1o, r1a, r1p, iangs;
        double[,] J;

        double[] CurAngles = new double[6];
        private void Rot_OnRotValueChanged()
        {
            MainWindowViewModel.JointAngles.CopyTo(CurAngles, 0);
            ForwardKinematics(CurAngles);
        }
        public double[][] AD => new double[6][] { new double[2] { MainWindowViewModel.JointLength[1], MainWindowViewModel.JointLength[0] }, new double[2] { MainWindowViewModel.JointLength[2], 0 }, new double[2] { MainWindowViewModel.JointLength[3], 0 }, new double[2] { 0, MainWindowViewModel.JointLength[4] }, new double[2] { 0, 0 }, new double[2] { MainWindowViewModel.JointLength[6], MainWindowViewModel.JointLength[5]+ MainWindowViewModel.JointLength[7] } };
        public static System.Windows.Media.Color ToColor(double r, double g, double b, double a = 1.0)
        {
            //return new Color4((float)r, (float)g, (float)b, (float)a);
            return System.Windows.Media.Color.FromScRgb((float)a, (float)r, (float)g, (float)b);
        }
    }
}
