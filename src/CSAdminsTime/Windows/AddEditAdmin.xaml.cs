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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CSAdminsTime.Common;

namespace CSAdminsTime
{
    /// <summary>
    /// Interaction logic for AddEditAdmin.xaml
    /// </summary>
    public partial class AddEditAdmin : Window, INotifyPropertyChanged
    {
        #region fields

        private readonly AdminConfiguration _originalAdminConfiguration;
        private readonly AdminConfiguration _adminConfiguration = new AdminConfiguration();
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IConfigurationContainerHolder _configurationContainerHolder;

        #endregion

        #region constructor

        public AddEditAdmin(IConfigurationContainerHolder configurationContainerHolder, AdminConfiguration adminConfiguration)
        {
            _configurationContainerHolder = configurationContainerHolder;
            _originalAdminConfiguration = adminConfiguration;
            AdminConfiguration.Copy(_originalAdminConfiguration, _adminConfiguration);
            DataContext = this;
            InitializeComponent();

            Title = _adminConfiguration.IsNew ? Properties.Strings.Create_admin : Properties.Strings.Modify_admin;
        }

        #endregion

        #region properties

        public string RealName
        {
            get { return _adminConfiguration.RealName; }
            set
            {
                _adminConfiguration.RealName = value;
                OnPropertyChanged("RealName");
            }
        }

        public string NameIPSteamid
        {
            get { return _adminConfiguration.NameIPSteamid; }
            set
            {
                _adminConfiguration.NameIPSteamid = value;
                OnPropertyChanged("NameIPSteamid");
            }
        }

        public string Password
        {
            get { return _adminConfiguration.Password; }
            set
            {
                _adminConfiguration.Password = value;
                OnPropertyChanged("Password");
            }
        }

        public string AccessFlags
        {
            get { return _adminConfiguration.AccessFlagsString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _adminConfiguration.AccessFlags = 0;
                    OnPropertyChanged("AccessFlags");
                    return;
                }

                AccessFlag accessFlags = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    string enumValue = value.Substring(i, 1);
                    try
                    {
                        if (Enum.IsDefined(typeof(AccessFlag), enumValue))
                        {
                            accessFlags |= (AccessFlag)Enum.Parse(typeof(AccessFlag), enumValue);
                        }
                    }
                    catch
                    {
                    }
                }

                _adminConfiguration.AccessFlags = accessFlags;
                OnPropertyChanged("AccessFlags");
            }
        }

        public string ConnectionFlags
        {
            get { return _adminConfiguration.ConnectionFlagsString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _adminConfiguration.ConnectionFlags = 0;
                    OnPropertyChanged("ConnectionFlags");
                    return;
                }

                ConnectionFlag connectionFlags = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    string enumValue = value.Substring(i, 1);
                    try
                    {
                        if (Enum.IsDefined(typeof(ConnectionFlag), enumValue))
                        {
                            connectionFlags |= (ConnectionFlag)Enum.Parse(typeof(ConnectionFlag), enumValue);
                        }
                    }
                    catch
                    {
                    }
                }

                _adminConfiguration.ConnectionFlags = connectionFlags;
                OnPropertyChanged("ConnectionFlags");
            }
        }

        public string StartDate
        {
            get { return _adminConfiguration.StartDate.ToShortDateString(); }
            set
            {
                DateTime parsedValue;
                if (DateTime.TryParse(value, out parsedValue))
                {
                    _adminConfiguration.StartDate = parsedValue;
                    OnPropertyChanged("StartDate");
                }
            }
        }

        public string EndDate
        {
            get { return _adminConfiguration.EndDate.ToShortDateString(); }
            set
            {
                DateTime parsedValue;
                if (DateTime.TryParse(value, out parsedValue))
                {
                    _adminConfiguration.EndDate = parsedValue;
                    OnPropertyChanged("EndDate");
                }
            }
        }

        public string StartTime
        {
            get { return _adminConfiguration.StartTime.ToShortTimeString(); }
            set
            {
                DateTime parsedValue;
                if (DateTime.TryParse(value, out parsedValue))
                {
                    _adminConfiguration.StartTime = parsedValue;
                    OnPropertyChanged("StartTime");
                }
            }
        }

        public string EndTime
        {
            get { return _adminConfiguration.EndTime.ToShortTimeString(); }
            set
            {
                DateTime parsedValue;
                if (DateTime.TryParse(value, out parsedValue))
                {
                    _adminConfiguration.EndTime = parsedValue;
                    OnPropertyChanged("EndTime");
                }
            }
        }

        #endregion

        #region methods

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ExecutedSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Validate())
            {
                return;
            }

            AdminConfiguration.Copy(_adminConfiguration, _originalAdminConfiguration);
            if (_originalAdminConfiguration.IsNew)
            {
                if (_configurationContainerHolder.ConfigurationContainer.Admins.Count == 0)
                {
                    _originalAdminConfiguration.Id = 1;
                }
                else
                {
                    _originalAdminConfiguration.Id = _configurationContainerHolder.ConfigurationContainer.Admins.Max(s => s.Id) + 1;
                }
            }

            DialogResult = true;
            Close();
        }

        private void ExecutedCancelCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool Validate()
        {
            var errors = new StringBuilder();
            if (_adminConfiguration.StartDate >= _adminConfiguration.EndDate)
            {
                errors.AppendLine(string.Format(Properties.Strings.X_less_or_equals_Y, Properties.Strings.Start_date, Properties.Strings.End_date));
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), Properties.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void AccessFlagsTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForFlagsEnum(e.Text, _adminConfiguration.AccessFlags,
                                                   (flag, accessFlag) => (flag & accessFlag) > 0,
                                                   (flag, accessFlag) => flag | accessFlag);
        }

        private void AccessFlagsTextBoxOnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                var text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowedForFlagsEnum(text, _adminConfiguration.AccessFlags,
                                               (flags1, flags2) => (flags1 & flags2) > 0,
                                               (flags1, flags2) => flags1 | flags2))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand(); 
            }
        }

        private void ConnectionFlagsTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForFlagsEnum(e.Text, _adminConfiguration.ConnectionFlags,
                                                   (flags1, flags2) => (flags1 & flags2) > 0,
                                                   (flags1, flags2) => flags1 | flags2);
        }

        private void ConnectionFlagsTextBoxOnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                var text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowedForFlagsEnum(text, _adminConfiguration.ConnectionFlags,
                                               (flags1, flags2) => (flags1 & flags2) > 0,
                                               (flags1, flags2) => flags1 | flags2))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowedForFlagsEnum<T>(string text, T currentFlagsEnumValue, Func<T, T, bool> compareFunc, Func<T, T, T> assignFunc) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (string.IsNullOrEmpty(text) || !typeof(T).IsEnum)
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                string enumValueString = text.Substring(i, 1);
                try
                {
                    if (Enum.IsDefined(typeof(T), enumValueString))
                    {
                        var enumValue = (T)Enum.Parse(typeof(T), enumValueString);
                        if (compareFunc(currentFlagsEnumValue, enumValue))
                        {
                            return false;
                        }
                        currentFlagsEnumValue = assignFunc(currentFlagsEnumValue, enumValue);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
