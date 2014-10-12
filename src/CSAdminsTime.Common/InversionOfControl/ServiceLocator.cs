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

namespace CSAdminsTime.Common
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static readonly object _synchronizationObject = new object();

        public static void SetService(Type typeOfService, object implementation)
        {
            if (typeOfService == null)
            {
                throw new ArgumentNullException("typeOfService");
            }
            if (implementation == null)
            {
                throw new ArgumentNullException("implementation");
            }
            
            lock (_synchronizationObject)
            {
                if (_services.ContainsKey(typeOfService))
                {
                    throw new Exception(string.Format("Type \"{0}\" already registered", typeOfService.FullName));
                }

                _services.Add(typeOfService, implementation);
            }
        }

        public static object GetService(Type typeOfService)
        {
            if (typeOfService == null)
            {
                throw new ArgumentNullException("typeOfService");
            }

            lock (_synchronizationObject)
            {
                if (!_services.ContainsKey(typeOfService))
                {
                    throw new Exception(string.Format("Type \"{0}\" is not registered", typeOfService.FullName));
                }

                return _services[typeOfService];
            }
        }

        public static T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public static IExceptionLogger<T> GetLogger<T>() where T : class, IExceptionContainer, new()
        {
            return GetService<IExceptionLogger<T>>();
        }

        public static IExceptionLogger<ExceptionContainer> GetLogger()
        {
            return GetLogger<ExceptionContainer>();
        }
    }
}