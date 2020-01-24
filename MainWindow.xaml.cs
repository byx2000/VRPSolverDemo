using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VRPSolverDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Node> nodes = new ObservableCollection<Node>(); //配送点列表数据源
        ObservableCollection<Car> cars = new ObservableCollection<Car>(); //车辆列表数据源
        List<Button> buttons = new List<Button>(); //所有按钮
        List<Line> lines = new List<Line>(); //所有连线

        private double xRange = 30;
        private double yRange = 20;

        public MainWindow()
        {
            InitializeComponent();

            //设置配送点列表数据源
            nodeList.ItemsSource = nodes;

            //设置车辆列表数据源
            carList.ItemsSource = cars;
        }

        //直角坐标系转换为屏幕坐标系
        Point coordToScreen(Point p)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            return new Point(p.X / xRange * width, height - p.Y / yRange * height);
        }

        //屏幕坐标系转换为直角坐标系
        Point screenToCoord(Point p)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            return new Point(p.X / width * xRange, yRange - p.Y / height * yRange);
        }

        //添加配送点
        private void addNode(double x, double y, double demand)
        {
            nodes.Add(new Node(x, y, demand));
            Point p = coordToScreen(new Point(x, y));

            Button button = new Button();
            button.Tag = nodes.Count - 1;
            button.Width = 10;
            button.Height = 10;
            setButtonPos(button, p);
            button.Click += Button_Click;
            canvas.Children.Add(button);
            buttons.Add(button);
        }

        //添加车辆
        public void addCar(double capacity, double disLimit)
        {
            cars.Add(new Car(capacity, disLimit));
        }

        //画布点击事件
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Point p = e.GetPosition(canvas);
            Point t = screenToCoord(p);
            addNode(t.X, t.Y, 0.0);
        }

        //按钮点击事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //获取按钮的标签
            Button button = sender as Button;
            int index = (int)button.Tag;

            //选中列表相应项
            nodeList.SelectedIndex = index;

            //弹出编辑对话框
            PointEditDlg dlg = new PointEditDlg(index, nodes[index].X, nodes[index].Y, nodes[index].Demand);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        //列表项选中事件
        private void pointList_Selected(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            buttons[listBox.SelectedIndex].Focus();
        }

        //设置点的坐标，供外部程序调用
        public void setPoint(int index, double x, double y, double demand)
        {
            Point p = coordToScreen(new Point(x, y));

            nodes[index].X = x;
            nodes[index].Y = y;
            nodes[index].Demand = demand;

            setButtonPos(buttons[index], p);
        }

        //重置画布
        private void resetCanvas()
        {
            canvas.Children.Clear();
            for (int i = 0; i < lines.Count; ++i)
            {
                canvas.Children.Add(lines[i]);
            }
            for (int i = 0; i < buttons.Count; ++i)
            {
                canvas.Children.Add(buttons[i]);
            }
        }

        //求解成功回调函数
        private void OnFinish(int[][] path, double[] load, double[] mileage)
        {
            Color[] colors =
            {
                Color.FromRgb(144, 211, 144),
                Color.FromRgb(148, 77, 184),
                Color.FromRgb(219, 77, 77),
                Color.FromRgb(184, 148, 77),
                Color.FromRgb(122, 122, 122),
                Color.FromRgb(179, 143, 72),
                Color.FromRgb(77, 113, 184),
                Color.FromRgb(219, 77, 148),
                Color.FromRgb(77, 148, 113),
            };

            int colorIndex = 0;

            lines.Clear();
            for (int i = 0; i < path.Length; ++i)
            {
                Color color = colors[colorIndex];
                colorIndex = (colorIndex + 1) % colors.Length;

                int last = 0;
                for (int j = 0; j < path[i].Length; ++j)
                {
                    //添加连线
                    Point pp1 = getButtonPos(buttons[last]);
                    Point pp2 = getButtonPos(buttons[path[i][j]]);
                    lines.Add(getLine(pp1, pp2, color));
                    last = path[i][j];
                }

                Point p1 = getButtonPos(buttons[last]);
                Point p2 = getButtonPos(buttons[0]);
                lines.Add(getLine(p1, p2, color));
            }

            resetCanvas();
        }

        //求解失败回调函数
        private void OnError(int errCode)
        {
            if (errCode == VRPSolver.NODE_DATA_SIZE_INVALID)
            {
                MessageBox.Show("配送点数据长度不一致");
            }
            else if (errCode == VRPSolver.CAR_DATA_SIZE_INVALID)
            {
                MessageBox.Show("车辆数据长度不一致");
            }
            else if (errCode == VRPSolver.WEIGHT_INVALID)
            {
                MessageBox.Show("权重数值不合法");
            }
            else if (errCode == VRPSolver.DEMAND_INVALID)
            {
                MessageBox.Show("配送点需求不合法");
            }
            else if (errCode == VRPSolver.CAPACITY_INVALID)
            {
                MessageBox.Show("车辆载重量不合法");
            }
            else if (errCode == VRPSolver.DISLIMIT_INVALID)
            {
                MessageBox.Show("车辆里程限制不合法");
            }
            else
            {
                MessageBox.Show("未知错误");
            }
        }

        private Line getLine(Point p1, Point p2, Color color)
        {
            Line line = new Line();
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = 2;
            return line;
        }

        private Point getButtonPos(Button button)
        {
            return button.TranslatePoint(new Point(button.Width / 2, button.Height / 2), canvas);
        }

        private void setButtonPos(Button button, Point p)
        {
            button.SetValue(Canvas.LeftProperty, p.X - button.Width / 2);
            button.SetValue(Canvas.TopProperty, p.Y - button.Height / 2);
        }

        //求解按钮点击事件
        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            int numNode = nodes.Count;
            double[] x = new double[numNode];
            double[] y = new double[numNode];
            double[] demand = new double[numNode];
            for (int i = 0; i < numNode; ++i)
            {
                x[i] = nodes[i].X;
                y[i] = nodes[i].Y;
                demand[i] = nodes[i].Demand;
            }

            int numCar = cars.Count;
            double[] capacity = new double[numCar];
            double[] disLimit = new double[numCar];
            for (int i = 0; i < numCar; ++i)
            {
                capacity[i] = cars[i].Capacity;
                disLimit[i] = cars[i].DisLimit;
            }

            double k1 = 100, k2 = 1, k3 = 1;

            VRPSolver.Solve(x, y, demand, capacity, disLimit, k1, k2, k3, OnFinish, OnError);
        }

        //添加车辆
        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            CarEditDlg dlg = new CarEditDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        //文件输入按钮
        private void FileInput_Click(object sender, RoutedEventArgs e)
        {
            // 打开文件
            StreamReader reader = new StreamReader("in.txt");
            string txt = reader.ReadToEnd();
            char[] sep = { ' ', '\t', '\n' };
            string[] input = txt.Split(sep);

            int index = 0;

            // 解析数据
            int numNode = Convert.ToInt32(input[index++]);
            double[] x = new double[numNode];
            double[] y = new double[numNode];
            double[] demand = new double[numNode];
            for (int i = 0; i < numNode; ++i)
            {
                x[i] = Convert.ToDouble(input[index++]);
                y[i] = Convert.ToDouble(input[index++]);
                demand[i] = Convert.ToDouble(input[index++]);
            }

            int numCar = Convert.ToInt32(input[index++]);
            double[] capacity = new double[numCar];
            double[] disLimit = new double[numCar];
            for (int i = 0; i < numCar; ++i)
            {
                capacity[i] = Convert.ToDouble(input[index++]);
                disLimit[i] = Convert.ToDouble(input[index++]);
            }

            double k1 = Convert.ToDouble(input[index++]);
            double k2 = Convert.ToDouble(input[index++]);
            double k3 = Convert.ToDouble(input[index++]);

            //添加数据
            nodes.Clear();
            cars.Clear();
            buttons.Clear();
            lines.Clear();
            for (int i = 0; i < numNode; ++i)
            {
                addNode(x[i], y[i], demand[i]);
            }
            for (int i = 0; i < numCar; ++i)
            {
                addCar(capacity[i], disLimit[i]);
            }

            resetCanvas();
        }
    }
}
