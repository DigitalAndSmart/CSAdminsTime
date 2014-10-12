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
using System.Collections.Generic;
using CSAdminsTime.Common.Properties;

namespace CSAdminsTime.Common
{
    public enum LanguageConfiguration
    {
        en,
        ru
    }

    public static class LanguageConfigurationHelper
    {
        public static readonly KeyValuePair<LanguageConfiguration, string> En = new KeyValuePair<LanguageConfiguration, string>(LanguageConfiguration.en, Strings.English);
        public static readonly KeyValuePair<LanguageConfiguration, string> Ru = new KeyValuePair<LanguageConfiguration, string>(LanguageConfiguration.ru, Strings.Russian);

        public static readonly IEnumerable<KeyValuePair<LanguageConfiguration, string>> Languages =
            new []
                {
                    new KeyValuePair<LanguageConfiguration, string>(LanguageConfiguration.en, Strings.English),
                    new KeyValuePair<LanguageConfiguration, string>(LanguageConfiguration.ru, Strings.Russian)
                };

    }
}