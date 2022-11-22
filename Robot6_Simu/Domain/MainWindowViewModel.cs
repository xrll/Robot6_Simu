using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Media3D=System.Windows.Media.Media3D;
using Robot6_Simu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using HelixToolkit.Wpf;

namespace Robot6_Simu.Domain
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<double[]>? Data { get; }
        public Home? mainW
        {
            set;
            get;
        }
        public Settings? setW
        {
            set;
            get;
        }
        public MainWindowViewModel()
        {
            //_ = AsyncOpenJson();
            MenuItems = new ObservableCollection<MenuItem>(new[]
            {
                new MenuItem(
                    "Home",
                    typeof(Home),
                    this
                ),
                new MenuItem(
                    "Settings",
                    typeof(Settings),
                    this
                )
            });

            _menuItemsView = CollectionViewSource.GetDefaultView(MenuItems);
            D3030RoboCommand = new AnotherCommandImplementation(D3030Robo);
            OriginalRoboCommand = new AnotherCommandImplementation(OriginalRobo);
            SingularRoboCommand = new AnotherCommandImplementation(o=>SingularRobo(o));
            ResetRoboCommand = new AnotherCommandImplementation(ResetRobo);
            HomeCommand = new AnotherCommandImplementation(_ => { SelectedItem = MenuItems[0]; });
            SettingsCommand = new AnotherCommandImplementation(_ => { SelectedItem = MenuItems[1]; });
            SaveSettingCommand = new AnotherCommandImplementation(_ => { SaveSetting(); });
            SelectedItem = MenuItems[SelectedIndex];
        }

        private readonly ICollectionView? _menuItemsView;
        private MenuItem? _selectedItem;
        private int _selectedIndex;
        private bool _controlsEnabled = true;


        private void OriginalRobo(object? o)
        {
            JointAngles[0] = 0;
            JointAngles[1] = 90;
            JointAngles[2] = 0;
            JointAngles[3] = 0;
            JointAngles[4] = 0;
            JointAngles[5] = 0;
        }
        private void ResetRobo(object? o)
        {
            JointAngles[0] = 0;
            JointAngles[1] = 0;
            JointAngles[2] = 0;
            JointAngles[3] = 0;
            JointAngles[4] = 0;
            JointAngles[5] = 0;
        }
        

        private void SingularRobo(object? o)
        {
            JointAngles[0] = 30;
            JointAngles[1] = 20;
            if (o.ToString() == "1")//一奇异点
            {
                JointAngles[2] = -Math.Atan2(JointLength[4], JointLength[3]) * 57.29578;
                JointAngles[3] = 30;
                JointAngles[4] = 30;
            }
            else if (o.ToString() == "2")//二奇异点
            {
                double r2 = Math.Sin(JointAngles[1] / 57.29578) * JointLength[2] + JointLength[1];
                double t21 = Math.Asin(r2 / (Math.Sqrt(JointLength[3] * JointLength[3] + JointLength[4] * JointLength[4])));
                double t22 = Math.Atan2(JointLength[4], JointLength[3]);
                JointAngles[2] = -(t21 + t22) * 57.29578 - JointAngles[1];
                JointAngles[3] = 30;
                JointAngles[4] = 30;
            }
            else if (o.ToString() == "3")//三奇异点
            {
                JointAngles[2] = 30;
                JointAngles[3] = 30;
                JointAngles[4] = 0;
            }
            JointAngles[5] = 30;
        }
        private void D3030Robo(object? o)
        {
            JointAngles[0] = 30;
            JointAngles[1] = -30;
            JointAngles[2] = 30;
            JointAngles[3] = -30;
            JointAngles[4] = 30;
            JointAngles[5] = 30;
        }
        private void SaveSetting()
        {
            Properties.Settings.Default.Save();
            DialogsViewModel.ShowSaveSucsseceDiag.Execute(null);
            Process p = new Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "Robot6_Simu.exe";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            Application.Current.Shutdown();
        }
        public ObservableCollection<MenuItem> MenuItems { get; }
        public ObservableCollection<MenuItem> MainMenuItems { get; }

        public MenuItem? SelectedItem
        {
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set => SetProperty(ref _controlsEnabled, value);
        }
        public AnotherCommandImplementation HomeCommand { get; }
        public AnotherCommandImplementation SettingsCommand { get; }
        public AnotherCommandImplementation SaveSettingCommand { get; }
        public AnotherCommandImplementation? ComputeCommand { get; }
        public AnotherCommandImplementation? SingularRoboCommand { get; }
        public AnotherCommandImplementation? ResetRoboCommand { get; }
        public AnotherCommandImplementation? D3030RoboCommand { get; }
        public AnotherCommandImplementation? OriginalRoboCommand { get; }
        public AnotherCommandImplementation? ForwardCommand { get; }



        public void Init(object o)
        {
        }
        public static ObservableCollection<double[]>? RotAngles
        {
            set;
            get;
        } = new ObservableCollection<double[]>(new List<double[]>()) { new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 } };
        public static ObservableCollection<double>? JointLength=> new ObservableCollection<double>(new List<double>() { Properties.Settings.Default.l0, Properties.Settings.Default.l1, Properties.Settings.Default.l2, Properties.Settings.Default.l3, Properties.Settings.Default.l4, Properties.Settings.Default.l5, Properties.Settings.Default.l6,Properties.Settings.Default.l7 });
        public static ObservableCollection<double>? JointAngles
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 0, 0, 0, 0 });

        public static Point3D J11
        {
            set;
            get;
        }=        new Point3D(0, 0, JointLength[0]);
        //public static Point3D J1n
        //{
        //    set;
        //    get;
        //} = new Point3D(J11.X + 150, J11.Y, J11.Z);
        //public static Point3D J1o
        //{
        //    set;
        //    get;
        //} = new Point3D(J11.X, J11.Y + 150, J11.Z);
        //public static Point3D J1a
        //{
        //    set;
        //    get;
        //} = new Point3D(J11.X, J11.Y, J11.Z + 150);

        public static Point3D J12
        {
            set;
            get;
        } = new Point3D(JointLength[1], 0, JointLength[0]);
        public static Point3D J121
        {
            set;
            get;
        } = new Point3D(JointLength[1], -40, JointLength[0]);
        public static Point3D J122
        {
            set;
            get;
        } = new Point3D(JointLength[1], 40, JointLength[0]);

        public static Point3D J21
        {
            set;
            get;
        } = J12;
        public static Point3D J1n
        {
            set;
            get;
        } = new Point3D(J21.X + 150, J21.Y, J21.Z);
        public static Point3D J1o
        {
            set;
            get;
        } = new Point3D(J21.X, J21.Y, J21.Z - 150);
        public static Point3D J1a
        {
            set;
            get;
        } = new Point3D(J21.X, J21.Y + 150, J21.Z);

        public static Point3D J22
        {
            set;
            get;
        } = new Point3D(JointLength[1], 0, (JointLength[0] + JointLength[2]));


        public static Point3D J221
        {
            set;
            get;
        } = new Point3D(JointLength[1], -40, (JointLength[0] + JointLength[2]));
        public static Point3D J222
        {
            set;
            get;
        } = new Point3D(JointLength[1], 40, (JointLength[0] + JointLength[2]));
        public static Point3D J31
        {
            set;
            get;
        } = J22;
        public static Point3D J2n
        {
            set;
            get;
        } = new Point3D(J31.X, J31.Y, J31.Z + 150);
        public static Point3D J2o
        {
            set;
            get;
        } = new Point3D(J31.X + 150, J31.Y, J31.Z);
        public static Point3D J2a
        {
            set;
            get;
        } = new Point3D(J31.X, J31.Y + 150, J31.Z);

        public static Point3D J32
        {
            set;
            get;
        } = new Point3D(JointLength[1], 0, (JointLength[0] + JointLength[2] + JointLength[3]));
        public static Point3D J321
        {
            set;
            get;
        } = new Point3D(JointLength[1] - 50, 0, (JointLength[0] + JointLength[2] + JointLength[3]));
        public static Point3D J322
        {
            set;
            get;
        } = new Point3D(JointLength[1] + 50, 0, (JointLength[0] + JointLength[2] + JointLength[3]));
        public static Point3D J41
        {
            set;
            get;
        } = J32;
        public static Point3D J3n
        {
            set;
            get;
        } = new Point3D(J41.X, J41.Y, J41.Z + 150);
        public static Point3D J3o
        {
            set;
            get;
        } = new Point3D(J41.X, J41.Y - 150, J41.Z);
        public static Point3D J3a
        {
            set;
            get;
        } = new Point3D(J41.X + 150, J41.Y, J41.Z);

        public static Point3D J42
        {
            set;
            get;
        } = new Point3D((JointLength[1]  + JointLength[4]), 0, (JointLength[0] + JointLength[2] + JointLength[3]));

        //public static Point3D J45
        //{
        //    set;
        //    get;
        //} = new Point3D((JointLength[1] + JointLength[4] / 2), 0, (JointLength[0] + JointLength[2] + JointLength[3]));

        public static Point3D J4n
        {
            set;
            get;
        } = new Point3D(J42.X, J42.Y, J42.Z + 150);
        public static Point3D J4o
        {
            set;
            get;
        } = new Point3D(J42.X + 150, J42.Y, J42.Z);
        public static Point3D J4a
        {
            set;
            get;
        } = new Point3D(J42.X, J42.Y + 150, J42.Z);

        public static Point3D J421
        {
            set;
            get;
        } = new Point3D((JointLength[1] + JointLength[4]), -20, (JointLength[0] + JointLength[2] + JointLength[3]));
        public static Point3D J422
        {
            set;
            get;
        } = new Point3D((JointLength[1] + JointLength[4]), 20, (JointLength[0] + JointLength[2] + JointLength[3]));
        public static Point3D J51
        {
            set;
            get;
        } = J42;


        public static Point3D J52
        {
            set;
            get;
        } = new Point3D((JointLength[1] + JointLength[4] + JointLength[5] ), 0, (JointLength[0] + JointLength[2] + JointLength[3]));
         public static Point3D J5n
        {
            set;
            get;
        } = new Point3D(J52.X, J52.Y, J52.Z+ 150 );
        public static Point3D J5o
        {
            set;
            get;
        } = new Point3D(J52.X , J52.Y - 150, J52.Z);
        public static Point3D J5a
        {
            set;
            get;
        } = new Point3D(J52.X + 150, J52.Y, J52.Z); 
        public static Point3D J61
        {
            set;
            get;
        } = new Point3D(J52.X, J52.Y, J52.Z);

        public static Point3D J62
        {
            set;
            get;
        } = new Point3D((JointLength[1] + JointLength[4] + JointLength[5]), 0, (JointLength[0] + JointLength[2] + JointLength[3] + JointLength[6]));
        public static Point3D J6n
        {
            set;
            get;
        } = new Point3D(J62.X, J62.Y, J62.Z + 150);
        public static Point3D J6o
        {
            set;
            get;
        } = new Point3D(J62.X, J62.Y - 150, J62.Z);
        public static Point3D J6a
        {
            set;
            get;
        } = new Point3D(J62.X + 150, J62.Y, J62.Z);
        public static Point3D J63
        {
            set;
            get;
        } = new Point3D((JointLength[1] + JointLength[4] + JointLength[5]+ JointLength[7]), 0, (JointLength[0] + JointLength[2] + JointLength[3] + JointLength[6]-11));

        public static ObservableCollection<double>? EndP
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1] + JointLength[4] + JointLength[5] + JointLength[7], 0, JointLength[0] + JointLength[2] + JointLength[3] + JointLength[6] });
        public static ObservableCollection<double>? EndN
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 1 });
        public static ObservableCollection<double>? EndO
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, -1, 0 });
        public static ObservableCollection<double>? EndA
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });

        public static ObservableCollection<double>? J5P
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1] + JointLength[4] + JointLength[5], 0, JointLength[0] + JointLength[2] + JointLength[3] });
        public static ObservableCollection<double>? J5N
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 1 });
        public static ObservableCollection<double>? J5O
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, -1, 0 });
        public static ObservableCollection<double>? J5A
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });


        public static ObservableCollection<double>? J4P
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1] + JointLength[4], 0, JointLength[0] + JointLength[2] + JointLength[3] });
        public static ObservableCollection<double>? J4N
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });
        public static ObservableCollection<double>? J4O
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, -1 });
        public static ObservableCollection<double>? J4A
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 1, 0 });

        public static ObservableCollection<double>? J3P
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1], 0, JointLength[0] + JointLength[2] + JointLength[3] });
        public static ObservableCollection<double>? J3N
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 1 });
        public static ObservableCollection<double>? J3O
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, -1, 0 });
        public static ObservableCollection<double>? J3A
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });

        public static ObservableCollection<double>? J2P
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1], 0, JointLength[0] + JointLength[2] });
        public static ObservableCollection<double>? J2N
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 1 });
        public static ObservableCollection<double>? J2O
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });
        public static ObservableCollection<double>? J2A
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 1, 0 });

        public static ObservableCollection<double>? J1P
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { JointLength[1], 0, JointLength[0]});
        public static ObservableCollection<double>? J1N
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 1, 0, 0 });
        public static ObservableCollection<double>? J1O
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, -1 });
        public static ObservableCollection<double>? J1A
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 1, 0 });

        public static ObservableCollection<double>? IJointAngles
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 0, 0, 0, 0 });
        public static ObservableCollection<double>? Jacobian
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 0, 0, 0, 0, 0 , 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        public static ObservableCollection<double>? Dxyz
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 0, 0, 0, 0 });
        public static ObservableCollection<double>? DAngle
        {
            set;
            get;
        } = new ObservableCollection<double>(new List<double>() { 0, 0, 0, 0, 0, 0 });
        public static float Tz
        {
            set;
            get;
        } = 550;
        public static GeometryModel3D? R0
        {
            get; set;
        }
        public static GeometryModel3D? R1
        {
            get; set;
        }
        public static GeometryModel3D? R2
        {
            get; set;
        }
        public static GeometryModel3D? R3
        {
            get; set;
        }
        public static GeometryModel3D? R4
        {
            get; set;
        }
        public static GeometryModel3D? R5
        {
            get; set;
        }
        public static GeometryModel3D? R6
        {
            get; set;
        }
        public static GeometryModel3D? R7
        {
            get; set;
        }

        //public static double[][] AD=> new double[6][] { new double[2] { JointLength[1], JointLength[0] }, new double[2] { JointLength[2], 0 }, new double[2] { JointLength[3], 0 }, new double[2] { 0, JointLength[4] }, new double[2] { 0, 0 }, new double[2] { JointLength[6], JointLength[5] } };
    }
}
