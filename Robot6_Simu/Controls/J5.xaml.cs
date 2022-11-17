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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot6_Simu.Controls
{
    /// <summary>
    /// MainPara.xaml 的交互逻辑
    /// </summary>
    public partial class J5 : UserControl
    {
        public delegate void ValueChanged();
        public event ValueChanged? OnRotValueChanged;
        public J5()
        {
            InitializeComponent();
        }

        private void V_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((e?.OldValue != null && e?.NewValue != null) && e?.OldValue != e?.NewValue)
                OnRotValueChanged?.Invoke();
        }
    }
}
