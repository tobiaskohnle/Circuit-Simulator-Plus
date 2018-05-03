using System;
using System.Collections.Generic;
using System.Windows;

namespace CircuitSimulatorPlus
{
    public class Gate
    {
        bool mutable;
        string name;
        string tag;
        Point position;

        List<Input> input;
        List<Output> output;
        

        public Int64 MikeStinktFaktor
        {
            get { return Int64.MaxValue; }
        }
    }
    // Hagen ☺
    //Benjamin Leon Dackermann 13.05.2001 

}
