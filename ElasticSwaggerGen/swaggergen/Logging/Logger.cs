using System;
using System.Collections.Generic;
using System.IO;

namespace ElasticSwaggerGen.Logging
{
    class Logger
    {
        private static readonly List<string> _elements = new List<string>();

        public static void AddElement(string element)
        {
            _elements.Add(element);
        }

        public static void Write()
        {
            File.AppendAllLines(@"D:\temp\elasticswaggergen.log", new string[] { String.Join(" - ", _elements) });
            _elements.Clear();
        }
    }
}
