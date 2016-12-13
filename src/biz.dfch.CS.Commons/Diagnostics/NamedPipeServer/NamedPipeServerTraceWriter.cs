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
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace biz.dfch.CS.Commons.Diagnostics.NamedPipeServer
{
    public class NamedPipeServerTraceWriter : IDisposable
    {
        private const int SERVER_CONNECTION_POOL_WAIT_TIMEOUT_MS = 5 * 1000;

        public const int CONCURRENT_CONNECTIONS_MAX_DEFAULT = 32;

        public const string NAMED_PIPE_NAME_DEFAULT = "biz.dfch.CS.Commons.Diagnostics.NamedPipeTraceListener";

        private readonly ManualResetEventSlim abortEvent = 
            new ManualResetEventSlim(false, 0);
        private readonly ManualResetEventSlim serverConnectionThreadClosedEvent = 
            new ManualResetEventSlim(false, 0);

        private static readonly object _lock = new object();
        private readonly List<NamedPipeServerStream> serverConnections; 

        public readonly ConcurrentQueue<string> Messages = new ConcurrentQueue<string>();

        public string Name { get; private set; }

        public int ConcurrentConnections { get; private set; }

        public int MaxAllowedServerInstances { get; private set; }

        public NamedPipeServerTraceWriter()
            : this
            (
                  NAMED_PIPE_NAME_DEFAULT, 
                  CONCURRENT_CONNECTIONS_MAX_DEFAULT, 
                  NamedPipeServerStream.MaxAllowedServerInstances
            )
        {
            // N/A
        }

        public NamedPipeServerTraceWriter(string name)
            : this(name, CONCURRENT_CONNECTIONS_MAX_DEFAULT, NamedPipeServerStream.MaxAllowedServerInstances)
        {
            // N/A
        }

        public NamedPipeServerTraceWriter(string name, int concurrentConnections)
            : this(name, concurrentConnections, NamedPipeServerStream.MaxAllowedServerInstances)
        {
            // N/A
        }

        public NamedPipeServerTraceWriter(string name, int concurrentConnections, int maxServerInstances)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(0 < concurrentConnections);
            Contract.Requires(NamedPipeServerStream.MaxAllowedServerInstances == maxServerInstances || 0 < maxServerInstances);

            Name = name;
            ConcurrentConnections = concurrentConnections;
            serverConnections = new List<NamedPipeServerStream>(ConcurrentConnections);
            MaxAllowedServerInstances = maxServerInstances;

            var initialisedMethodHasBeenQueued = ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectionManager), this);
            Contract.Assert(initialisedMethodHasBeenQueued);
        }

        public static void ConnectionManager(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var instance = stateInfo as NamedPipeServerTraceWriter;
            Contract.Assert(null != instance);

            for (;;)
            {
                lock (_lock)
                {
                    var count = instance.ConcurrentConnections - instance.serverConnections.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var server = new NamedPipeServerStream
                        (
                            instance.Name, 
                            PipeDirection.In,
                            instance.MaxAllowedServerInstances,
                            PipeTransmissionMode.Message
                        );

                        instance.serverConnections.Add(server);

                        var connectionInfo = new ConnectionInfo()
                        {
                            Instance = instance,
                            Server = server,
                        };

                        ThreadPool.QueueUserWorkItem(new WaitCallback(ServerProc), connectionInfo);
                    }

                    if (instance.abortEvent.IsSet)
                    {
                        foreach (var server in instance.serverConnections)
                        {
                            server.Dispose();
                        }

                        return;
                    }

                    var serverConnectionHasBeenClosed = instance
                        .serverConnectionThreadClosedEvent
                        .Wait(SERVER_CONNECTION_POOL_WAIT_TIMEOUT_MS);
                    if (serverConnectionHasBeenClosed)
                    {
                        instance.serverConnectionThreadClosedEvent.Reset();
                    }
                }
            }
        }

        public static void ServerProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var connectionInfo = stateInfo as ConnectionInfo;
            Contract.Assert(null != connectionInfo);
            Contract.Assert(null != connectionInfo.Instance);
            Contract.Assert(null != connectionInfo.Server);

            try
            {
                connectionInfo.Server.WaitForConnection();
                var pipeHandler = new MessageHandler(connectionInfo.Server);

                for(;;)
                {
                    if (!connectionInfo.Server.IsConnected)
                    {
                        return;
                    }

                    var message = pipeHandler.Read();
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        continue;
                    }

                    connectionInfo.Instance.Messages.Enqueue(message);

                    if (connectionInfo.Instance.abortEvent.IsSet)
                    {
                        return;
                    }
                }

            }
            finally
            {
                connectionInfo.Server.Dispose();

                lock (_lock)
                {
                    connectionInfo.Instance.serverConnections.Remove(connectionInfo.Server);
                }

                connectionInfo.Instance.serverConnectionThreadClosedEvent.Set();
            }
        }

        public void Dispose()  
        {  
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        protected virtual void Dispose(bool disposing)  
        {  
            if (!disposing)
            {
                return;
            }

            abortEvent.Set();
        }

        private class ConnectionInfo
        {
            public NamedPipeServerTraceWriter Instance;
            public NamedPipeServerStream Server;
        }

   }
}
