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
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using biz.dfch.CS.Commons.Diagnostics.Log4Net;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class Log4NetTraceListener : TraceListener
    {
        private const char DELIMITER = '|';
        private static readonly object[] _emptyArgs = {};
        private const int DEFAULT_TRACE_ID = short.MaxValue;
        private const string ISO8601_FORMAT_STRING = "O";
        private const string FAIL_MESSAGE_TEMPLATE = "{0} ({1})";

        private const BindingFlags BINDING_FLAGS = BindingFlags.Static | BindingFlags.Public;

        private const string ASSEMBLY_BASENAME = "log4net";
        private const string ASSEMBLY_NAME = ASSEMBLY_BASENAME + ".dll";

        private const string CLASS_NAME_LOG_MANAGER = "LogManager";
        private const string METHOD_NAME_GET_LOGGER = "GetLogger";

        private const string CLASS_NAME_XML_CONFIGURATOR = "XmlConfigurator";
        private const string METHOD_NAME_XML_CONFIGURATOR = "Configure";

        private static readonly ConcurrentDictionary<string, ILog> _loggers =
            new ConcurrentDictionary<string, ILog>();

        private static readonly Lazy<Assembly> _assembly = new Lazy<Assembly>(() =>
        {
            Contract.Ensures(null != Contract.Result<Assembly>(), ASSEMBLY_NAME);

            var result = default(Assembly);
            try
            {
                result = Assembly.Load(ASSEMBLY_BASENAME);
                if (null != result)
                {
                    return result;
                }
            }
            catch (Exception)
            {
                // N/A
            }

            var pathFromThisAssembly = GetPathFromAssembly(typeof(Log4NetTraceListener).Assembly);
            var assemblyNameInThisAssembly = Path.Combine(pathFromThisAssembly, ASSEMBLY_NAME);
            if (File.Exists(assemblyNameInThisAssembly))
            {
                try
                {
                    result = Assembly.LoadFrom(ASSEMBLY_NAME);
                    if (null != result)
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // N/A
                }
            }

            var pathFromExecutingAssembly = GetPathFromAssembly(Assembly.GetExecutingAssembly());
            var assemblyNameInExecutingAssembly = Path.Combine(pathFromExecutingAssembly, ASSEMBLY_NAME);
            if (File.Exists(assemblyNameInExecutingAssembly))
            {
                try
                {
                    result = Assembly.LoadFrom(ASSEMBLY_NAME);
                    if (null != result)
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // N/A
                }
            }

            return result;
        });

        public const string SUPPORTED_ATTRIBUTE_LOGGER = "logger";

        public Log4NetTraceListener()
            : base()
        {
            Configure();
        }

        public Log4NetTraceListener(string name)
            : base(name)
        {

            if (Attributes.ContainsKey(SUPPORTED_ATTRIBUTE_LOGGER))
            {
                DefaultLoggerName = Attributes[SUPPORTED_ATTRIBUTE_LOGGER];
            }

            var configFile = new FileInfo(name);

            if (File.Exists(configFile.FullName))
            {
                Configure(configFile);

                return;
            }

            var pathFromThisAssembly = GetPathFromAssembly(this.GetType().Assembly);
            var configFileFromThisAssembly = new FileInfo(Path.Combine(pathFromThisAssembly, configFile.Name));
            if (File.Exists(configFileFromThisAssembly.FullName))
            {
                Configure(configFile);

                return;
            }
            
            var pathFromExecutingAssembly = GetPathFromAssembly(Assembly.GetExecutingAssembly());
            var configFileFromExecutingAssembly = new FileInfo(Path.Combine(pathFromExecutingAssembly, configFile.Name));
            if (File.Exists(configFileFromExecutingAssembly.FullName))
            {
                Configure(configFile);

                return;
            }

            Configure();
        }

        private static string GetPathFromAssembly(Assembly assembly)
        {
            Contract.Requires(null != assembly);
            Contract.Ensures(null != Contract.Result<string>());
            Contract.Ensures(Directory.Exists(Contract.Result<string>()));

            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            var result = Path.GetDirectoryName(path);
            return result;
        }
            
        public static Assembly Assembly
        {
            get { return _assembly.Value; }
        }

        public static ILog GetLogger(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(null != Contract.Result<ILog>());

            var logger = _loggers.GetOrAdd(name, key =>
            {
                var logManager = Assembly.DefinedTypes.FirstOrDefault(e => e.Name == CLASS_NAME_LOG_MANAGER);
                Contract.Assert(null != logManager, CLASS_NAME_LOG_MANAGER);

                var methodInfo = logManager.GetMethod(METHOD_NAME_GET_LOGGER, BINDING_FLAGS, null, new Type[] { typeof(string) }, null);
                Contract.Assert(null != methodInfo, METHOD_NAME_GET_LOGGER);

                var loggerInstance = methodInfo.Invoke(null, new object[] { name });
                Contract.Assert(null != loggerInstance);

                return new Log4Net.Log4Net(loggerInstance);
            });

            return logger;
        }

        public static void Configure()
        {
            var xmlConfigurator = Assembly.DefinedTypes.FirstOrDefault(e => e.Name == CLASS_NAME_XML_CONFIGURATOR);
            Contract.Assert(null != xmlConfigurator, CLASS_NAME_XML_CONFIGURATOR);

            var methodInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BINDING_FLAGS, null, new Type[] { }, null);
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
                var methodInfoFileInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BINDING_FLAGS, null, new Type[] { typeof(FileInfo) }, null);
                Contract.Assert(null != methodInfoFileInfo, METHOD_NAME_XML_CONFIGURATOR);

                methodInfoFileInfo.Invoke(null, new object[] { configFile });
            }
            else
            {
                var methodInfo = xmlConfigurator.GetMethod(METHOD_NAME_XML_CONFIGURATOR, BINDING_FLAGS, null, new Type[] { }, null);
                Contract.Assert(null != methodInfo, METHOD_NAME_XML_CONFIGURATOR);

                methodInfo.Invoke(null, new object[] { });
            }
        }

        public string DefaultLoggerName { get; set; }

        public override void Write(string message)
        {
            WriteImpl(message, Logger.DEFAULT_TRACESOURCE_NAME, false);
        }

        public override void Write(object o)
        {
            Contract.Requires(null != o);

            WriteImpl(o.ToString(), Logger.DEFAULT_TRACESOURCE_NAME, false);
        }

        public override void Write(object o, string category)
        {
            Contract.Requires(null != o);
            Contract.Requires(null != category);

            WriteImpl(o.ToString(), category, false);
        }

        public override void Write(string message, string category)
        {
            Contract.Requires(null != category);

            WriteImpl(message, category, false);
        }

        public override void WriteLine(string message)
        {
            WriteImpl(message, Logger.DEFAULT_TRACESOURCE_NAME, true);
        }

        public override void WriteLine(object o)
        {
            Contract.Requires(null != o);
            WriteImpl(o.ToString(), Logger.DEFAULT_TRACESOURCE_NAME, true);
        }

        public override void WriteLine(object o, string category)
        {
            Contract.Requires(null != o);
            Contract.Requires(null != category);

            WriteImpl(o.ToString(), category, true);
        }

        public override void WriteLine(string message, string category)
        {
            Contract.Requires(null != category);

            WriteImpl(message, category, true);
        }

        private void WriteImpl(string message, string source, bool appendNewLine)
        {
            var logger = GetLoggerOrDefault(Logger.DEFAULT_TRACESOURCE_NAME);
            if (!logger.IsDebugEnabled) { return; }

            var activityId = Trace.CorrelationManager.ActivityId;

            var sb = new StringBuilder(activityId.ToString());
            if (0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(DELIMITER);
                sb.Append(DateTimeOffset.Now.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(DELIMITER);
            sb.Append(source);

            sb.Append(DELIMITER);
            sb.Append(DEFAULT_TRACE_ID);

            sb.Append(DELIMITER);
            sb.Append(message);

            if (appendNewLine)
            {
                sb.AppendLine();
            }

            logger.Debug(sb.ToString());
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, string.Empty, _emptyArgs);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, message, _emptyArgs);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (null != base.Filter && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) { return; }

            var logger = GetLoggerOrDefault(source);

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

        protected string TraceEventFormatter(TraceEventCache eventCache, string source, int id, string format, params object[] args)
        {
            var activityId = Trace.CorrelationManager.ActivityId;
            
            var sb = new StringBuilder(activityId.ToString());
            if (null != eventCache && 0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(DELIMITER);
                sb.Append(eventCache.DateTime.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(DELIMITER);
            sb.Append(source);

            sb.Append(DELIMITER);
            sb.Append(id);

            if (!string.IsNullOrEmpty(format))
            {
                sb.Append(DELIMITER);
                
                if (null != args && 0 < args.Length)
                {
                    sb.AppendFormat(format, args);
                }
                else
                {
                    sb.Append(format);
                }
            }

            return sb.ToString();
        }

        public override void Fail(string message)
        {
            var logger = GetLoggerOrDefault(Logger.DEFAULT_TRACESOURCE_NAME);
            if (!logger.IsDebugEnabled) { return; }

            var eventCache = new TraceEventCache();

            TraceEventFormatter(eventCache, Logger.DEFAULT_TRACESOURCE_NAME, DEFAULT_TRACE_ID, message, _emptyArgs);

            base.Fail(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            var logger = GetLoggerOrDefault(Logger.DEFAULT_TRACESOURCE_NAME);
            if (!logger.IsFatalEnabled) { return; }

            var formattedMessage = TraceEventFormatter(new TraceEventCache(), Logger.DEFAULT_TRACESOURCE_NAME, DEFAULT_TRACE_ID, FAIL_MESSAGE_TEMPLATE, message, detailMessage);
            logger.Fatal(formattedMessage);

            base.Fail(message, detailMessage);
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { SUPPORTED_ATTRIBUTE_LOGGER };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ILog GetLoggerOrDefault(string name)
        {
            return string.IsNullOrWhiteSpace(DefaultLoggerName) ? GetLogger(name) : GetLogger(DefaultLoggerName);
        }
    }
}
