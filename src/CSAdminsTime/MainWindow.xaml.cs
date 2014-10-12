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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CSAdminsTime.Common;
using CSAdminsTime.Properties;

namespace CSAdminsTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IConfigurationContainerHolder
    {
        #region fields

        private ObservableCollection<ServerConfiguration> _servers;
        private ObservableCollection<AdminConfiguration> _admins;
        private ObservableCollection<AdminConfiguration> _relatedAdmins;
        private ObservableCollection<AdminConfiguration> _adminsForAdd;
        private readonly Processor _processor = new Processor();

        #endregion

        #region constructors

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            _processor.Stopped += ProcessorOnStopped;
            Logger.Instance.LogEntryAdded += LoggerOnLogEntryAdded;
        }

        #endregion

        #region properties

        private bool CanSave { get; set; }
        private bool IsStarted { get; set; }

        public ConfigurationContainer ConfigurationContainer
        {
            get { return ((App) Application.Current).ConfigurationContainer; }
        }

        public ObservableCollection<ServerConfiguration> Servers
        {
            get
            {
                if (_servers == null)
                {
                    _servers = new ObservableCollection<ServerConfiguration>(ConfigurationContainer.Servers);
                }
                return _servers;
            }
        }

        public ObservableCollection<AdminConfiguration> Admins
        {
            get
            {
                if (_admins == null)
                {
                    _admins = new ObservableCollection<AdminConfiguration>(ConfigurationContainer.Admins);
                }
                return _admins;
            }
        }

        public ObservableCollection<AdminConfiguration> RelatedAdmins
        {
            get
            {
                if (_relatedAdmins == null)
                {
                    _relatedAdmins = new ObservableCollection<AdminConfiguration>();
                }
                return _relatedAdmins;
            }
        }

        public ObservableCollection<AdminConfiguration> AdminsForAdd
        {
            get
            {
                if (_adminsForAdd == null)
                {
                    _adminsForAdd = new ObservableCollection<AdminConfiguration>();
                }
                return _adminsForAdd;
            }
        }
        
        #endregion

        #region methods

        private void ReloadRelatedAdmins()
        {
            var selectedServerConfiguration = ServersGrid.SelectedItem as ServerConfiguration;
            if (selectedServerConfiguration == null)
            {
                _relatedAdmins.Clear();
                _adminsForAdd.Clear();
                return;
            }

            _relatedAdmins.Clear();
            _adminsForAdd.Clear();

            foreach (AdminConfiguration adminConfiguration in ConfigurationContainer.Admins)
            {
                if (selectedServerConfiguration.AdminIds.Contains(adminConfiguration.Id))
                {
                    _relatedAdmins.Add(adminConfiguration);
                }
                else
                {
                    _adminsForAdd.Add(adminConfiguration);
                }
            }
        }

        private void Save()
        {
            SettingsManager.Write(ConfigurationContainer);
            CanSave = false;
        }

        public void Start()
        {
            IsStarted = true;
            Logger.Instance.WriteInfo(Strings.Execution_started);
            _processor.Start();
        }

        private void Stop()
        {
            _processor.Stop();
        }

        private void ProcessorOnStopped(object sender, EventArgs e)
        {
            IsStarted = false;
            Logger.Instance.WriteInfo(Strings.Execution_stopped);
        }

        private void ServersGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadRelatedAdmins();
        }

        private void LoggerOnLogEntryAdded(LogEntry logEntry)
        {
            Brush textColor;
            switch (logEntry.LogEntryType)
            {
                case LogEntryType.Info:
                    textColor = Brushes.Gray;
                    break;
                case LogEntryType.Success:
                    textColor = Brushes.Green;
                    break;
                case LogEntryType.Warning:
                    textColor = Brushes.Orange;
                    break;
                case LogEntryType.Error:
                    textColor = Brushes.Red;
                    break;
                default:
                    textColor = Brushes.Gray;
                    break;
            }

            string logEntryTypeString = Enum.GetName(typeof (LogEntryType), logEntry.LogEntryType);

            Action textUpdateDelegate = delegate
            {
                var textRange = new TextRange(LogsRichTextBox.Document.ContentEnd, LogsRichTextBox.Document.ContentEnd);
                textRange.Text = string.Format("[{0}] [{1}] - {2}{3}", logEntry.Created, logEntryTypeString, logEntry.Message, Environment.NewLine);
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, (Brush)textColor);
                LogsRichTextBox.ScrollToEnd();
            };
            LogsRichTextBox.Dispatcher.Invoke(DispatcherPriority.Normal, textUpdateDelegate);
        }

        #endregion
    }

    #region commands

    public partial class MainWindow: Window
    {
        #region properties

        public static RoutedCommand SaveCommand = new RoutedCommand();
        public static RoutedCommand SettingsCommand = new RoutedCommand();
        public static RoutedCommand StartCommand = new RoutedCommand();
        public static RoutedCommand StopCommand = new RoutedCommand();
        public static RoutedCommand AboutCommand = new RoutedCommand();
        public static RoutedCommand AddServerCommand = new RoutedCommand();
        public static RoutedCommand EditServerCommand = new RoutedCommand();
        public static RoutedCommand DeleteServerCommand = new RoutedCommand();
        public static RoutedCommand AddAdminCommand = new RoutedCommand();
        public static RoutedCommand EditAdminCommand = new RoutedCommand();
        public static RoutedCommand DeleteAdminCommand = new RoutedCommand();
        public static RoutedCommand AddRelatedAdminCommand = new RoutedCommand();
        public static RoutedCommand RemoveRelatedAdminCommand = new RoutedCommand();

        #endregion

        #region methods

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CanSave)
            {
                base.OnClosing(e);
                return;
            }

            MessageBoxResult msgBoxResult = MessageBox.Show(Strings.You_have_unsaved_settings, string.Format("{0}?", Strings.Save_configuration), MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (msgBoxResult == MessageBoxResult.Yes)
            {
                Save();
            }
        }

        private void ExecutedSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanSave;
        }

        private void ExecutedSettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var editSettings = new EditSettings(this);
            bool? dialogResult = editSettings.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                CanSave = true;
            }
        }

        private void ExecutedStartCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Start();
        }

        private void CanExecuteStartCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsStarted;
        }

        private void ExecutedStopCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Stop();
        }

        private void CanExecuteStopCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsStarted;
        }

        private void ExecutedAboutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void ExecutedAddServerCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var serverConfiguration = new ServerConfiguration();
            var addEditServer = new AddEditServer(this, serverConfiguration);
            var dialogResult = addEditServer.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                ConfigurationContainer.Servers.Add(serverConfiguration);
                Servers.Add(serverConfiguration);
                ReloadRelatedAdmins();
                CanSave = true;
            }
        }

        private void ExecutedEditServerCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var serverConfiguration = e.Parameter as ServerConfiguration;
            if (serverConfiguration == null)
            {
                return;
            }

            var addEditServer = new AddEditServer(this, serverConfiguration);
            var dialogResult = addEditServer.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                int index = Servers.IndexOf(serverConfiguration);
                Servers.RemoveAt(index);
                Servers.Insert(index, serverConfiguration);
                ReloadRelatedAdmins();
                CanSave = true;
            }
        }

        private void ExecutedDeleteServerCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var serverConfiguration = e.Parameter as ServerConfiguration;
            if (serverConfiguration == null)
            {
                return;
            }
            ConfigurationContainer.Servers.Remove(serverConfiguration);
            Servers.Remove(serverConfiguration);
            ReloadRelatedAdmins();
            CanSave = true;
        }

        private void ExecutedAddAdminCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var adminConfiguration = new AdminConfiguration();
            adminConfiguration.StartDate = DateTime.Now;
            adminConfiguration.EndDate = DateTime.Now.AddDays(1);
            adminConfiguration.StartTime = DateTime.Now.Date;
            adminConfiguration.EndTime = DateTime.Now.Date;

            var addEditAdmin = new AddEditAdmin(this, adminConfiguration);
            var dialogResult = addEditAdmin.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                ConfigurationContainer.Admins.Add(adminConfiguration);
                Admins.Add(adminConfiguration);
                ReloadRelatedAdmins();
                CanSave = true;
            }
        }

        private void ExecutedEditAdminCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var adminConfiguration = e.Parameter as AdminConfiguration;
            if (adminConfiguration == null)
            {
                return;
            }

            var addEditAdmin = new AddEditAdmin(this, adminConfiguration);
            var dialogResult = addEditAdmin.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                int index = Admins.IndexOf(adminConfiguration);
                Admins.RemoveAt(index);
                Admins.Insert(index, adminConfiguration);
                ReloadRelatedAdmins();
                CanSave = true;
            }
        }

        private void ExecutedDeleteAdminCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var adminConfiguration = e.Parameter as AdminConfiguration;
            if (adminConfiguration == null)
            {
                return;
            }
            ConfigurationContainer.Admins.Remove(adminConfiguration);
            Admins.Remove(adminConfiguration);
            foreach (ServerConfiguration serverConfiguration in ConfigurationContainer.Servers)
            {
                serverConfiguration.AdminIds.Remove(adminConfiguration.Id);
            }
            ReloadRelatedAdmins();
            CanSave = true;
        }

        private void ExecutedAddRelatedAdminCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var items = e.Parameter as IList;
            if (items == null || items.Count == 0)
            {
                return;
            }

            var serverConfiguration = ServersGrid.SelectedItem as ServerConfiguration;
            if (serverConfiguration == null)
            {
                return;
            }

            var adminConfigurations = items.Cast<AdminConfiguration>();
            serverConfiguration.AdminIds.AddRange(adminConfigurations.Select(a => a.Id));
            ReloadRelatedAdmins();
            CanSave = true;
        }

        private void CanExecuteAddRelatedAdminCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var items = e.Parameter as IList;
            if (items == null || items.Count == 0)
            {
                e.CanExecute = false;
                return;
            }

            var serverConfiguration = ServersGrid.SelectedItem as ServerConfiguration;
            if (serverConfiguration == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = true;
        }

        private void ExecutedRemoveRelatedAdminCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var items = e.Parameter as IList;
            if (items == null || items.Count == 0)
            {
                return;
            }

            var serverConfiguration = ServersGrid.SelectedItem as ServerConfiguration;
            if (serverConfiguration == null)
            {
                return;
            }

            var adminConfigurations = items.Cast<AdminConfiguration>();
            serverConfiguration.AdminIds.RemoveAll(id => adminConfigurations.Select(a => a.Id).Contains(id));
            ReloadRelatedAdmins();
            CanSave = true;
        }

        private void CanExecuteRemoveRelatedAdminCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var items = e.Parameter as IList;
            if (items == null || items.Count == 0)
            {
                e.CanExecute = false;
                return;
            }

            var serverConfiguration = ServersGrid.SelectedItem as ServerConfiguration;
            if (serverConfiguration == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = true;
        }

        #endregion
    }

    #endregion

    public interface IConfigurationContainerHolder
    {
        ConfigurationContainer ConfigurationContainer { get; }
    }
}