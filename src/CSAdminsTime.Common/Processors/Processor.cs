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
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace CSAdminsTime.Common
{
    public class Processor
    {
        #region events

        public event EventHandler Stopped;

        #endregion

        #region fields

        private const string BackupFileExtension = ".bak";
        private readonly Timer _timer = new Timer();
        private bool _isFirstTimeInitialized = false;

        #endregion

        #region constructors

        public Processor()
        {
            _timer.Elapsed += TimerOnElapsed;
        }

        #endregion

        #region properties

        private ConfigurationContainer ConfigurationContainerForExecution { get; set; }

        #endregion

        #region methods

        public void Start()
        {
            ConfigurationContainerForExecution = SettingsManager.Read();
            if (!ConfigurationIsValid())
            {
                Stop();
                return;
            }

            _timer.Interval = ConfigurationContainerForExecution.UpdatePeriod * 1000 * 60;
            _timer.Start();
            Logger.Instance.WriteInfo(string.Format(Properties.Strings.Next_execution_at_X, DateTime.Now.AddMinutes(ConfigurationContainerForExecution.UpdatePeriod)));
            _isFirstTimeInitialized = false;
        }

        public bool ConfigurationIsValid()
        {
            bool validationResult = true;
            if (ConfigurationContainerForExecution == null)
            {
                Logger.Instance.WriteError(Properties.Strings.Configuration_not_defined);
                return false;
            }

            if (ConfigurationContainerForExecution.UpdatePeriod < ConfigurationContainer.MinUpdatePeriod)
            {
                string message = string.Format(Properties.Strings.X_is_Y_this_less_than_minimun_Z, Properties.Strings.Update_period, ConfigurationContainerForExecution.UpdatePeriod, ConfigurationContainer.MinUpdatePeriod);
                message += ". ";
                message += string.Format(Properties.Strings.X_will_be_set_to_Y, Properties.Strings.Update_period, ConfigurationContainer.MinUpdatePeriod);

                Logger.Instance.WriteWarning(message);
                ConfigurationContainerForExecution.UpdatePeriod = ConfigurationContainer.MinUpdatePeriod;
            }

            if (ConfigurationContainerForExecution.UpdatePeriod > ConfigurationContainer.MaxUpdatePeriod)
            {
                string message = string.Format(Properties.Strings.X_is_Y_this_bigger_than_maximum_Z, Properties.Strings.Update_period, ConfigurationContainerForExecution.UpdatePeriod, ConfigurationContainer.MaxUpdatePeriod);
                message += ". ";
                message += string.Format(Properties.Strings.X_will_be_set_to_Y, Properties.Strings.Update_period, ConfigurationContainer.MaxUpdatePeriod);

                Logger.Instance.WriteWarning(message);
                ConfigurationContainerForExecution.UpdatePeriod = ConfigurationContainer.MaxUpdatePeriod;
            }

            if (ConfigurationContainerForExecution.Servers == null ||
                ConfigurationContainerForExecution.Servers.Count == 0)
            {
                Logger.Instance.WriteError(Properties.Strings.Servers_not_configured);
                validationResult = false;
            }

            if (ConfigurationContainerForExecution.Admins == null ||
                ConfigurationContainerForExecution.Admins.Count == 0)
            {
                Logger.Instance.WriteError(Properties.Strings.Admins_not_configured);
                validationResult = false;
            }

            return validationResult;
        }

        public void Stop()
        {
            _timer.Stop();
            _isFirstTimeInitialized = false;
            OnStopped();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                if (!_isFirstTimeInitialized)
                {
                    Misc.SetCurrentUiCulture(ConfigurationContainerForExecution.Language);
                    _isFirstTimeInitialized = true;
                }

                DateTime originDateTime = DateTime.Now;
                Logger.Instance.WriteInfo(string.Format(Properties.Strings.Count_of_configured_servers_is_X, ConfigurationContainerForExecution.Servers.Count));
                foreach (ServerConfiguration serverConfiguration in ConfigurationContainerForExecution.Servers)
                {
                    Logger.Instance.WriteInfo(string.Format(Properties.Strings.Start_execution_for_server_X, serverConfiguration.Title));

                    if (!File.Exists(serverConfiguration.Path))
                    {
                        Logger.Instance.WriteWarning(string.Format(Properties.Strings.Cant_find_path_X, serverConfiguration.Path));
                        Logger.Instance.WriteInfo(string.Format(Properties.Strings.Stop_execution_for_server_X, serverConfiguration.Title));
                        continue;
                    }

                    var admins = new StringBuilder();
                    IEnumerable<AdminConfiguration> filteredAdmins = ConfigurationContainerForExecution.Admins.Where(a => serverConfiguration.AdminIds.Contains(a.Id) && AdminConfiguration.CheckStartEnd(a, originDateTime));
                    if (!filteredAdmins.Any())
                    {
                        Logger.Instance.WriteWarning(string.Format(Properties.Strings.Count_of_administrators_for_add_X, 0));
                        Logger.Instance.WriteInfo(string.Format(Properties.Strings.Stop_execution_for_server_X, serverConfiguration.Title));
                    }
                    else
                    {
                        Logger.Instance.WriteInfo(string.Format(Properties.Strings.Count_of_administrators_for_add_X, filteredAdmins.Count()));
                    }

                    foreach (AdminConfiguration adminConfiguration in filteredAdmins)
                    {
                        admins.AppendLine(string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" //{4}", adminConfiguration.NameIPSteamid, adminConfiguration.Password, adminConfiguration.AccessFlagsString, adminConfiguration.ConnectionFlagsString, adminConfiguration.RealName));
                    }

                    Logger.Instance.WriteInfo(Properties.Strings.Trying_update_file);

                    string backupFilePath = string.Format("{0}{1}", serverConfiguration.Path, BackupFileExtension);
                    if (!File.Exists(backupFilePath))
                    {
                        File.Copy(serverConfiguration.Path, backupFilePath);
                    }

                    File.WriteAllText(serverConfiguration.Path, admins.ToString(), Misc.Utf8WithoutBom);

                    Logger.Instance.WriteSuccess(Properties.Strings.File_updated_successfully);
                    Logger.Instance.WriteInfo(string.Format(Properties.Strings.Stop_execution_for_server_X, serverConfiguration.Title));
                }

                Logger.Instance.WriteInfo(string.Format(Properties.Strings.Next_execution_at_X, DateTime.Now.AddMinutes(ConfigurationContainerForExecution.UpdatePeriod)));
            }
            catch (Exception ex)
            {
                var exceptionContainer = ServiceLocator.GetLogger().WriteException(ex, true);
                Logger.Instance.WriteError(string.Format(Properties.Strings.Something_went_wrong_exception_UID_X, exceptionContainer.Uid));
                Stop();
            }
        }

        protected virtual void OnStopped()
        {
            EventHandler handler = Stopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}