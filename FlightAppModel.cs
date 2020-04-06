using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;



namespace FlightApp
{
    class FlightAppModel : IFlightAppModel { 
        public Socket mySocket ;
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
            // Connect to a remote device.  
            try
            {
                //for localHost
                //IPAddress ipAddress = ipHostInfo.AddressList[0];

                // Establish the remote endpoint for the socket.   
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                byte[] ipaddress = Encoding.ASCII.GetBytes(ip);
                IPAddress ipAddress = new IPAddress(ipaddress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.  
                mySocket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint & Catch any errors.  
                try
                {
                    mySocket.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        mySocket.RemoteEndPoint.ToString());

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    
        public void disconnect()
        {
            // Release the socket.  
            mySocket.Shutdown(SocketShutdown.Both);
            mySocket.Close();
        }


        public void write(string msg)
        {
            // Encode string into a byte array.  
            byte[] my_msg = Encoding.ASCII.GetBytes(msg);
            // Send the data through the socket.  
            int bytesSent = mySocket.Send(my_msg);
        }

        public string read()
        {
            // Buffer for incoming data.  
            byte[] buffer = new byte[1024];

            // Receive the response from the remote device.  
            int bytesRec = mySocket.Receive(buffer);
            string output = Encoding.ASCII.GetString(buffer, 0, bytesRec);

            return output;
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
                write("set / controls / engines / current - engine / throttle" + value);
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
                    write("get /instrumentation/heading-indicator/indicated-heading-deg");
                    indicated_heading_deg = Double.Parse(read());

                   // write("get /instrumentation/gps/indicated-vertical-speed");
                  //  gps_indicated_vertical_speed = Double.Parse(read());

                  //  write("get /instrumentation/gps/indicated-ground-speed-kt");
                  //  gps_indicated_ground_speed_kt  = Double.Parse(read());

                  //  write("get /instrumentation/airspeed-indicator/indicated-speed-kt");
                  //  airspeed_indicator_indicatedspeed_kt = Double.Parse(read());

                  //  write("get instrumentation/gps/indicated-altitude-ft");
                    //gps_indicated_altitude_ft = Double.Parse(read());

                  //  write("get instrumentation/attitude-indicator/internal-roll-deg");
                  //  attitude-indicator_internal_roll_deg = Double.Parse(read());

                  //  write("get /instrumentation/attitude-indicator/internal-pitch-deg");
                   // attitude-indicator_internal_pitch_deg = Double.Parse(read());

                 //   write("get /instrumentation/altimeter/indicated-altitude-ft");
                  //  altimeter_indicated_altitude_ft = Double.Parse(read());

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
