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
        private const int DEQUEUE_SLEEP_TIME_MS = 1;

        private readonly List<T> list;

        private readonly object _lock = new object();

        private volatile bool isInitialised;

        private volatile int dequeuePointer;
        private volatile int enqueuePointer;
        private volatile int availableItems;
        public int AvailableItems
        {
            get { return availableItems; }
        }

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

        public const int CAPACITY_DEFAULT = 16;

        public CircularQueue()
            : this(CAPACITY_DEFAULT)
        {
            // nothing to be done here
        }

        public CircularQueue(int capacity)
        {
            Contract.Requires(2 <= capacity);

            list = new List<T>(capacity);
            this.capacity = capacity;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                if (!isInitialised)
                {
                    // in this branch we fill the list for the first time
                    // really adding items to the list, i.e. the list Count 
                    // increments on every Add operation
                    list.Add(item);

                    // once we filled the whole list with items
                    // we consider the list to be initialised
                    // i.e. we do not have to add any more items
                    // but just have to overwrite an existing item
                    if (0 != (++enqueuePointer & capacity))
                    {
                        enqueuePointer = 0;
                        isInitialised = true;
                    }

                    // naturally the number of available items increments
                    // as well
                    availableItems++;

                    // hint:
                    // there is no need to increment the dequeue pointer
                    // while initialising, as it starts at 0 and can dequeue
                    // up to the position of the enqueue pointer

                    return;
                }

                // here we have an already initialised list
                // so we just replace the items
                list[enqueuePointer] = item;

                // we increment the enqueue pointer
                // as always checking if we have to start at the 
                // bottom of the list
                if (0 != (++enqueuePointer & capacity))
                {
                    enqueuePointer = 0;
                }

                // if the number of available items already equals the capacity
                // we have effectively lost an item
                // in addition, we have to set the dequeue pointer to the 
                // enqueue pointer as we now have a full list of buffered items
                if (capacity == availableItems)
                {
                    dequeuePointer = enqueuePointer;

                    discardedItems++;

                    return;
                }

                // after adding we have an additional item available
                // after icrementing this is at maximum euqal to the capacity
                availableItems++;

                // if enqueue pointer "overrounds" dequeue pointer
                // we have to increment the dequeue pointer so it reads the 
                // oldest item
                if (enqueuePointer == dequeuePointer)
                {
                    if (0 != (++dequeuePointer & capacity))
                    {
                        dequeuePointer = 0;
                    }
                }

            }
        }

        public bool TryDequeue(out T item, int waitTimeoutMs)
        {
            Contract.Requires(0 < waitTimeoutMs);

            var sw = Stopwatch.StartNew();
            for (;;)
            {
                var result = TryDequeue(out item);
                if (result)
                {
                    sw.Stop();
                    return true;
                }

                if (waitTimeoutMs >= sw.ElapsedMilliseconds + DEQUEUE_SLEEP_TIME_MS)
                {
                    Thread.Sleep(DEQUEUE_SLEEP_TIME_MS);
                    continue;
                }

                sw.Stop();
                break;
            }

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

                if (0 != (++dequeuePointer & capacity))
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
