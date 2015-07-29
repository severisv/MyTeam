using System;

namespace MyTeam.Services.Application
{
    public interface ILogger
    {
        void LogError(string message = "", Exception e = null);
    }
}