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

using System.Diagnostics.Contracts;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class MessageHandler
    {
        private static readonly UTF8Encoding _streamEncoding = new UTF8Encoding();

        private readonly PipeStream pipeStream;

        public const int MESSAGE_SIZE_MAX = 256 * 1024;

        public MessageHandler(PipeStream pipeStream)
        {
            Contract.Requires(null != pipeStream);

            this.pipeStream = pipeStream;
        }

        public string Read()
        {
            var bytes = new byte[MESSAGE_SIZE_MAX];

            var bytesRead = pipeStream.Read(bytes, 0, bytes.Length);
            Contract.Assert(pipeStream.IsMessageComplete);
            Contract.Assert(0 != bytesRead);

            return _streamEncoding.GetString(bytes, 0, bytesRead);
        }
        public void Write(string message)
        {
            var bytes = _streamEncoding.GetBytes(message);
            Contract.Assert(MESSAGE_SIZE_MAX >= bytes.Length);

            pipeStream.Write(bytes, 0, bytes.Length);
            pipeStream.Flush();
        }
    }
}
