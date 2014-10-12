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
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CSAdminsTime.Common;
using CSAdminsTime.Properties;

namespace CSAdminsTime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields

        private const string ExceptionsFolderName = "exceptions";
        private const string ErrorsFolderName = "errors";
        private const string ErrorsFileNameTemplate = "{0}.log";

        #endregion

        public ConfigurationContainer ConfigurationContainer { get; private set; }

        public App()
        {
            Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Current.Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeServices();
            
            var culture = new CultureInfo("en-Gb");
            culture.DateTimeFormat.ShortDatePattern = Misc.ShortDatePattern;
            culture.DateTimeFormat.ShortTimePattern = Misc.ShortTimePattern;
            Thread.CurrentThread.CurrentCulture = culture;
            
            ConfigurationContainer configurationContainer = SettingsManager.Read();

            if (configurationContainer == null)
            {
                configurationContainer = new ConfigurationContainer();
                configurationContainer.UpdatePeriod = ConfigurationContainer.DefaultUpdatePeriod;
                configurationContainer.Servers = new List<ServerConfiguration>();
                configurationContainer.Admins = new List<AdminConfiguration>();

                if (CultureInfo.CurrentUICulture.Name.StartsWith(LanguageConfiguration.en.ToString()))
                {
                    configurationContainer.Language = LanguageConfiguration.en;
                }
                if (CultureInfo.CurrentUICulture.Name.StartsWith(LanguageConfiguration.ru.ToString()))
                {
                    configurationContainer.Language = LanguageConfiguration.ru;
                }
            }

            Misc.SetCurrentUiCulture(configurationContainer.Language);

            ConfigurationContainer = configurationContainer;
        }

        private void InitializeServices()
        {
            ServiceLocator.SetService(typeof(IExceptionLogger<ExceptionContainer>), new ExceptionLogger());

            var exceptionLogger = ServiceLocator.GetLogger();
            exceptionLogger.Initialize(ExceptionsFolderName, ErrorsFolderName, ErrorsFileNameTemplate);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ServiceLocator.GetLogger().WriteException(e.Exception);
                e.Handled = true;
            }
            catch
            {
                MessageBox.Show(Strings.Cant_write_exception, Strings.Critical_error);
            }
        }

        private void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            try
            {
                ServiceLocator.GetLogger().WriteException(e.Exception);
                e.RequestCatch = false;
            }
            catch
            {
                MessageBox.Show(Strings.Cant_write_exception, Strings.Critical_error);
            }
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ServiceLocator.GetLogger().WriteException(e.ExceptionObject as Exception, e.IsTerminating);
            }
            catch
            {
                MessageBox.Show(Strings.Cant_write_exception, Strings.Critical_error);
            }
        }
    }
}
