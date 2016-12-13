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
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Commons.Diagnostics.NamedPipeServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Diagnostics.NamedPipeServer
{
    [TestClass]
    public class NamedPipeServerTraceWriterTest
    {
        [TestMethod]
        public void Ctor1Succeeds()
        {
            var sut = new NamedPipeServerTraceWriter();

            Assert.AreEqual(NamedPipeServerTraceWriter.NAMED_PIPE_NAME_DEFAULT, sut.Name);
            Assert.AreEqual(NamedPipeServerTraceWriter.CONCURRENT_CONNECTIONS_MAX_DEFAULT, sut.ConcurrentConnections);
        }

        [TestMethod]
        public void Ctor2Succeeds()
        {
            var name = "arbitrary-name";
            var connections = 4;

            var sut = new NamedPipeServerTraceWriter(name, connections);

            Assert.AreEqual(name, sut.Name);
            Assert.AreEqual(connections, sut.ConcurrentConnections);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void InvokeTraceListenerAndDequeueFromNamedPipeServerSucceeds()
        {
            var sut = new NamedPipeServerTraceWriter();

            var listener = new NamedPipeTraceListener();

            do
            {
                Thread.Sleep(1000);
            }
            while (!listener.IsInitialised);

            int c;
            var count = 1000;
            for (c = 0; c < count; c++)
            {
               listener.WriteLine("message-" + c);
            }

            Thread.Sleep(10 * 10000);

            string item;
            bool result;
            for (c = 0; c < count; c++)
            {
                result = sut.Messages.TryDequeue(out item);
                Assert.IsTrue(result, c.ToString());
            }

            result = sut.Messages.TryDequeue(out item);
            Assert.IsFalse(result, c.ToString());
            
            listener.Dispose();
            sut.Dispose();
        }
    }
}
