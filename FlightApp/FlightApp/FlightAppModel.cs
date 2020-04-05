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
    }
}
