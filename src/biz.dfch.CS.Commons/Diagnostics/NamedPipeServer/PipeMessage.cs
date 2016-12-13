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
using System.Text;

namespace biz.dfch.CS.Commons.Diagnostics.NamedPipeServer
{
    public class PipeMessage
    {
        public const char DELIMITER = '|';

        public TraceEventType TraceEventType;

        public string Source;

        public string Message;

        public PipeMessage()
        {
            // ctor for composing messages when setting properties indepedently
        }
            
        public PipeMessage(TraceEventType traceEventType, string source, string message)
        {
            Contract.Requires(Enum.IsDefined(typeof(TraceEventType), traceEventType));
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(message));

            TraceEventType = traceEventType;
            Source = source;
            Message = message;
        }
            
        public PipeMessage(string composedMessage)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(composedMessage));

            var startIndex = 0;

            var firstDelimiter = composedMessage.IndexOf(DELIMITER, startIndex);
            var eventTypeValue = composedMessage.Substring(startIndex, firstDelimiter);
            TraceEventType traceEventType;
            var isValidTraceEventType = Enum.TryParse(eventTypeValue, true, out traceEventType);
            TraceEventType = traceEventType;

            startIndex += firstDelimiter.ToString().Length + eventTypeValue.Length;
            var secondDelimiter = composedMessage.IndexOf(DELIMITER, startIndex);
            Source = composedMessage.Substring(startIndex, secondDelimiter - startIndex);

            startIndex = secondDelimiter + DELIMITER.ToString().Length;
            Message = composedMessage.Substring(startIndex);
        }

        public override string ToString()
        {
            Contract.Requires(!string.IsNullOrEmpty(Message));
            Contract.Requires(!string.IsNullOrEmpty(Source));

            var sb = new StringBuilder(DELIMITER);

            sb.Append(TraceEventType);
            sb.Append(DELIMITER);
            sb.Append(Source);
            sb.Append(DELIMITER);
            sb.Append(Message);

            return sb.ToString();
        }

        public bool IsValid()
        {
            var result = string.IsNullOrWhiteSpace(Message);
            if (result)
            {
                return false;
            }

            result = string.IsNullOrWhiteSpace(Source);
            if (result)
            {
                return false;
            }

            result = Enum.IsDefined(typeof(TraceEventType), TraceEventType);
            return result;
        }
    }
}