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
    /// PointEditDlg.xaml 的交互逻辑
    /// </summary>
    public partial class PointEditDlg : Window
    {
        private int index;

        public PointEditDlg(int index, double X, double Y, double Demand)
        {
            InitializeComponent();

            this.index = index;
            xVal.Text = X.ToString();
            yVal.Text = Y.ToString();
            demand.Text = Demand.ToString();
        }

        //点击确定按钮
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            double x = Convert.ToDouble(xVal.Text);
            double y = Convert.ToDouble(yVal.Text);
            double d = Convert.ToDouble(demand.Text);
            ((MainWindow)Owner).setPoint(index, x, y, d);
            Close();
        }


        //点击取消按钮
        private void cancalBtn_Click(object sender, RoutedEventArgs e)
        {
            //直接退出
            Close();
        }
    }
}
