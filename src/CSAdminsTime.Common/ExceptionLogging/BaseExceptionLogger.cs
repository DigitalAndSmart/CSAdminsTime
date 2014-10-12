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
using System.Text;
using System.Xml.Serialization;

namespace CSAdminsTime.Common
{
    public class BaseExceptionLogger<T> : IExceptionLogger<T> where T : class, IExceptionContainer, new()
    {
        private static string _exceptionsFolderName;
        private static string _errorsFolderName;
        private static string _errorsFileNameTemplate;
        private static readonly object _synchronizationObject = new object();
        
        public void Initialize(string exceptionsFolderName, string errorsFolderName, string errorsFileNameTemplate)
        {
            _exceptionsFolderName = exceptionsFolderName;
            _errorsFolderName = errorsFolderName;
            _errorsFileNameTemplate = errorsFileNameTemplate;
        }

        public T WriteException(Exception exception, bool isCritical)
        {
            lock (_synchronizationObject)
            {
                T exceptionContainer = Convert(exception, isCritical);
                string exceptionsDtoString = exceptionContainer.ToLogString();

                if (!Directory.Exists(_errorsFolderName))
                {
                    Directory.CreateDirectory(_errorsFolderName);
                }

                string errorsFileName = Path.Combine(_errorsFolderName, string.Format(_errorsFileNameTemplate, DateTime.Now.ToString("yyyy-MM-dd")));
                File.AppendAllText(errorsFileName, exceptionsDtoString);

                if (!Directory.Exists(_exceptionsFolderName))
                {
                    Directory.CreateDirectory(_exceptionsFolderName);
                }
                var exceptionFilePath = Path.Combine(_exceptionsFolderName, exceptionContainer.Uid);
                SerializeAndWrite(exceptionFilePath, exceptionContainer);

                return exceptionContainer;
            }
        }

        public T Convert(Exception exception, bool isCritical)
        {
            var exceptionContainer = new T();
            exceptionContainer.Uid = Guid.NewGuid().ToString();
            exceptionContainer.LocalDateTime = DateTime.Now;
            exceptionContainer.UtcDateTime = DateTime.UtcNow;

            var currentException = exception;
            int count = 1;
            var sb = new StringBuilder();
            while (currentException != null)
            {
                sb.AppendLine(string.Format("\t[Message#{0}: \"{1}\"] [Type: \"{2}\"]", count, currentException.Message, currentException.GetType().FullName));
                sb.AppendLine(string.Format("\t\t[Data#{0}]", count));
                sb.AppendLine(string.Format("\t\t\t{0}", currentException.Data));
                sb.AppendLine(string.Format("\t\t[StackTrace#{0}]", count));
                sb.AppendLine(string.Format("\t\t\t{0}", currentException.StackTrace));
                currentException = currentException.InnerException;
                count++;
            }

            exceptionContainer.Information = sb.ToString();
            exceptionContainer.IsCritical = isCritical;
            return exceptionContainer;
        }

        public void SerializeAndWrite(string exceptionFilePath, T exceptionContainer)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StreamWriter(exceptionFilePath, false, Encoding.UTF8))
            {
                serializer.Serialize(stream, exceptionContainer);
            }
        }

        public T ReadAndDeserialize(string exceptionFilePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new StreamReader(exceptionFilePath, Encoding.UTF8))
                {
                    return serializer.Deserialize(stream) as T;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}