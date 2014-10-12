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

namespace CSAdminsTime.Common
{
    [Flags]
    public enum AccessFlag
    {
        a = 1 << 0,
        b = 1 << 1,
        c = 1 << 2,
        d = 1 << 3,
        e = 1 << 4,
        f = 1 << 5,
        g = 1 << 6,
        h = 1 << 7,
        i = 1 << 8,
        j = 1 << 9,
        k = 1 << 10,
        l = 1 << 11,
        m = 1 << 12,
        n = 1 << 13,
        o = 1 << 14,
        p = 1 << 15,
        q = 1 << 16,
        r = 1 << 17,
        s = 1 << 18,
        t = 1 << 19,
        u = 1 << 20,
        z = 1 << 21
    }
}