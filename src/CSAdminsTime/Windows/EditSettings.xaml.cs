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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CSAdminsTime.Common;

namespace CSAdminsTime
{
    /// <summary>
    /// Interaction logic for EditSettings.xaml
    /// </summary>
    public partial class EditSettings :  Window, INotifyPropertyChanged
    {
        #region fields

        private int _updatePeriod;
        private LanguageConfiguration _language;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IConfigurationContainerHolder _configurationContainerHolder;

        #endregion

        #region constructor

        public EditSettings(IConfigurationContainerHolder configurationContainerHolder)
        {
            _configurationContainerHolder = configurationContainerHolder;
            _updatePeriod = _configurationContainerHolder.ConfigurationContainer.UpdatePeriod;
            _language = _configurationContainerHolder.ConfigurationContainer.Language;

            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region properties

        public int UpdatePeriod
        {
            get { return _updatePeriod; }
            set
            {
                _updatePeriod = value;
                OnPropertyChanged("UpdatePeriod");
            }
        }

        public LanguageConfiguration LanguageConfiguration
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged("LanguageConfiguration");
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

            bool settingsChanged = false;

            if (_configurationContainerHolder.ConfigurationContainer.UpdatePeriod != _updatePeriod)
            {
                settingsChanged = true;
                _configurationContainerHolder.ConfigurationContainer.UpdatePeriod = _updatePeriod;
            }

            if (_configurationContainerHolder.ConfigurationContainer.Language != _language)
            {
                settingsChanged = true;
                _configurationContainerHolder.ConfigurationContainer.Language = _language;

                string message = Properties.Strings.ResourceManager.GetString("Save_and_restart_application_for_applying_language", CultureInfo.CreateSpecificCulture(_language.ToString()));
                MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (!settingsChanged)
            {
                ExecutedCancelCommand(sender, e);
                return;
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
            if (_updatePeriod < ConfigurationContainer.MinUpdatePeriod)
            {
                errors.AppendLine(string.Format(Properties.Strings.X_cant_be_less_than_Y, Properties.Strings.Update_period, ConfigurationContainer.MinUpdatePeriod));
            }

            if (_updatePeriod > ConfigurationContainer.MaxUpdatePeriod)
            {
                errors.AppendLine(string.Format(Properties.Strings.X_cant_be_bigger_than_Y, Properties.Strings.Update_period, ConfigurationContainer.MaxUpdatePeriod));
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), Properties.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion
    }
}
