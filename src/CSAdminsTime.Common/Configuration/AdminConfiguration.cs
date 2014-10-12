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
    [XmlType("Admin")]
    public class AdminConfiguration
    {
        #region properties

        public int Id { get; set; }
        public string RealName { get; set; }

        public string NameIPSteamid { get; set; }
        public string Password { get; set; }
        public AccessFlag AccessFlags { get; set; }
        public ConnectionFlag ConnectionFlags { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }

        [XmlIgnore]
        public string AccessFlagsString
        {
            get { return AccessFlags == 0 ? string.Empty : AccessFlags.ToString().Replace(", ", string.Empty); }
        }

        [XmlIgnore]
        public string ConnectionFlagsString
        {
            get { return ConnectionFlags == 0 ? string.Empty : ConnectionFlags.ToString().Replace(", ", string.Empty); }
        }

        [XmlIgnore]
        public string StartDateString
        {
            get { return StartDate.ToShortDateString(); }
        }

        [XmlIgnore]
        public string StartTimeString
        {
            get { return StartTime.ToShortTimeString(); }
        }

        [XmlIgnore]
        public string EndDateString
        {
            get { return EndDate.ToShortDateString(); }
        }

        [XmlIgnore]
        public string EndTimeString
        {
            get { return EndTime.ToShortTimeString(); }
        }

        [XmlIgnore]
        public bool IsNew
        {
            get { return Id == 0; }
        }

        #endregion

        #region methods

        public static void Copy(AdminConfiguration source, AdminConfiguration destination)
        {
            destination.Id = source.Id;
            destination.RealName = source.RealName;
            destination.NameIPSteamid = source.NameIPSteamid;
            destination.Password = source.Password;
            destination.AccessFlags = source.AccessFlags;
            destination.ConnectionFlags = source.ConnectionFlags;
            destination.StartDate = source.StartDate;
            destination.StartTime = source.StartTime;
            destination.EndDate = source.EndDate;
            destination.EndTime = source.EndTime;
        }

        public static bool CheckStartEnd(AdminConfiguration adminConfiguration, DateTime originDateTime)
        {
            bool filterResult = originDateTime.Date >= adminConfiguration.StartDate.Date && originDateTime.Date < adminConfiguration.EndDate.Date;
            if (adminConfiguration.StartTime.TimeOfDay > adminConfiguration.EndTime.TimeOfDay)
            {
                filterResult &= originDateTime.TimeOfDay >= adminConfiguration.StartTime.TimeOfDay || originDateTime.TimeOfDay < adminConfiguration.EndTime.TimeOfDay;
            }
            else if (adminConfiguration.StartTime.TimeOfDay < adminConfiguration.EndTime.TimeOfDay)
            {
                filterResult &= originDateTime.TimeOfDay >= adminConfiguration.StartTime.TimeOfDay && originDateTime.TimeOfDay < adminConfiguration.EndTime.TimeOfDay;
            }

            return filterResult;
        }

#if DEBUG

        public static void Test()
        {
            var list = new List<AdminConfiguration>();
            list.Add(new AdminConfiguration { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), StartTime = DateTime.Now.Date, EndTime = DateTime.Now.Date.AddHours(12) });
            list.Add(new AdminConfiguration { StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2), StartTime = DateTime.Now.Date, EndTime = DateTime.Now.Date.AddHours(12) });
            list.Add(new AdminConfiguration { StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now, StartTime = DateTime.Now.Date, EndTime = DateTime.Now.Date.AddHours(12) });

            foreach (AdminConfiguration adminConfiguration in list)
            {
                CheckStartEnd(adminConfiguration, DateTime.Now.Date);
            }

            list.Clear();

            list.Add(new AdminConfiguration { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), StartTime = DateTime.Now.Date, EndTime = DateTime.Now.Date.AddHours(12) });
            list.Add(new AdminConfiguration { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), StartTime = DateTime.Now.Date.AddHours(12), EndTime = DateTime.Now.Date });
            list.Add(new AdminConfiguration { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), StartTime = DateTime.Now.Date.AddHours(12), EndTime = DateTime.Now.Date.AddHours(11) });
            list.Add(new AdminConfiguration { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), StartTime = DateTime.Now.Date.AddHours(23), EndTime = DateTime.Now.Date.AddHours(5) });

            foreach (AdminConfiguration adminConfiguration in list)
            {
                CheckStartEnd(adminConfiguration, DateTime.Now.Date);
                CheckStartEnd(adminConfiguration, DateTime.Now.Date.AddHours(5));
                CheckStartEnd(adminConfiguration, DateTime.Now.Date.AddHours(11).AddMinutes(30));
                CheckStartEnd(adminConfiguration, DateTime.Now.Date.AddHours(13));
            }
        }

#endif

        protected bool Equals(AdminConfiguration other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AdminConfiguration) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion
    }
}