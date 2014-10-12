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
    [XmlType("Server")]
    public class ServerConfiguration
    {
        #region constructor

        public ServerConfiguration()
        {
            AdminIds = new List<int>();
        }

        #endregion

        #region properties

        public int Id { get; set; }

        public string Title { get; set; }
        public string Path { get; set; }
        public List<int> AdminIds { get; set; }

        [XmlIgnore]
        public bool IsNew
        {
            get { return Id == 0; }
        }

        #endregion

        #region methods

        public static void Copy(ServerConfiguration source, ServerConfiguration destination)
        {
            destination.Id = source.Id;
            destination.Title = source.Title;
            destination.Path = source.Path;
            destination.AdminIds.Clear();
            destination.AdminIds.AddRange(source.AdminIds);
        }

        protected bool Equals(ServerConfiguration other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServerConfiguration) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion
    }
}