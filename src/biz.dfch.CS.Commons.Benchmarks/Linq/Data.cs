using System;
using System.Collections.Generic;
using System.Linq;

namespace biz.dfch.CS.Commons.Benchmarks.Linq
{
    public class Data
    {
        private const int START = 0;
        private const int COUNT = 1000;

        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly Random _random = new Random();

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(CHARS, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray());
        }

        static Data()
        {
            for (var c = START; c < START + COUNT; c++)
            {
                _list.Add(RandomString(257));
            }

            _enumerableList = _list;
        }

        // ReSharper disable once InconsistentNaming
        internal static List<string> _list = new List<string>();
        // ReSharper disable once InconsistentNaming
        internal static IEnumerable<string> _enumerableList;

        public IEnumerable<string> EnumerableList => _enumerableList;
    }
}
