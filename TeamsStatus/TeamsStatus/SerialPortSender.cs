using System.IO.Ports;

namespace TeamsStatus
{
    public static class SerialPortSender 
    {
        private static string PortName = string.Empty;
        private static SerialPort _port;

        public static void SendString(string value)
        {
            try
            {
                TryToFindComDevice();

                if (_port == null || !_port!.IsOpen)
                {
                    _port = new SerialPort(PortName, 9600);//Set your board COM
                    _port.Open();
                }

                _port.WriteLine(value);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Device not found.");
            }
        }

        private static void TryToFindComDevice()
        {
            if (    PortName != string.Empty 
                |   (_port != null && _port!.IsOpen))
            {
                return;
            }

            string[] ports = SerialPort.GetPortNames();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The following serial ports were found:");

            foreach (var item in ports)
                Console.WriteLine(item);

            foreach (string port in ports)
            {
                try
                {
                    if (_port == null || !_port!.IsOpen)
                    {
                        _port = new SerialPort(port, 9600);//Set your board COM
                        _port.Open();
                        PortName = port;
                    }

                }
                catch (Exception e)
                {
                    if (_port != null)
                    {
                        _port = null;
                    }
                }
            }
        }
    }
}
