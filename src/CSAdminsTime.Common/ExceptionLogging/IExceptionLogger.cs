﻿// Copyright 2014 Sergey Rumyantsev http://www.digitalandsmart.com
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

namespace CSAdminsTime.Common
{
    public interface IExceptionLogger<T> where T : class, IExceptionContainer, new()
    {
        void Initialize(string exceptionsFolderName, string errorsFolderName, string errorsFileNameTemplate);
        T WriteException(Exception exception, bool isCritical = false);
        T Convert(Exception exception, bool isCritical = false);
        void SerializeAndWrite(string exceptionFilePath, T exceptionContainer);
        T ReadAndDeserialize(string exceptionFilePath);
    }
}