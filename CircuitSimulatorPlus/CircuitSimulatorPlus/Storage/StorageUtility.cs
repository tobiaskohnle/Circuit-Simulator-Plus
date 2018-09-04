using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircuitSimulatorPlus
{
    static class StorageUtil
    {
        public static StorageObject Load(string filepath)
        {
            StorageObject store = null;
            using (var reader = new StreamReader(filepath))
                store = Deserialize(reader);
            return store;
        }

        public static void Save(string filepath, StorageObject store)
        {
            using (var writer = new StreamWriter(filepath))
                Serialize(store, writer);
        }

        public static string CreateText(StorageObject store)
        {
            string text = "";
            using (var writer = new StringWriter())
            {
                Serialize(store, writer);
                text = writer.ToString();
            }
            return text;
        }

        public static StorageObject LoadString(string text)
        {
            StorageObject store;
            using (var reader = new StringReader(text))
                store = Deserialize(reader);
            return store;
        }

        private static void Serialize(StorageObject store, TextWriter writer)
        {
            try
            {
                using (var jwriter = new JsonTextWriter(writer))
                    serializer.Serialize(jwriter, store);
            }
            catch
            {
                MessageBox.Show("Saving failed");
            }
        }

        private static StorageObject Deserialize(TextReader reader)
        {
            StorageObject store = null;

            try
            {
                using (var jreader = new JsonTextReader(reader))
                    store = serializer.Deserialize<StorageObject>(jreader);
            }
            catch
            {
                MessageBox.Show("Loading failed");
            }

            return store;
        }

        private static JsonSerializer _serializer;
        private static JsonSerializer serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new JsonSerializer()
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                }
                return _serializer;
            }
        }
    }
}
