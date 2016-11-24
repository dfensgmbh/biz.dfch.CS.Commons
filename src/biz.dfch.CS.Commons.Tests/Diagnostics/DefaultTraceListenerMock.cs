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

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Commons.Tests.Diagnostics
{
    public class DefaultTraceListenerMock : TraceListener
    {
        private string initializeData;

        public DefaultTraceListenerMock()
            : base()
        {
            // N/A
        }

        public DefaultTraceListenerMock(string initializeData)
            : base(initializeData)
        {
            this.initializeData = initializeData;
        }
            
        public override void Write(string message)
        {
            var writeMethodIsImplemented = false;
            Contract.Assert(writeMethodIsImplemented, message);
        }

        public override void WriteLine(string message)
        {
            var writeLineMethodIsImplemented = false;
            Contract.Assert(writeLineMethodIsImplemented, message);
        }
    }
}
