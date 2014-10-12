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
using System.Windows;
using System.Windows.Input;
using CSAdminsTime.Common;

namespace CSAdminsTime
{
    /// <summary>
    /// Interaction logic for AddEditServer.xaml
    /// </summary>
    public partial class AddEditServer : Window, INotifyPropertyChanged
    {
        #region fields

        private readonly ServerConfiguration _originalServerConfiguration;
        private readonly ServerConfiguration _serverConfiguration = new ServerConfiguration();
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IConfigurationContainerHolder _configurationContainerHolder;
        
        #endregion

        #region constructor

        public AddEditServer(IConfigurationContainerHolder configurationContainerHolder,ServerConfiguration serverConfiguration)
        {
            _configurationContainerHolder = configurationContainerHolder;
            _originalServerConfiguration = serverConfiguration;
            ServerConfiguration.Copy(_originalServerConfiguration, _serverConfiguration);
            DataContext = this;
            InitializeComponent();

            Title = _serverConfiguration.IsNew ? Properties.Strings.Create_server : Properties.Strings.Modify_server;
        }

        #endregion

        #region properties

        public string ServerTitle
        {
            get { return _serverConfiguration.Title; }
            set
            {
                _serverConfiguration.Title = value;
                OnPropertyChanged("ServerTitle");
            }
        }

        public string Path
        {
            get { return _serverConfiguration.Path; }
            set
            {
                _serverConfiguration.Path = value;
                OnPropertyChanged("Path");
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
            ServerConfiguration.Copy(_serverConfiguration, _originalServerConfiguration);
            if (_originalServerConfiguration.IsNew)
            {
                if (_configurationContainerHolder.ConfigurationContainer.Servers.Count == 0)
                {
                    _originalServerConfiguration.Id = 1;
                }
                else
                {
                    _originalServerConfiguration.Id = _configurationContainerHolder.ConfigurationContainer.Servers.Max(s => s.Id) + 1;
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

        #endregion
    }
}
