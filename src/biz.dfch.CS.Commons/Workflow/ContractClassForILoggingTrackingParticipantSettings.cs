﻿/**
 * Copyright 2017 d-fens GmbH
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
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Commons.Workflow
{
    [ContractClassFor(typeof(ILoggingTrackingParticipantSettings))]
    public abstract class ContractClassForILoggingTrackingParticipantSettings : ILoggingTrackingParticipantSettings
    {
        private string _traceSourceName;
        public string TraceSourceName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return _traceSourceName;
            }
            set
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(value));
                _traceSourceName = value;
            }
        }

        public TrackingProfile TrackingProfile { get; set; }
    }
}
