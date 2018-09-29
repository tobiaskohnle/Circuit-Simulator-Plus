using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace CircuitSimulatorPlus
{
    static class StorageUtil
    {
        public static SerializedGate Load(string filepath)
        {
            SerializedGate store = null;
            using (var reader = new StreamReader(filepath))
                store = Deserialize(reader);
            return store;
        }

        public static SerializedGate Load(Stream stream)
        {
            SerializedGate store = null;
            using (var reader = new StreamReader(stream))
                store = Deserialize(reader);
            return store;
        }

        public static void Save(string filepath, SerializedGate store)
        {
            using (var writer = new StreamWriter(filepath))
                Serialize(store, writer);
        }

        public static string CreateText(SerializedGate store)
        {
            string text = "";
            using (var writer = new StringWriter())
            {
                Serialize(store, writer);
                text = writer.ToString();
            }
            return text;
        }

        public static SerializedGate LoadString(string text)
        {
            SerializedGate store;
            using (var reader = new StringReader(text))
                store = Deserialize(reader);
            return store;
        }

        private static void Serialize(SerializedGate store, TextWriter writer)
        {
            try
            {
                using (var jwriter = new JsonTextWriter(writer))
                    serializer.Serialize(jwriter, store);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Saving failed");
            }
        }

        private static SerializedGate Deserialize(TextReader reader)
        {
            SerializedGate store = null;

            try
            {
                using (var jreader = new JsonTextReader(reader))
                    store = serializer.Deserialize<SerializedGate>(jreader);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading failed");
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
