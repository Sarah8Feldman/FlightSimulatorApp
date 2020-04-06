using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;


namespace FlightApp
{
    class FlightAppModel : IFlightAppModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        volatile Boolean stop;

        public FlightAppModel()
        {
            stop = false;
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void connect(string ip, int port)
        {
            //...
        }

        public void disconnect()
        {
            //...
        }

        public void start()
        {
            new Thread(delegate ()
            {
                while (!stop)
                {
                    //....

                    Thread.Sleep(250);
                }

            }).Start();
        }

        public double Rudder
        {
            get { return this.Rudder; }
            set {
                if(value <= 1 && value >= -1) {
                    this.Rudder = value;
                }

                if(value < -1){
                    this.Rudder = -1;
                } else {
                    this.Rudder = 1;                
                }
                
            }
        }

         public double heading_deg
        {
            get { return this.heading_deg; }
            set {
                if(value < 360 && value >= 0) {
                    this.heading_deg = value;
                }

                if(value < 0){
                    this.heading_deg = 0;
                } else {
                    this.heading_deg = 360;                
                }
                
            }
        }
    }
}
