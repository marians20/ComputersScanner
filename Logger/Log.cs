using NLog;
using System;

namespace Logger
{
    public class Log
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public static void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public static void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public static void Exception(Exception ex)
        {
            var stackTrace = ex.StackTrace;
            while(ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            _logger.Error("Exception: {0}\r\n{1}",stackTrace, ex.Message);
        }
    }
}
