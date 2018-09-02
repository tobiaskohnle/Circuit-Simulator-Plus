using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircuitSimulatorPlus
{
    public class CableRenderer
    {
        Cable cable;

        public CableRenderer(Cable cable)
        {
            this.cable = cable;

            cable.OnRenderedChanged += OnRenderedChanged;

            if (cable.IsRendered)
            {
                OnRenderedChanged();
            }
        }

        public void OnRenderedChanged()
        {
            if (cable.IsRendered)
            {
            }
            else
            {
            }
        }

        public void OnSelectionChanged()
        {
        }

        public void OnEmptyChanged()
        {
        }

        public void OnStateChanged()
        {
        }

        public void OnPositionChanged()
        {
        }
    }
}
