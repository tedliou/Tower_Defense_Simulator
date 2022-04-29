using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Server
{
    public class Core
    {
        public static ILoggerFactory? LoggerFactory { get; set; }
        public static int MaxPlayer { get; private set; }
        public int Port { get; private set; }

        private readonly ILogger<Core> _logger;
        private TcpListener _tcpListener;

        public Core()
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddConsole();
            });
            _logger = LoggerFactory.CreateLogger<Core>();

            Start(10, 3000);
        }

        private void Start(int maxPlayer, int port)
        {
            _logger.LogInformation("Starting...");
            _logger.LogInformation($"Port: {port}");
            _logger.LogInformation($"MaxPlayer: {maxPlayer}");

            MaxPlayer = maxPlayer;
            Port = port;

            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPCallback), null);

            _logger.LogInformation("Done!");
        }

        private void TCPCallback(IAsyncResult result)
        {
            var client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPCallback), null);
        }
    }
}
