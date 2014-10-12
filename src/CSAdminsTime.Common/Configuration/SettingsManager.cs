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
using System.Xml;
using System.Xml.Serialization;

namespace CSAdminsTime.Common
{
    public static class SettingsManager
    {
        private const string SettingsFileName = "settings.xml";

        public static ConfigurationContainer Read()
        {
            if (!File.Exists(SettingsFileName))
            {
                return null;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(ConfigurationContainer));
                using (var textWriter = new XmlTextReader(SettingsFileName))
                {
                    return serializer.Deserialize(textWriter) as ConfigurationContainer;
                }
            }
            catch (Exception ex)
            {
                ServiceLocator.GetLogger().WriteException(ex);
                return null;
            }
        }

        public static void Write(ConfigurationContainer configurationContainer)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ConfigurationContainer));
                using (var textWriter = new XmlTextWriter(SettingsFileName, Encoding.UTF8))
                {
                    serializer.Serialize(textWriter, configurationContainer);
                }
            }
            catch (Exception ex)
            {
                ServiceLocator.GetLogger().WriteException(ex);
            }
        }
    }
}