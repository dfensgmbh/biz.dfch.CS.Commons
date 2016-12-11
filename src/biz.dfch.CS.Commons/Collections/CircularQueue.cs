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
using System.Threading;

namespace biz.dfch.CS.Commons.Collections
{
    public class CircularQueue<T>
        where T : class
    {
        public const int CAPACITY_DEFAULT = 16;

        private volatile bool isInitialised;

        private volatile int dequeuePointer;
        private volatile int enqueuePointer;
        private volatile int availableItems;

        private readonly int capacity;
        public int Capacity
        {
            get { return capacity; }
        }

        private ulong discardedItems;
        public ulong DiscardedItems
        {
            get { return discardedItems; }
        }

        private readonly List<T> list;

        private readonly object _lock = new object();

        public CircularQueue()
            : this(CAPACITY_DEFAULT)
        {
            // nothing to be done here
        }

        public CircularQueue(int capacity)
        {
            Contract.Requires(2 <= capacity);
            Contract.Requires(capacity >> 1 == capacity / (decimal)2);

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
                    if (0 == (++enqueuePointer ^ capacity))
                    {
                        enqueuePointer = 0;
                        isInitialised = true;
                    }

                    availableItems++;

                    return;
                }

                // already initialised
                list[enqueuePointer] = item;

                if (0 == (++enqueuePointer ^ capacity))
                {
                    enqueuePointer = 0;
                }

                if (capacity == availableItems)
                {
                    dequeuePointer = enqueuePointer;

                    discardedItems++;

                    return;
                }

                availableItems++;

                // if enqueue operations pointer "overrounds" dequeue pointer
                // we have to increment the dequeue pointer so it reads the 
                // oldest item
                if (enqueuePointer == dequeuePointer)
                {
                    if (0 == (++dequeuePointer ^ capacity))
                    {
                        dequeuePointer = 0;
                    }
                }

            }
        }

        public bool TryDequeue(out T item, int waitTimeoutMs)
        {
            Contract.Requires(Timeout.Infinite == waitTimeoutMs || 0 < waitTimeoutMs);

            var sw = Stopwatch.StartNew();
            do
            {
                var result = TryDequeue(out item);
                if (result)
                {
                    return true;
                }

                Thread.Sleep(1);
            }
            while (waitTimeoutMs > sw.ElapsedMilliseconds);
            sw.Stop();

            return false;
        }

        public bool TryDequeue(out T item)
        {
            lock (_lock)
            {
                if (0 == availableItems)
                {
                    item = default(T);
                    return false;
                }

                item = list[dequeuePointer];

                if (0 == (++dequeuePointer ^ capacity))
                {
                    dequeuePointer = 0;
                }

                availableItems--;
                return true;
            }
        }

        public bool TryPeek(out T item)
        {
            lock (_lock)
            {
                //if (enqueuePointer == dequeuePointer || UNINITIALISED_DEQUEUE_POINTER == dequeuePointer)
                if (0 == availableItems)
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
