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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class Log4NetTraceListener : TraceListener
    {
        private const char DELIMITER = '|';
        private static readonly object[] _emptyArgs = { };

        public Log4NetTraceListener()
            : base()
        {
            // N/A
        }

        public Log4NetTraceListener(string name)
            : base(name)
        {
            // N/A
        }
            
        public override void Write(string message)
        {
            TraceImpl(message, false);
        }

        public override void WriteLine(string message)
        {
            TraceImpl(message, true);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, string.Empty);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, string.Empty, _emptyArgs);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            var activityId = Trace.CorrelationManager.ActivityId;
            
            var sb = new StringBuilder(activityId.ToString());
            if (0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(DELIMITER);
                sb.Append(eventCache.DateTime.ToString("O"));
            }

            sb.Append(DELIMITER);
            sb.Append(source);

            if (!string.IsNullOrEmpty(format))
            {
                sb.Append(DELIMITER);
                
                if (0 < args.Length)
                {
                    sb.AppendFormat(format, args);
                }
                else
                {
                    sb.Append(format);
                }
            }

            sb.AppendLine();

            Trace.Write(sb.ToString());              
        }
            
        internal void TraceImpl(string message, bool appendNewLine, params string[] args)
        {
            var activityId = Trace.CorrelationManager.ActivityId;
            
            var sb = new StringBuilder(activityId.ToString());
            if (0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(DELIMITER);
                sb.Append(DateTimeOffset.Now.ToString("O"));
            }

            sb.Append(DELIMITER);
            sb.AppendFormat(message, args);

            if (appendNewLine)
            {
                Trace.WriteLine(sb.ToString());
            }
            else
            {
                Trace.Write(sb.ToString());
            }
        }
    }
}
