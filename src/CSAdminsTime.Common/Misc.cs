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
using System.Globalization;
using System.Text;
using System.Threading;

namespace CSAdminsTime.Common
{
    public static class Misc
    {
        public const string ShortDatePattern = "dd/MM/yyyy";
        public const string ShortTimePattern = "HH:mm";

        public static UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);

        public static void SetCurrentUiCulture(LanguageConfiguration languageConfiguration)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(languageConfiguration.ToString());
            }
            catch (Exception ex)
            {
                var exception = new Exception(string.Format("Can't set CurrentUiCulture to {0}", languageConfiguration), ex);
                ServiceLocator.GetLogger().WriteException(exception);
            }
        }

        public static class SettingSyncKey
        {
            public const string Language = "language";
            public const string ExecutionInterval = "execution_interval";
            public const string CountOfServers = "count_of_servers";
            public const string CountOfAdmins = "count_of_admins";
        }
    }
}