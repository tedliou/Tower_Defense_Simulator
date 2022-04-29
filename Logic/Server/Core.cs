using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Core
    {
        public static ILoggerFactory? LoggerFactory { get; set; }

        private readonly ILogger<Core> _logger;

        public Core()
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddConsole();
            });
            _logger = LoggerFactory.CreateLogger<Core>();
        }
    }
}
