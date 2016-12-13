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
using System.IO.Pipes;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Diagnostics
{
    [TestClass]
    public partial class PipeTest
    {
        private const int DURATION_MS = 60 * 1000;
        private const string SERVER_NAME = ".";
        private const string PIPE_HANDLE_AS_STRING = "NAMED_PIPE";

        private static readonly object _lock = new object();
        private const int PIPE_CONNECTIONS_MAX = 1000;
        private static readonly List<NamedPipeServerStream> _serverConnections = 
            new List<NamedPipeServerStream>(PIPE_CONNECTIONS_MAX);

        public const int MESSAGE_SIZE_MAX = 1024 * 1024;

        private static bool _source0 = false;
        private static bool _source1 = false;
        private static bool _source2 = false;
        private static bool _source3 = false;
        private static bool _sourceOther = false;

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void TestNamedPipeWithMultipleClientsAndServers()
        {
            for (var c = 0; c < 4; c++)
            {
                var clientInfo = new ClientInfo
                {
                    DurationMs = DURATION_MS,
                    PipeName =  PIPE_HANDLE_AS_STRING,
                    Server = SERVER_NAME,
                    SourceName = string.Format("Source-{0}", c),
                };

                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientProc), clientInfo);
            }

            var sw = Stopwatch.StartNew();
            do
            {
                lock (_lock)
                {
                    var count = PIPE_CONNECTIONS_MAX - _serverConnections.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var server = new NamedPipeServerStream
                        (
                            PIPE_HANDLE_AS_STRING, 
                            PipeDirection.In,
                            NamedPipeServerStream.MaxAllowedServerInstances,
                            PipeTransmissionMode.Message
                        );

                        var serverInfo = new ServerInfo()
                        {
                            DurationMs = DURATION_MS * 2,
                            Instance = server,
                        };

                        _serverConnections.Add(server);

                        ThreadPool.QueueUserWorkItem(new WaitCallback(ServerProc), serverInfo);
                    }

                    Thread.Sleep(15 * 1000);
                }
            }
            while (2 * DURATION_MS + 5000 > sw.ElapsedMilliseconds);


            System.Diagnostics.Trace.WriteLine(string.Format("send: '{0}'", PipeHandler.SendCount.ToString()));
            System.Diagnostics.Trace.WriteLine(string.Format("read: '{0}'", PipeHandler.ReadCount.ToString()));

            System.Diagnostics.Trace.WriteLine(string.Format("_source0: '{0}'", _source0));
            System.Diagnostics.Trace.WriteLine(string.Format("_source1: '{0}'", _source1));
            System.Diagnostics.Trace.WriteLine(string.Format("_source2: '{0}'", _source2));
            System.Diagnostics.Trace.WriteLine(string.Format("_source3: '{0}'", _source3));
            System.Diagnostics.Trace.WriteLine(string.Format("_sourceOther: '{0}'", _sourceOther));

            Assert.IsTrue(_source0);
            Assert.IsTrue(_source1);
            Assert.IsTrue(_source2);
            Assert.IsTrue(_source3);
            Assert.IsFalse(_sourceOther);
        }

        public static void ClientProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var taskInfo = stateInfo as ClientInfo;
            Contract.Assert(null != taskInfo);

            var sw = Stopwatch.StartNew();
            using (var client = new NamedPipeClientStream(taskInfo.Server, taskInfo.PipeName, PipeDirection.Out))
            {
                client.Connect(taskInfo.DurationMs);
                client.ReadMode = PipeTransmissionMode.Message;
                var pipeHandler = new PipeHandler(client);

                var randomString = MessageFactory.Get();
                do
                {
                    var message = string.Format
                    (
                        "Source: '{0}', DateTimeOffset '{1}', Message '{2}'", 
                        taskInfo.SourceName, DateTimeOffset.Now.ToString("O"), randomString
                    );
                    pipeHandler.Send(message);
                }
                while (taskInfo.DurationMs > sw.ElapsedMilliseconds);
            }

            return;
        }

        public static void ServerProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var serverInfo = stateInfo as ServerInfo;
            Contract.Assert(null != serverInfo);

            var server = serverInfo.Instance;
            Contract.Assert(null != server);

            try
            {
                server.WaitForConnection();
                var pipeHandler = new PipeHandler(server);

                var sw = Stopwatch.StartNew();
                do
                {
                    if (!server.IsConnected)
                    {
                        return;
                    }

                    var message = pipeHandler.Read();
                    if (message.StartsWith("Source: 'Source-0"))
                    {
                        _source0 = true;
                    }
                    else if (message.StartsWith("Source: 'Source-1"))
                    {
                        _source1 = true;
                    }
                    else if (message.StartsWith("Source: 'Source-2"))
                    {
                        _source2 = true;
                    }
                    else if (message.StartsWith("Source: 'Source-3"))
                    {
                        _source3 = true;
                    }
                    else
                    {
                        _sourceOther = true;
                    }

                }
                while (serverInfo.DurationMs > sw.ElapsedMilliseconds);

            }
            finally
            {
                server.Dispose();

                lock (_lock)
                {
                    _serverConnections.Remove(server);
                }
            }

        }
    }
}
