﻿/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using biz.dfch.CS.Commons.Converters;

namespace biz.dfch.CS.Commons.Tests.Converters
{
    public enum EnumExample
    {
        [StringValue("None")]
        None
        ,
        [StringValue("Trivial")]
        Trivial
        ,
        [StringValue("Regular")]
        Regular
        ,
        [StringValue("Important")]
        Important
        ,
        [StringValue("Critical")]
        Critical
    }
}
