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
using System.IO;
using biz.dfch.CS.Commons.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace biz.dfch.CS.Commons.Tests.Diagnostics
{
    [TestClass]
    public class Log4NetTraceListenerTest
    {
        [TestMethod]
        public void GetAssemblySucceeds()
        {
            var result = Log4NetTraceListener.Assembly;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetLoggerWithEmptyNameThrowsContractException()
        {
            var name = "   ";
            var result = Log4NetTraceListener.GetLogger(name);
        }

        [TestMethod]
        public void GetLoggerSucceeds()
        {
            var name = Guid.NewGuid().ToString();
            var result = Log4NetTraceListener.GetLogger(name);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DebugFormatSucceeds()
        {
            var name = Guid.NewGuid().ToString();
            var result = Log4NetTraceListener.GetLogger(name);
            Assert.IsNotNull(result);

            result.DebugFormat("format '{0}', '{1}', '{2}', '{3}'", "arg0", "arg1", "arg2", "arg3");
        }

        [TestMethod]
        public void ConfigureWithExistingConfigurationFileSucceeds()
        {
            var traceAssert = Mock.Create<DebugAndTraceListenerAssert>();
            Mock.Arrange(() => traceAssert.WriteLine())
                .IgnoreInstance()
                .MustBeCalled();

            var configFile = new FileInfo(@".\log4net.config");
            Log4NetTraceListener.Configure(configFile);

            var name = Guid.NewGuid().ToString();
            var result = Log4NetTraceListener.GetLogger(name);
            Assert.IsNotNull(result);

            Assert.IsTrue(result.IsDebugEnabled);
            Assert.IsTrue(result.IsErrorEnabled);
            Assert.IsTrue(result.IsFatalEnabled);
            Assert.IsTrue(result.IsInfoEnabled);
            Assert.IsTrue(result.IsWarnEnabled);

            result.DebugFormat("format '{0}', '{1}', '{2}', '{3}'", "arg0", "arg1", "arg2", "arg3");

            Mock.Assert(traceAssert);
        }

        [TestMethod]
        public void ConfigureWithInexistingConfigurationFileSucceeds()
        {
            var traceAssert = Mock.Create<DebugAndTraceListenerAssert>();
            Mock.Arrange(() => traceAssert.WriteLine())
                .IgnoreInstance()
                .MustBeCalled();

            var configFile = new FileInfo(@".\invalid.config");
            Log4NetTraceListener.Configure(configFile);

            var name = Guid.NewGuid().ToString();
            var result = Log4NetTraceListener.GetLogger(name);
            Assert.IsNotNull(result);

            Assert.IsTrue(result.IsDebugEnabled);
            Assert.IsTrue(result.IsErrorEnabled);
            Assert.IsTrue(result.IsFatalEnabled);
            Assert.IsTrue(result.IsInfoEnabled);
            Assert.IsTrue(result.IsWarnEnabled);

            Mock.Assert(traceAssert);
        }
        
        [TestMethod]
        public void ConfigureWithoutParametersTriesToLoadFromConfigurationSection()
        {
            var traceAssert = Mock.Create<DebugAndTraceListenerAssert>();
            Mock.Arrange(() => traceAssert.WriteLine())
                .IgnoreInstance()
                .MustBeCalled();

            Log4NetTraceListener.Configure();

            var name = Guid.NewGuid().ToString();
            var result = Log4NetTraceListener.GetLogger(name);
            Assert.IsNotNull(result);

            Assert.IsTrue(result.IsDebugEnabled);
            Assert.IsTrue(result.IsErrorEnabled);
            Assert.IsTrue(result.IsFatalEnabled);
            Assert.IsTrue(result.IsInfoEnabled);
            Assert.IsTrue(result.IsWarnEnabled);

            Mock.Assert(traceAssert);
        }
    }
}
