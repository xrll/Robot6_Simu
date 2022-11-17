﻿using Robot6_Simu.Domain;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot6_Simu.Controls
{
    /// <summary>
    /// Layer.xaml 的交互逻辑
    /// </summary>
    public partial class Inverse : UserControl
    {
        public delegate void ValueChanged();//层参数变化;
        public event ValueChanged OnBaseValueChanged;
        public Inverse()
        {
            InitializeComponent();
        }
    }
}
