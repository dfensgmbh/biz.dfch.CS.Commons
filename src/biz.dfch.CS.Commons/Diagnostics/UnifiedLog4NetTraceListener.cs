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

using System.Diagnostics;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class UnifiedLog4NetTraceListener : Log4NetTraceListener
    {
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (null != base.Filter && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) { return; }

            var logger = GetLogger(Logger.DEFAULT_TRACESOURCE_NAME);

            switch (eventType)
            {
                case TraceEventType.Critical:
                    if (!logger.IsFatalEnabled) { return; }
                    logger.Fatal(TraceEventFormatter(eventCache, source, id, format, args));
                    break;
                case TraceEventType.Error:
                    if (!logger.IsErrorEnabled) { return; }
                    logger.Error(TraceEventFormatter(eventCache, source, id, format, args));
                    break;
                case TraceEventType.Warning:
                    if (!logger.IsWarnEnabled) { return; }
                    logger.Warn(TraceEventFormatter(eventCache, source, id, format, args));
                    break;
                case TraceEventType.Information:
                    if (!logger.IsInfoEnabled) { return; }
                    logger.Info(TraceEventFormatter(eventCache, source, id, format, args));
                    break;
                default:
                    if (!logger.IsDebugEnabled) { return; }
                    logger.Debug(TraceEventFormatter(eventCache, source, id, format, args));
                    break;
            }
        }

    }
}
