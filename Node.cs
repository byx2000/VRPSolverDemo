using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRPSolverDemo
{
    class Node : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double x, y;
        private double demand;
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("X"));
                }
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Y"));
                }
            }
        }

        public double Demand
        {
            get { return demand; }
            set
            {
                demand = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Demand"));
                }
            }
        }

        public Node(double X, double Y, double Demand)
        {
            this.X = X;
            this.Y = Y;
            this.Demand = Demand;
        }

        public override string ToString()
        {
            return "(" + Math.Round(X, 2).ToString() + ", " + Math.Round(Y, 2).ToString() + ")";
        }
    }
}
