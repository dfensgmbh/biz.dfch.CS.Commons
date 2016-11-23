/**
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

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class TraceSource : System.Diagnostics.TraceSource
    {
        public const string EXCEPTION_MESSAGE_FORMAT = "{0}@{1}: '{2}'\r\n[{3}]\r\n[{4}]";
        public const string EXCEPTION_MESSAGE_BASE_FORMAT = "{0}@{1}: '{2}'\r\n[{3}]";

        public TraceSource(string name) 
            : base(name)
        {
            // N/A
        }

        public TraceSource(string name, SourceLevels defaultLevel) 
            : base(name, defaultLevel)
        {
            // N/A
        }

        public virtual void TraceException(Exception ex)
        {
            Contract.Requires(null != ex);

            if(SourceLevels.Error != (Switch.Level & SourceLevels.Error))
            {
                return;
            }

            TraceEvent(TraceEventType.Error, ex.GetType().GetHashCode(), EXCEPTION_MESSAGE_BASE_FORMAT, ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace);
        }

        public virtual void TraceException(Exception ex, string message)
        {
            Contract.Requires(null != ex);
            Contract.Requires(null != message);

            if(SourceLevels.Error != (Switch.Level & SourceLevels.Error))
            {
                return;
            }

            TraceEvent(TraceEventType.Error, ex.GetType().GetHashCode(), EXCEPTION_MESSAGE_FORMAT, ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace);
        }
    }
}
