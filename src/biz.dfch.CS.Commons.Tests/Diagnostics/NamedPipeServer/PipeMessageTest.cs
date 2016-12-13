/**
 * Copyright 2011-2016 d-fens GmbH
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
 *
 */

using System.Diagnostics;
using biz.dfch.CS.Commons.Diagnostics.NamedPipeServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Diagnostics.NamedPipeServer
{
    [TestClass]
    public class PipeMessageTest
    {
        [TestMethod]
        public void DecomposingMessageSucceeds()
        {
            var delimiter = '|';
            var message = "This is an arbitrary message";
            var source = "Arbitrary.Source.Name";
            var traceEventType = TraceEventType.Warning;

            var template = "{1}{0}{2}{0}{3}";
            var value = string.Format(template, delimiter, traceEventType, source, message);

            var sut = new PipeMessage(value);

            Assert.AreEqual(message, sut.Message);
            Assert.AreEqual(source, sut.Source);
            Assert.AreEqual(traceEventType, sut.TraceEventType);

            Assert.IsTrue(sut.IsValid());
        }
    }
}
