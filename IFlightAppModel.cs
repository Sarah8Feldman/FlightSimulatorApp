using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FlightApp
{
    public interface IFlightAppModel : INotifyPropertyChanged
    {
        //event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propName);
        void connect(string ip, int port);
        void disconnect();

        void start();

        double heading_deg { get; set;}


    }
}
