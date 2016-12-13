using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace biz.dfch.CS.Commons.Diagnostics.NamedPipeServer
{
    public class Item
    {
        public const char DELIMITER = MessageHandler.DELIMITER;

        public TraceEventType TraceEventType;

        public string Source;

        public string Message;

        public Item()
        {
            // ctor for composing messages when setting properties indepedently
        }
            
        public Item(TraceEventType traceEventType, string source, string message)
        {
            Contract.Requires(Enum.IsDefined(typeof(TraceEventType), traceEventType));
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(message));

            TraceEventType = traceEventType;
            Source = source;
            Message = message;
        }
            
        public Item(string composedMessage)
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