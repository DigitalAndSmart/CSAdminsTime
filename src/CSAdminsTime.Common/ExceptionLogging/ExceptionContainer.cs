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
using System.Text;

namespace CSAdminsTime.Common
{
    public class ExceptionContainer : IExceptionContainer
    {
        public string Uid { get; set; }
        public DateTime LocalDateTime { get; set; }
        public DateTime UtcDateTime { get; set; }
        public string Information { get; set; }
        public bool IsCritical { get; set; }

        public virtual string ToLogString()
        {
            var sb = new StringBuilder();
            if (IsCritical)
            {
                sb.Append("[CRITICAL] ");
            }
            sb.AppendLine(string.Format("[{0}] [UID={1}]", LocalDateTime, Uid));
            sb.AppendLine(Information);
            return sb.ToString();
        }
    }
}