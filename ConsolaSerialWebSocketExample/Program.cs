using ClientSerialWebSocket.SerialPort;
using ClientSerialWebSocket.Websocket;
using System.IO.Ports;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;


public class Program
{
    public static SerialPortEngine _serialPortEngine;
    public static WebSocketServerEngine _webSocketServerEngine;
    public static bool closeConnection=false;
    public static async Task Main(string[] args)
    {
        string comPort = "COM3";
        if(args.Length >0)
        {
            try
            {
                comPort = args[0];
            }
            catch (Exception){}
        }
        _serialPortEngine = new SerialPortEngine(comPort);
        _webSocketServerEngine = new WebSocketServerEngine(IPAddress.Parse("127.0.0.1"));
        Console.WriteLine("Websocket starting in t http://127.0.0.1:4452");

        _= _webSocketServerEngine.Start();
        Console.WriteLine("Client Connected");
        Console.WriteLine("Open and Reading Serial Port COM3");
        _serialPortEngine.OpenPort();
        _serialPortEngine.PortReadAction += PrintSerialPort;

        bool closeTerminal=false;

        while (!closeTerminal)
        {
           
            if(_webSocketServerEngine.RestartService) { 
                _serialPortEngine.ClosePort();
                _ = _webSocketServerEngine.Start();
                Console.WriteLine("Client Connected");
                Console.WriteLine("Open and Reading Serial Port COM3");
                _serialPortEngine.OpenPort();
            }
        }

        
      
    }

    private static void PrintSerialPort(object sender, EventArgs e)
    {
        var message = _serialPortEngine.readPort.LastOrDefault();
        Console.WriteLine(_serialPortEngine.readPort.LastOrDefault());
        
        var response = new
        {
            message = _serialPortEngine.readPort.LastOrDefault()
        };
        _webSocketServerEngine.SendDataMessage(JsonSerializer.Serialize(response));
        if (message == "quit")
        {
            _webSocketServerEngine.Dispose();
            _serialPortEngine.Dispose();
            Environment.Exit(0);
            
        }
    }
}