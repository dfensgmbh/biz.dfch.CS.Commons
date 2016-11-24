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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Commons.Diagnostics.Log4Net
{
    public class Log4Net : ILog
    {
        private readonly object logger;

        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;

        private const string METHOD_NAME_DEBUG = "Debug";
        private const string METHOD_NAME_DEBUG_FORMAT = "DebugFormat";
        private const string METHOD_NAME_ERROR = "Error";
        private const string METHOD_NAME_ERROR_FORMAT = "ErrorFormat";
        private const string METHOD_NAME_FATAL = "Fatal";
        private const string METHOD_NAME_FATAL_FORMAT = "FatalFormat";
        private const string METHOD_NAME_INFO = "Info";
        private const string METHOD_NAME_INFO_FORMAT = "InfoFormat";

        public bool IsDebugEnabled { get; private set; }
        public bool IsErrorEnabled { get; private set; }
        public bool IsFatalEnabled { get; private set; }
        public bool IsInfoEnabled { get; private set; }
        public bool IsWarnEnabled { get; private set; }

        public Log4Net(object logger)
        {
            Contract.Requires(null != logger);

            this.logger = logger;

            debugObject = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG, BINDING_FLAGS, null, new Type[] { typeof(object) }, null);
            Contract.Assert(null != debugObject);

            debugObjectException = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG, BINDING_FLAGS, null, new Type[] { typeof(object), typeof(Exception) }, null);
            Contract.Assert(null != debugObjectException);

            debugFormatStringObject = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object) }, null);
            Contract.Assert(null != debugFormatStringObject);

            debugFormatStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != debugFormatStringObjectArray);

            debugFormatIFormatProviderStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(IFormatProvider), typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != debugFormatIFormatProviderStringObjectArray);

            debugFormatStringObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object), typeof(object) }, null);
            Contract.Assert(null != debugFormatStringObjectObject);

            debugFormatStringObjectObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_DEBUG_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]), typeof(object[]), typeof(object[]) }, null);
            Contract.Assert(null != debugFormatStringObjectObjectObject);

            errorObject = this.logger.GetType().GetMethod(METHOD_NAME_ERROR, BINDING_FLAGS, null, new Type[] { typeof(object) }, null);
            Contract.Assert(null != errorObject);

            errorObjectException = this.logger.GetType().GetMethod(METHOD_NAME_ERROR, BINDING_FLAGS, null, new Type[] { typeof(object), typeof(Exception) }, null);
            Contract.Assert(null != errorObjectException);

            errorFormatStringObject = this.logger.GetType().GetMethod(METHOD_NAME_ERROR_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object) }, null);
            Contract.Assert(null != errorFormatStringObject);

            errorFormatStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_ERROR_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != errorFormatStringObjectArray);

            errorFormatIFormatProviderStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_ERROR_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(IFormatProvider), typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != errorFormatIFormatProviderStringObjectArray);

            errorFormatStringObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_ERROR_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object), typeof(object) }, null);
            Contract.Assert(null != errorFormatStringObjectObject);

            errorFormatStringObjectObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_ERROR_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]), typeof(object[]), typeof(object[]) }, null);
            Contract.Assert(null != errorFormatStringObjectObjectObject);

            fatalObject = this.logger.GetType().GetMethod(METHOD_NAME_FATAL, BINDING_FLAGS, null, new Type[] { typeof(object) }, null);
            Contract.Assert(null != fatalObject);

            fatalObjectException = this.logger.GetType().GetMethod(METHOD_NAME_FATAL, BINDING_FLAGS, null, new Type[] { typeof(object), typeof(Exception) }, null);
            Contract.Assert(null != fatalObjectException);

            fatalFormatStringObject = this.logger.GetType().GetMethod(METHOD_NAME_FATAL_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object) }, null);
            Contract.Assert(null != fatalFormatStringObject);

            fatalFormatStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_FATAL_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != fatalFormatStringObjectArray);

            fatalFormatIFormatProviderStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_FATAL_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(IFormatProvider), typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != fatalFormatIFormatProviderStringObjectArray);

            fatalFormatStringObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_FATAL_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object), typeof(object) }, null);
            Contract.Assert(null != fatalFormatStringObjectObject);

            fatalFormatStringObjectObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_FATAL_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]), typeof(object[]), typeof(object[]) }, null);
            Contract.Assert(null != fatalFormatStringObjectObjectObject);

            infoObject = this.logger.GetType().GetMethod(METHOD_NAME_INFO, BINDING_FLAGS, null, new Type[] { typeof(object) }, null);
            Contract.Assert(null != infoObject);

            infoObjectException = this.logger.GetType().GetMethod(METHOD_NAME_INFO, BINDING_FLAGS, null, new Type[] { typeof(object), typeof(Exception) }, null);
            Contract.Assert(null != infoObjectException);

            infoFormatStringObject = this.logger.GetType().GetMethod(METHOD_NAME_INFO_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object) }, null);
            Contract.Assert(null != infoFormatStringObject);

            infoFormatStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_INFO_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != infoFormatStringObjectArray);

            infoFormatIFormatProviderStringObjectArray = this.logger.GetType().GetMethod(METHOD_NAME_INFO_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(IFormatProvider), typeof(string), typeof(object[]) }, null);
            Contract.Assert(null != infoFormatIFormatProviderStringObjectArray);

            infoFormatStringObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_INFO_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object), typeof(object) }, null);
            Contract.Assert(null != infoFormatStringObjectObject);

            infoFormatStringObjectObjectObject = this.logger.GetType().GetMethod(METHOD_NAME_INFO_FORMAT, BINDING_FLAGS, null, new Type[] { typeof(string), typeof(object[]), typeof(object[]), typeof(object[]) }, null);
            Contract.Assert(null != infoFormatStringObjectObjectObject);
        }
            
        private readonly MethodInfo debugObject;
        public void Debug(object message)
        {
            debugObject.Invoke(logger, new object[] { message });
        }

        private readonly MethodInfo debugObjectException;
        public void Debug(object message, Exception exception)
        {
            debugObjectException.Invoke(logger, new object[] { message, exception });
        }

        private readonly MethodInfo debugFormatStringObject;
        public void DebugFormat(string format, object arg0)
        {
            debugFormatStringObject.Invoke(logger, new object[] { format, arg0 });
        }

        private readonly MethodInfo debugFormatStringObjectArray;
        public void DebugFormat(string format, params object[] args)
        {
            debugFormatStringObjectArray.Invoke(logger, new object[] { format, args });
        }

        private readonly MethodInfo debugFormatIFormatProviderStringObjectArray;
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            debugFormatIFormatProviderStringObjectArray.Invoke(logger, new object[] { provider, format, args });
        }

        private readonly MethodInfo debugFormatStringObjectObject;
        public void DebugFormat(string format, object arg0, object arg1)
        {
            debugFormatStringObjectObject.Invoke(logger, new object[] { format, arg0, arg1 });
        }

        private readonly MethodInfo debugFormatStringObjectObjectObject;
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            debugFormatStringObjectObjectObject.Invoke(logger, new object[] { format, arg0, arg1, arg2 });
        }

        private readonly MethodInfo errorObject;
        public void Error(object message)
        {
            errorObject.Invoke(logger, new object[] { message });
        }

        private readonly MethodInfo errorObjectException;
        public void Error(object message, Exception exception)
        {
            errorObjectException.Invoke(logger, new object[] { message, exception });
        }

        private readonly MethodInfo errorFormatStringObject;
        public void ErrorFormat(string format, object arg0)
        {
            errorFormatStringObject.Invoke(logger, new object[] { format, arg0 });
        }

        private readonly MethodInfo errorFormatStringObjectArray;
        public void ErrorFormat(string format, params object[] args)
        {
            errorFormatStringObjectArray.Invoke(logger, new object[] { format, args });
        }

        private readonly MethodInfo errorFormatIFormatProviderStringObjectArray;
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            errorFormatIFormatProviderStringObjectArray.Invoke(logger, new object[] { provider, format, args });
        }

        private readonly MethodInfo errorFormatStringObjectObject;
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            errorFormatStringObjectObject.Invoke(logger, new object[] { format, arg0, arg1 });
        }

        private readonly MethodInfo errorFormatStringObjectObjectObject;
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            errorFormatStringObjectObjectObject.Invoke(logger, new object[] { format, arg0, arg1, arg2 });
        }

        private readonly MethodInfo fatalObject;
        public void Fatal(object message)
        {
            fatalObject.Invoke(logger, new object[] { message });
        }

        private readonly MethodInfo fatalObjectException;
        public void Fatal(object message, Exception exception)
        {
            fatalObjectException.Invoke(logger, new object[] { message, exception });
        }

        private readonly MethodInfo fatalFormatStringObject;
        public void FatalFormat(string format, object arg0)
        {
            fatalFormatStringObject.Invoke(logger, new object[] { format, arg0 });
        }

        private readonly MethodInfo fatalFormatStringObjectArray;
        public void FatalFormat(string format, params object[] args)
        {
            fatalFormatStringObjectArray.Invoke(logger, new object[] { format, args });
        }

        private readonly MethodInfo fatalFormatIFormatProviderStringObjectArray;
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            fatalFormatIFormatProviderStringObjectArray.Invoke(logger, new object[] { provider, format, args });
        }

        private readonly MethodInfo fatalFormatStringObjectObject;
        public void FatalFormat(string format, object arg0, object arg1)
        {
            fatalFormatStringObjectObject.Invoke(logger, new object[] { format, arg0, arg1 });
        }

        private readonly MethodInfo fatalFormatStringObjectObjectObject;
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            fatalFormatStringObjectObject.Invoke(logger, new object[] { format, arg0, arg1, arg2 });
        }

        private readonly MethodInfo infoObject;
        public void Info(object message)
        {
            infoObject.Invoke(logger, new object[] { message });
        }

        private readonly MethodInfo infoObjectException;
        public void Info(object message, Exception exception)
        {
            infoObjectException.Invoke(logger, new object[] { message, exception });
        }

        private readonly MethodInfo infoFormatStringObject;
        public void InfoFormat(string format, object arg0)
        {
            infoFormatStringObject.Invoke(logger, new object[] { format, arg0 });
        }

        private readonly MethodInfo infoFormatStringObjectArray;
        public void InfoFormat(string format, params object[] args)
        {
            infoFormatStringObjectArray.Invoke(logger, new object[] { format, args });
        }

        private readonly MethodInfo infoFormatIFormatProviderStringObjectArray;
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            infoFormatIFormatProviderStringObjectArray.Invoke(logger, new object[] { provider, format, args });
        }

        private readonly MethodInfo infoFormatStringObjectObject;
        public void InfoFormat(string format, object arg0, object arg1)
        {
            infoFormatStringObjectObject.Invoke(logger, new object[] { format, arg0, arg1 });
        }

        private readonly MethodInfo infoFormatStringObjectObjectObject;
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            infoFormatStringObjectObjectObject.Invoke(logger, new object[] { format, arg0, arg1, arg2 });
        }

        public void Warn(object message)
        {
            throw new NotImplementedException();
        }

        public void Warn(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }
    }
}
