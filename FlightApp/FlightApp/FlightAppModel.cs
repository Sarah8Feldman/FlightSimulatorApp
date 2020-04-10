using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


namespace FlightApp
{
    class FlightAppModel : IFlightAppModel { 
        public event PropertyChangedEventHandler PropertyChanged;
        volatile bool stop;
        private static Mutex mut = new Mutex();
        private TcpClient myClient;
        private NetworkStream myStream;

        public FlightAppModel()
        {
            stop = false;
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void connect()
        {
            try
            {
                // Create a TcpClient. 
                //get ip and port from app.config
                string ip = ConfigurationManager.AppSettings["ip"];
                int port = Int32.Parse(ConfigurationManager.AppSettings["port"]);
                myClient = new TcpClient(ip, port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    
        public void disconnect()
        {
            //stop the thread loop
            this.stop = true;
            // Release the stream.  
            myStream.Close();
            myClient.Close();
            
        }


        public void write(string msg)
        {
            mut.WaitOne();
            // Encode string into a byte array.  
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg+"\n");
            myStream = myClient.GetStream();
            myStream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", msg);
        
            mut.ReleaseMutex();
        }

        public string read()
        {
            mut.WaitOne();
            // Buffer for incoming data.  
            byte[] buffer = new byte[1024];
            String dataFromSever = String.Empty;
            Int32 bytes = myStream.Read(buffer, 0, buffer.Length);
             dataFromSever = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
            Console.WriteLine("Received: {0}", dataFromSever);
            mut.ReleaseMutex();

            return dataFromSever;
        }

        public void setToServer(string var, string value)
        {
            if (var == "Rudder")
            {
                write("set /controls/flight/rudder" + value);
            }
            else if (var == "Elevator")
            {
                write("set /controls/flight/elevator" + value);
            }
            else if (var == "Aileron")
            {
                write("set /controls/flight/aileron" + value);
            }
            else if (var == "Throttle")
            {
                write("set /controls/engines/current-engine/throttle" + value);
            }
            else
            {
                Console.WriteLine("ERR - no such value");
            }

        }


        public void start()
        {
            new Thread(delegate ()
            {
                while (!stop)
                {
                  //  write("get /instrumentation/heading-indicator/indicated-heading-deg");
                   // indicated_heading_deg = Double.Parse(read());
                   // Console.WriteLine("indicated_heading_de = " + indicated_heading_deg);


                   // write("get /instrumentation/gps/indicated-vertical-speed");
                  //  gps_indicated_vertical_speed = Double.Parse(read());

                  //  write("get /instrumentation/gps/indicated-ground-speed-kt");
                  //  gps_indicated_ground_speed_kt  = Double.Parse(read());

                  //  write("get /instrumentation/airspeed-indicator/indicated-speed-kt");
                  //  airspeed_indicator_indicatedspeed_kt = Double.Parse(read());

                  //  write("get instrumentation/gps/indicated-altitude-ft");
                    //gps_indicated_altitude_ft = Double.Parse(read());

                  //  write("get instrumentation/attitude-indicator/internal-roll-deg");
                  //  attitude_indicator_internal_roll_deg = Double.Parse(read());

                  //  write("get /instrumentation/attitude-indicator/internal-pitch-deg");
                   // attitude_indicator_internal_pitch_deg = Double.Parse(read());

                 //   write("get /instrumentation/altimeter/indicated-altitude-ft");
                  //  altimeter_indicated_altitude_ft = Double.Parse(read());


                    //for testing pavels server
                    write("get /controls/flight/rudder");
                    
                    Rudder = Double.Parse(read());
                    Console.WriteLine("Rudder = " + Rudder);


                    Thread.Sleep(250);
                }

            }).Start();
        }
    

    public double Rudder
    {
        get { return this.Rudder; }
        set
        {
            if (value <= 1 && value >= -1)
            {
                this.Rudder = value;
            }

            if (value < -1)
            {
                this.Rudder = -1;
            }
            else
            {
                this.Rudder = 1;
            }

        }
    }

    public double indicated_heading_deg
    {
        get { return this.indicated_heading_deg; }
        set
        {
            if (value < 360 && value >= 0)
            {
                this.indicated_heading_deg = value;
            }

            if (value < 0)
            {
                this.indicated_heading_deg = 0;
            }
            else
            {
                this.indicated_heading_deg = 360;
            }

        }
    }

}
    

}



