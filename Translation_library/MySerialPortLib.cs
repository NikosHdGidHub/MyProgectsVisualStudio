using System.IO.Ports;
using System;



namespace Lib.TranslationLib
{
    public interface ISerialPortSimple
    {
        SerialPort Serial { get; }
        string[] GetPortNames();
        void PrintLine(string line);

        event Action<string> ReadData;
        event Action<string> ErrorMessage;

        void Open();
        void Close();
    }

    public class MySerialPortLib : ISerialPortSimple
    {
        public SerialPort Serial { get; }

        public MySerialPortLib(SerialPort serial)
        {
            Serial = serial;

            Serial.DataReceived += Serial_DataReceived;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReadData?.Invoke(Serial.ReadLine());
        }

        public MySerialPortLib():this(new SerialPort())
        { }

        public event Action<string> ReadData;
        public event Action<string> ErrorMessage;

        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void PrintLine(string line)
        {
            if (Serial.IsOpen)
            {
                Serial.Write(line);
            }
            else
                throw new NotImplementedException();
        }

        public void Open()
        {
            try
            {
                Serial.Open();
            }
            catch(Exception ex)
            {
                ErrorMessage?.Invoke(ex.Message);
            }
        }

        public void Close()
        {
            Serial.Close();
        }
    }
}
