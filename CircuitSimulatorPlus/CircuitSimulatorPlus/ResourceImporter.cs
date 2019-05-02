using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    class ResourceImporter
    {
        MainWindow window;
        string resourceName;

        public ResourceImporter(MainWindow window, string resourceName)
        {
            this.window = window;
            this.resourceName = resourceName;
        }

        public Gate import()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return window.Import(stream);
        }
    }
}
