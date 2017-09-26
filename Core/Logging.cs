using System;

namespace Aleeda.Core
{
    public class Logging
    {
        #region Fields
        private LogType mMinimumLogImportancy;
        #endregion

        #region Properties
        public LogType MinimumLogImportancy
        {
            get { return mMinimumLogImportancy; }
            set
            {
                if (value != mMinimumLogImportancy)
                {
                    WriteInformation("Changed log type to " + value + ".");
                }
                mMinimumLogImportancy = value;
            }
        }
        #endregion

        #region Methods
        private void WriteLinepublic(ref string sLine, LogType pLogType, bool ignoreLogType)
        {
            lock (this)
            {
                Console.Write("[{0}]", DateTime.Now.ToString());
                Console.Write(" -- ");

                if (pLogType == LogType.Information)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (pLogType == LogType.Title)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (pLogType == LogType.Warning)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (pLogType == LogType.Error)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (pLogType == LogType.Debug)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine(sLine);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        private void WriteCore(ref string sLine, LogType pLogType, bool ignoreLogType)
        {
            lock (this)
            {

                if (pLogType == LogType.Title)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(sLine);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public void WriteLine(string sLine)
        {
            WriteLinepublic(ref sLine, LogType.Information, true);
        }
        public void WriteTitle(string sLine)
        {
            WriteCore(ref sLine, LogType.Title, true);
        }
        public void WriteInformation(string sLine)
        {
            if(mMinimumLogImportancy <= LogType.Information)
                WriteLinepublic(ref sLine, LogType.Information, false);
        }
        public void WriteWarning(string sLine)
        {
            if (mMinimumLogImportancy <= LogType.Warning)
                WriteLinepublic(ref sLine, LogType.Warning, false);
        }
        public void WriteError(string sLine)
        {
            if (mMinimumLogImportancy <= LogType.Error)
                WriteLinepublic(ref sLine, LogType.Error, false);
        }
        public void WriteUnhandledExceptionError(string sMethodName, Exception ex)
        {
            WriteError("Unhandled exception in " + sMethodName + "() method, "
            + "exception message: " + ex.Message + ", "
            + "stack trace: " + ex.StackTrace);
        }
        public void WriteConfigurationParseError(string sField)
        {
            if (mMinimumLogImportancy <= LogType.Error)
            {
                string sLine = string.Format("Could not parse configuration field '{0}'.", sField);
                WriteLinepublic(ref sLine, LogType.Error, false);
            }
        }
        #endregion
    }

    public enum LogType
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Title = 4
    }
}
