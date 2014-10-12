// Copyright 2014 Sergey Rumyantsev http://www.digitalandsmart.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.IO;

namespace CSAdminsTime.Common
{
    public class Logger
    {
        #region events

        public event LogEntryAddedDelegate LogEntryAdded;
        public delegate void LogEntryAddedDelegate(LogEntry logEntry);

        #endregion

        #region fields

        private const string _logsFolderName = "logs";
        private const string _logFileNameTemplate = "{0}.log";
        private static readonly object _synchronizationObject = new object();
        private static Logger _instance;

        #endregion

        #region constructors

        private Logger()
        {
        }

        #endregion

        #region properties

        public static Logger Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        #endregion

        #region methods

        public void WriteInfo(string message)
        {
            var logEntry = new LogEntry(LogEntryType.Info, message);
            OnLogEntryAdded(logEntry);
            WriteToLog(logEntry);
        }

        public void WriteSuccess(string message)
        {
            var logEntry = new LogEntry(LogEntryType.Success, message);
            OnLogEntryAdded(logEntry);
            WriteToLog(logEntry);
        }

        public void WriteWarning(string message)
        {
            var logEntry = new LogEntry(LogEntryType.Warning, message);
            OnLogEntryAdded(logEntry);
            WriteToLog(logEntry);
        }

        public void WriteError(string message)
        {
            var logEntry = new LogEntry(LogEntryType.Error, message);
            OnLogEntryAdded(logEntry);
            WriteToLog(logEntry);
        }

        protected virtual void OnLogEntryAdded(LogEntry logEntry)
        {
            LogEntryAddedDelegate handler = LogEntryAdded;
            if (handler != null)
            {
                handler(logEntry);
            }
        }

        private void WriteToLog(LogEntry logEntry)
        {
            lock (_synchronizationObject)
            {
                string logMessage = string.Format("[{0}] [{1}] - {2}{3}", logEntry.Created, Enum.GetName(typeof (LogEntryType), logEntry.LogEntryType), logEntry.Message, Environment.NewLine);

                if (!Directory.Exists(_logsFolderName))
                {
                    Directory.CreateDirectory(_logsFolderName);
                }

                string logFileName = Path.Combine(_logsFolderName, string.Format(_logFileNameTemplate, DateTime.Now.ToString("yyyy-MM-dd")));
                File.AppendAllText(logFileName, logMessage);
            }
        }

        #endregion
    }
}