/**
 * Copyright 2017 d-fens GmbH
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
using System.IO;
using BenchmarkDotNet.Reports;

namespace biz.dfch.CS.Commons.Benchmarks
{
    public static class SummaryExtensions
    {
        private const string REPORT_GITHUB_MD = @"{0}-report-github.md";

        public static string GetMarkdownReport(this Summary summary)
        {
            Contract.Requires(null != summary);
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var fileName = Path.Combine
            (
                summary.ResultsDirectoryPath,
                string.Format(REPORT_GITHUB_MD, summary.Title)
            );

            var report = File.ReadAllText(fileName);
            return report;
        }
    }
}
