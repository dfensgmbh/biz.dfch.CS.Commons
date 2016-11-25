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

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using OriginalTraceSource = System.Diagnostics.TraceSource;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public static class Logger
    {
        private const string TRACESOURCE_TYPE_ERROR = "The specified name is not an instance of type 'biz.dfch.CS.Commons.Diagnostics.TraceSource'.";

        public const string DEFAULT_TRACESOURCE_NAME = "biz.dfch.Commons.Diagnostics.TraceSource.Default";

        private static readonly object _lock = new object();

        private static readonly ConcurrentDictionary<string, OriginalTraceSource> _traceSources =
            new ConcurrentDictionary<string, OriginalTraceSource>();

        private static TraceListener _traceListener = new DefaultTraceListener();

        public static TraceSource Default
        {
            get
            {
                Contract.Ensures(null != Contract.Result<TraceSource>());

                var traceSource = _traceSources.GetOrAdd(DEFAULT_TRACESOURCE_NAME, key =>
                {
                    var result = (OriginalTraceSource) new TraceSource(key, SourceLevels.All);

                    ApplyListeners(result);

                    return result;
                });

                return traceSource as TraceSource;
            }
        }

        public static TraceSource Get(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(null != Contract.Result<TraceSource>(), TRACESOURCE_TYPE_ERROR);

            var traceSource = _traceSources.GetOrAdd(name, key =>
            {
                var result = (OriginalTraceSource) new TraceSource(key, SourceLevels.All);

                ApplyListeners(result);

                return result;
            });

            return traceSource as TraceSource;
        }

        public static OriginalTraceSource GetBase(string traceSourceBase)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(traceSourceBase));
            Contract.Ensures(null != Contract.Result<OriginalTraceSource>());

            var traceSource = _traceSources.GetOrAdd(traceSourceBase, key =>
            {
                var result = new OriginalTraceSource(key, SourceLevels.All);

                ApplyListeners(result);

                return result;
            });

            return traceSource;
        }

        public static TraceSource GetOrDefault(string name, TraceSource defaultTraceSource)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(null != defaultTraceSource);
            Contract.Ensures(null != Contract.Result<TraceSource>());

            var result = _traceSources.AddOrUpdate(name, defaultTraceSource, (key, value) => value);
            return result as TraceSource;
        }

        public static OriginalTraceSource GetOrDefault(string name, OriginalTraceSource traceSource)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(null != traceSource);
            Contract.Ensures(null != Contract.Result<OriginalTraceSource>());

            var result = _traceSources.AddOrUpdate(name, traceSource, (key, oldvalue) => oldvalue);
            return result;
        }

        public static TraceListener SetTraceListener(TraceListener traceListener)
        {
            return SetTraceListener(traceListener, updateTraceSources: false);
        }

        public static TraceListener SetTraceListener(TraceListener traceListener, bool updateTraceSources)
        {
            lock (_lock)
            {
                var previousTraceListener = _traceListener;
                _traceListener = traceListener;

                if (updateTraceSources)
                {
                    foreach (var traceSource in _traceSources)
                    {
                        if (null != _traceListener)
                        {
                            traceSource.Value.Listeners.Add(_traceListener);
                        }
                        else
                        {
                            traceSource.Value.Listeners.Clear();
                        }
                    }
                }
                
                return previousTraceListener;
            }
        }

        private static void ApplyListeners(OriginalTraceSource traceSource)
        {
            lock (_lock)
            {
                var defaultListenerMustBeAdded = false;

                for (var c = traceSource.Listeners.Count -1; c >= 0; c--)
                {
                    var traceSourceListener = traceSource.Listeners[c];

                    var defaultTraceListener = traceSourceListener as DefaultTraceListener;
                    if (null == defaultTraceListener || typeof(DefaultTraceListener) != defaultTraceListener.GetType())
                    {
                        continue;
                    }

                    traceSource.Listeners.RemoveAt(c);

                    defaultListenerMustBeAdded = true;

                }
                if (defaultListenerMustBeAdded)
                {
                    if (null != _traceListener)
                    {
                        traceSource.Listeners.Add(_traceListener);
                    }
                }
            }
        }
    }
}
