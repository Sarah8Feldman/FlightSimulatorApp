using System;
using System.ComponentModel;


namespace FlightApp
{
    public class DashboardVM : INotifyPropertyChanged
    {
        private IFlightAppModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardVM(IFlightAppModel model)
        {
            this.model = model;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public double VM_heading_deg
        {
            get { return model.heading_deg; }
        }
    }
}
