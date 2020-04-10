using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FlightApp
{
    interface IFlightAppModel : INotifyPropertyChanged
    {
        //event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propName);
        void connect();
        void disconnect();

        void start();


    }
}
