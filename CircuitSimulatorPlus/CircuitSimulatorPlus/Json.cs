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
    using StorageObject = List<Gate>;

    static class Json
    {
        /// <summary>
        /// Loads a Context from a JSON file.
        /// Shows a MessageBox on error.
        /// </summary>
        /// <param name="filepath">Path to load from</param>
        /// <returns>If successful returns a context object. Otherwise an "empty" representation is returned.</returns>
        public static StorageObject Load(string filepath)
        {
            StorageObject store = null;
            var ser = new JsonSerializer();

            try
            {
                using (var file = new StreamReader(filepath))
                using (var reader = new JsonTextReader(file))
                {
                    store = ser.Deserialize<StorageObject>(reader);
                }
            }
            catch
            {
                MessageBox.Show("Error loading file");
            }

            if (store == null)
                store = new List<Gate>();

            return store;
        }

        /// <summary>
        /// Saves a context as JSON.
        /// Shows MessageBox on error.
        /// </summary>
        /// <param name="filepath">Path to save to</param>
        /// <param name="store">Object to save</param>
        public static void Save(string filepath, StorageObject store)
        {
            var ser = new JsonSerializer();

            try
            {
                using (var file = new StreamWriter(filepath))
                using (var writer = new JsonTextWriter(file))
                {
                    ser.Serialize(writer, store);
                }
            }
            catch
            {
                MessageBox.Show("Error saving file");
            }
        }
    }
}
