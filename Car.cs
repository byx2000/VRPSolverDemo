using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRPSolverDemo
{
    class Car : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double capacity;
        private double disLimit;

        public double Capacity
        {
            get { return capacity; }
            set
            {
                capacity = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Capacity"));
                }
            }
        }

        public double DisLimit
        {
            get { return disLimit; }
            set
            {
                disLimit = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DisLimit"));
                }
            }
        }

        public Car(double capacity, double disLimit)
        {
            Capacity = capacity;
            DisLimit = disLimit;
        }
    }
}
