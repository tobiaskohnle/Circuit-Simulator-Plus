using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    public static class Constants
    {
        public const string WindowTitle = "Circuit Simulator Plus";
        public const string FileFilter = "Circuit Simulator Plus Circuit|*" + FileExtention;
        public const string DefaultTitle = "untitled";
        public const string FileExtention = ".tici";
        public const double MinPxMouseMoved = 5;
        public const double DefaultGridSize = 20;
        public const double ScaleFactor = 1.1;
        public const double LineRadius = 1d / 20;
        public const double LineWidth = 1d / 10;
        public const double InversionDotRadius = 1d / 4;
        public const double InversionDotDiameter = 1d / 2;
        public const double CableJointSize = 1d / 3;
        public const double ConnectionNodeLineLength = 1d;
        public const int UndoBufferSize = 3;
        public const int RisingEdgePulse = 4;
    }
}
