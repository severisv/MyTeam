using System;
using System.Diagnostics;

namespace MyTeam.Services.Application
{
    class Logger : ILogger
    {
        public void LogError(string message = "", Exception e = null)
        {
            Debug.Write($"{message} : {e?.Message}");
        }
    }
}