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
using System.Windows.Shapes;

namespace VRPSolverDemo
{
    /// <summary>
    /// CarEditDlg.xaml 的交互逻辑
    /// </summary>
    public partial class CarEditDlg : Window
    {
        public CarEditDlg()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            double cap = Convert.ToDouble(capacity.Text);
            double dis = Convert.ToDouble(disLimit.Text);
            ((MainWindow)Owner).addCar(cap, dis);
            Close();
        }

        private void cancalBtn_Click(object sender, RoutedEventArgs e)
        {
            //直接退出
            Close();
        }
    }
}
