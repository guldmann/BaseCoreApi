using Serilog.Core;
using Serilog.Events;
using System;

namespace BaseCoreApi.Settings
{
    public class VariableLoggingLevelSwitch : LoggingLevelSwitch
    {
        public VariableLoggingLevelSwitch(string logLevel)
        {
            LogEventLevel level = LogEventLevel.Information;
            if (Enum.TryParse<LogEventLevel>(Environment.ExpandEnvironmentVariables(logLevel), true, out level))
            {
                MinimumLevel = level;
            }
        }
    }
}
