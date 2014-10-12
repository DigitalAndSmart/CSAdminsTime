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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CSAdminsTime.Common
{
    [Serializable]
    [XmlRoot("configuration")]
    public class ConfigurationContainer
    {
        [XmlIgnore]
        public const int DefaultUpdatePeriod = 25;
#if DEBUG
        [XmlIgnore]
        public const int MinUpdatePeriod = 1;
#else
        [XmlIgnore]
        public const int MinUpdatePeriod = 5;
#endif
        [XmlIgnore]
        public const int MaxUpdatePeriod = 60;
        
        public LanguageConfiguration Language { get; set; }

        public int UpdatePeriod { get; set; }
        public List<ServerConfiguration> Servers { get; set; }
        public List<AdminConfiguration> Admins { get; set; }
    }
}