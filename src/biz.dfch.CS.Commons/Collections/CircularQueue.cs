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
using System.Diagnostics.Contracts;
using System.Threading;

namespace biz.dfch.CS.Commons.Collections
{
    public class CircularQueue<T>
        where T : class
    {
        public const int CAPACITY_DEFAULT = 16;

        private volatile bool isInitialised;

        private volatile int enqueuePointer;
        private volatile int dequeuePointer;

        private readonly int capacity;
        public int Capacity
        {
            get { return capacity; }
        }

        private readonly List<T> list;

        private readonly object _lock = new object();

        private readonly ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

        public CircularQueue()
            : this(CAPACITY_DEFAULT)
        {
            // nothing to be done here
        }

        public CircularQueue(int capacity)
        {
            Contract.Requires(0 < capacity);

            list = new List<T>(capacity);
            this.capacity = capacity;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                if (!isInitialised)
                {
                    list.Add(item);

                    // once we filled the whole list with items
                    // we consider the list to be initialised
                    // i.e. we do not have to add any more items
                    // but just have to overwrite an existing item
                    isInitialised = 0 == (enqueuePointer = ++enqueuePointer % capacity);
                }
                else
                {
                    list[enqueuePointer] = item;

                    enqueuePointer = ++enqueuePointer % capacity;
                }

                // if enqueue operations pointer "overrounds" dequeue pointer
                // we have to increment the dequeue pointer so it reads the 
                // oldest item
                if (enqueuePointer == dequeuePointer)
                {
                    dequeuePointer = ++dequeuePointer % capacity;
                }

                manualResetEventSlim.Set();
            }
        }

        public bool TryDequeue(out T item, int waitTimeoutMs)
        {
            var result = TryDequeue(out item);
            if (result)
            {
                manualResetEventSlim.Reset();

                return true;
            }

            result = manualResetEventSlim.Wait(waitTimeoutMs);
            if (!result)
            {
                return false;
            }

            result = TryDequeue(out item);

            manualResetEventSlim.Reset();

            return result;
        }

        public bool TryDequeue(out T item)
        {
            lock (_lock)
            {
                if (enqueuePointer == dequeuePointer)
                {
                    item = default(T);
                    return false;
                }

                item = list[dequeuePointer];

                dequeuePointer = ++dequeuePointer % capacity;

                return true;
            }
        }

        public bool TryPeek(out T item)
        {
            lock (_lock)
            {
                if (enqueuePointer == dequeuePointer)
                {
                    item = default(T);
                    return false;
                }

                item = list[dequeuePointer];

                return true;
            }
        }
    }
}
