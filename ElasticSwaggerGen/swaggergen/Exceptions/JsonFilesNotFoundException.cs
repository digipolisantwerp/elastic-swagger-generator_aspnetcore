using System;

namespace ElasticSwaggerGen
{
    public class JsonFilesNotFoundException : Exception
    {
        public JsonFilesNotFoundException(string path) : base($"No JSON files  in {path}.")
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
