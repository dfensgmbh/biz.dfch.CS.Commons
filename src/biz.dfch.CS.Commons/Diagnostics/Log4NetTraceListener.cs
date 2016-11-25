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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Diagnostics.Log4Net;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class Log4NetTraceListener : TraceListener
    {
        private const char DELIMITER = '|';
        private static readonly object[] _emptyArgs = {};

        private const string CLASS_NAME_LOG_MANAGER = "LogManager";
        private const string METHOD_NAME_GET_LOGGER = "GetLogger";
        
        private const string CLASS_NAME_XML_CONFIGURATOR = "XmlConfigurator";
        private const string METHOD_NAME_XML_CONFIGURATOR = "Configure";

        private static readonly Lazy<Assembly> _assembly = new Lazy<Assembly>(() =>
        {
            try
            {
                //var assembly = typeof(Log4NetTraceListener).Assembly;
                //var location = assembly.Location;
                //Contract.Assert(null != location);

                //var directoryName = System.IO.Path.GetDirectoryName(location);
                //Contract.Assert(null != directoryName);

                //var log4NetLocation = System.IO.Path.Combine(directoryName, "log4net.dll");
                //Contract.Assert(System.IO.File.Exists(log4NetLocation), log4NetLocation);
                var log4NetLocation = "log4net.dll";

                var log4Net = Assembly.LoadFrom(log4NetLocation);
                Contract.Assert(null != log4Net);

                return log4Net;
            }
            catch (Exception)
            {
                // N/A
                return default(Assembly);
            }
        });

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
            
        public static Assembly Assembly
        {
            get { return _assembly.Value; }
        }

        public static ILog GetLogger(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(null != Contract.Result<ILog>());

            var logManager = Assembly.DefinedTypes.FirstOrDefault(e => e.Name == CLASS_NAME_LOG_MANAGER);
            Contract.Assert(null != logManager, CLASS_NAME_LOG_MANAGER);

            var methodInfo = logManager.GetMethod(METHOD_NAME_GET_LOGGER, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
            Contract.Assert(null != methodInfo, METHOD_NAME_GET_LOGGER);

            var loggerInstance = methodInfo.Invoke(null, new object[] { name });
            Contract.Assert(null != loggerInstance);

            return new Log4Net.Log4Net(loggerInstance);
        }

        public static void Configure()
        {
            var xmlConfigurator = Assembly.DefinedTypes.FirstOrDefault(e => e.Name == CLASS_NAME_XML_CONFIGURATOR);
            Contract.Assert(null != xmlConfigurator, CLASS_NAME_XML_CONFIGURATOR);

            var methodInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null);
            Contract.Assert(null != methodInfo, METHOD_NAME_XML_CONFIGURATOR);

            methodInfo.Invoke(null, new object[] { });
        }

        public static void Configure(FileInfo configFile)
        {
            Contract.Requires(null != configFile);

            var xmlConfigurator = Assembly.DefinedTypes.FirstOrDefault(e => e.Name == CLASS_NAME_XML_CONFIGURATOR);
            Contract.Assert(null != xmlConfigurator, CLASS_NAME_XML_CONFIGURATOR);

            if (File.Exists(configFile.FullName))
            {
                var methodInfoFileInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(FileInfo) }, null);
                Contract.Assert(null != methodInfoFileInfo, METHOD_NAME_XML_CONFIGURATOR);

                methodInfoFileInfo.Invoke(null, new object[] { configFile });
            }
            else
            {
                var methodInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null);
                Contract.Assert(null != methodInfo, METHOD_NAME_XML_CONFIGURATOR);

                methodInfo.Invoke(null, new object[] { });
            }
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
