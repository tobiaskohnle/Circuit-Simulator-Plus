using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CircuitSimulatorPlus
{
    static class GateSerializer
    {
        public static SerializedGate SerializeTopLayer(ContextGate contextGate, List<Cable> cables)
        {
            var store = new SerializedGate();
            store.Name = contextGate.Name;
            store.Type = typeof(ContextGate).Name;
            store.Size = contextGate.Size;

            ExtractContext(store, contextGate, cables);

            return store;
        }

        public static SerializedGate Serialize(Gate gate)
        {
            var store = new SerializedGate();
            store.Name = gate.Name;
            store.Position = gate.Position;
            store.Tag = gate.Tag;
            store.Size = gate.Size;
            store.Type = gate.GetType().Name;

            for (int i = 0; i < gate.Input.Count; i++)
            {
                if (gate.Input[i].IsInverted)
                {
                    if (store.InvertedInputs == null)
                        store.InvertedInputs = new List<int>();
                    store.InvertedInputs.Add(i);
                }
                if (gate.Input[i].IsRisingEdge)
                {
                    if (store.RisingEdgeInputs == null)
                        store.RisingEdgeInputs = new List<int>();
                    store.RisingEdgeInputs.Add(i);
                }
                if (gate.Input[i].IsCentered)
                {
                    if (store.CenteredInputs == null)
                        store.CenteredInputs = new List<int>();
                    store.CenteredInputs.Add(i);
                }
            }

            for (int i = 0; i < gate.Output.Count; i++)
            {
                if (gate.Output[i].IsInverted)
                {
                    if (store.InvertedOutputs == null)
                        store.InvertedOutputs = new List<int>();
                    store.InvertedOutputs.Add(i);
                }
                if (gate.Output[i].State)
                {
                    if (store.InitialActiveOutputs == null)
                        store.InitialActiveOutputs = new List<int>();
                    store.InitialActiveOutputs.Add(i);
                }
                if (gate.Output[i].IsMasterSlave)
                {
                    if (store.MasterSlaveOutputs == null)
                        store.MasterSlaveOutputs = new List<int>();
                    store.MasterSlaveOutputs.Add(i);
                }
                if (gate.Output[i].IsCentered)
                {
                    if (store.CenteredOutputs == null)
                        store.CenteredOutputs = new List<int>();
                    store.CenteredOutputs.Add(i);
                }
            }

            if (gate is ContextGate)
                ExtractContext(store, (ContextGate)gate);

            return store;
        }

        private static void ExtractContext(SerializedGate store, ContextGate contextGate, List<Cable> cables = null)
        {
            var contextCopy = new List<Gate>(contextGate.Context);
            int nextId = 1;
            int nextEp = 1;
            var nodeToId = new Dictionary<ConnectionNode, int>();
            var nodeToCableEp = new Dictionary<ConnectionNode, int>();
            var switches = new List<SerializedGate>();
            var lights = new List<SerializedGate>();
            store.Context = new List<SerializedGate>();

            for (int i = 0; i < contextGate.Input.Count; i++)
            {
                var inputSwitch = new InputSwitch();
                inputSwitch.Output.Add(new OutputNode(inputSwitch));
                //inputSwitch.UpdateConnectionNodePos();
                ConnectionNode contextNode = contextGate.Input[i];
                ConnectionNode switchNode = inputSwitch.Output.First();
                switchNode.NextConnectedTo = contextNode.NextConnectedTo;
                inputSwitch.Position = new Point(0, i);
                contextCopy.Add(inputSwitch);
            }
            foreach (Gate innerGate in contextCopy)
            {
                foreach (OutputNode output in innerGate.Output)
                {
                    if (output.NextConnectedTo.Count > 0)
                    {
                        nodeToId[output] = nextId;
                        foreach (ConnectionNode next in output.NextConnectedTo)
                            nodeToId[next] = nextId;
                        nextId++;
                    }
                    else
                    {
                        nodeToId[output] = 0;
                    }
                }
            }

            for (int i = 0; i < contextGate.Output.Count; i++)
            {
                int id;
                var outputLight = new OutputLight();
                outputLight.Input.Add(new InputNode(outputLight));
                //outputLight.UpdateConnectionNodePos();
                ConnectionNode contextNode = contextGate.Output[i];
                if (nodeToId.ContainsKey(contextNode))
                {
                    id = nodeToId[contextNode];
                    nodeToId.Remove(contextNode);
                }
                else
                    id = 0;
                ConnectionNode switchNode = outputLight.Input.First();
                nodeToId[switchNode] = id;
                outputLight.Position = new Point(1, i);
                contextCopy.Add(outputLight);
            }

            foreach (Gate innerGate in contextCopy)
            {
                SerializedGate innerStore = Serialize(innerGate);
                if (innerGate is InputSwitch)
                    switches.Add(innerStore);
                else if (innerGate is OutputLight)
                    lights.Add(innerStore);
                innerStore.InputConnections = new int[innerGate.Input.Count];
                if (cables != null)
                    innerStore.CableEndPoints = new int[innerGate.Input.Count];
                for (int i = 0; i < innerGate.Input.Count; i++)
                {
                    ConnectionNode node = innerGate.Input[i];
                    if (nodeToId.ContainsKey(node))
                    {
                        innerStore.InputConnections[i] = nodeToId[node];
                        if (cables != null)
                        {
                            innerStore.CableEndPoints[i] = nextEp;
                            nodeToCableEp[node] = nextEp++;
                        }
                    }
                    else
                        innerStore.InputConnections[i] = 0;
                    if (node.Name != null)
                    {
                        if (innerStore.InputLabels == null)
                            innerStore.InputLabels = new string[innerGate.Input.Count];
                        innerStore.InputLabels[i] = node.Name;
                    }
                }
                innerStore.OutputConnections = new int[innerGate.Output.Count];
                for (int i = 0; i < innerGate.Output.Count; i++)
                {
                    ConnectionNode node = innerGate.Output[i];
                    if (nodeToId.ContainsKey(node))
                        innerStore.OutputConnections[i] = nodeToId[node];
                    else
                        throw new InvalidOperationException("Invalid connection");
                    if (node.Name != null)
                    {
                        if (innerStore.OutputLabels == null)
                            innerStore.OutputLabels = new string[innerGate.Output.Count];
                        innerStore.OutputLabels[i] = node.Name;
                    }
                }
                store.Context.Add(innerStore);
            }

            switches.Sort(ComparePosition);
            lights.Sort(ComparePosition);

            for (int i = 0; i < Math.Min(switches.Count, contextGate.Input.Count); i++)
            {
                switches[i].Name = contextGate.Input[i].Name;
            }
            for (int i = 0; i < Math.Min(lights.Count, contextGate.Output.Count); i++)
            {
                lights[i].Name = contextGate.Output[i].Name;
            }

            if (cables == null)
                return;
            store.Cables = new List<SerializedGate.Cable>();
            foreach (Cable cable in cables)
            {
                var cablestore = new SerializedGate.Cable();
                cablestore.Points = new List<Point>(cable.Points);
                ConnectionNode startNode, endNode;
                if (cable.StartNode is OutputNode)
                {
                    startNode = cable.StartNode;
                    endNode = cable.EndNode;
                }
                else
                {
                    endNode = cable.StartNode;
                    startNode = cable.EndNode;
                    cablestore.Points.Reverse();
                }
                if (nodeToId.ContainsKey(startNode))
                    cablestore.OutputConnection = nodeToId[startNode];
                if (nodeToCableEp.ContainsKey(endNode))
                    cablestore.EndPoint = nodeToCableEp[endNode];
                store.Cables.Add(cablestore);
            }
        }

        public static ContextGate DeserializeTopLayer(SerializedGate storageObject, List<Cable> cables)
        {
            if (storageObject.Type != "ContextGate")
                throw new Exception("Object does not store an ContextGate");
            ContextGate contextGate = new ContextGate();
            contextGate.Name = storageObject.Name;
            contextGate.Tag = storageObject.Tag;
            contextGate.Size = storageObject.Size;
            var idToNode = new Dictionary<int, ConnectionNode>();
            var cableEpToNode = new Dictionary<int, ConnectionNode>();
            var uncabledNodes = new LinkedList<ConnectionNode>();

            foreach (SerializedGate innerStore in storageObject.Context)
            {
                Gate innerGate = Deserialize(innerStore);
                if (!(innerGate is ContextGate))
                {
                    if (innerStore.InputLabels != null)
                        for (int i = 0; i < innerGate.Input.Count; i++)
                            innerGate.Input[i].Name = innerStore.InputLabels[i];
                    if (innerStore.OutputLabels != null)
                        for (int i = 0; i < innerGate.Output.Count; i++)
                            innerGate.Output[i].Name = innerStore.OutputLabels[i];
                }
                contextGate.Context.Add(innerGate);
                for (int i = 0; i < innerStore.OutputConnections.Count(); i++)
                {
                    int id = innerStore.OutputConnections[i];
                    if (id != 0)
                        idToNode[id] = innerGate.Output[i];
                }
            }

            for (int i = 0; i < storageObject.Context.Count; i++)
            {
                SerializedGate innerStore = storageObject.Context[i];
                Gate innerGate = contextGate.Context[i];
                for (int j = 0; j < innerStore.InputConnections.Count(); j++)
                {
                    int id = innerStore.InputConnections[j];
                    if (id != 0)
                    {
                        if (!idToNode.ContainsKey(id))
                            throw new InvalidOperationException("Invalid connection");
                        ConnectionNode thisNode = innerGate.Input[j];
                        ConnectionNode otherNode = idToNode[id];
                        otherNode.NextConnectedTo.Add(thisNode);
                        thisNode.BackConnectedTo = otherNode;
                        otherNode.IsEmpty = false;
                        thisNode.IsEmpty = false;
                        thisNode.State = thisNode.IsInverted ? !otherNode.State : otherNode.State;
                        if (innerStore.CableEndPoints != null)
                        {
                            int endpoint = innerStore.CableEndPoints[j];
                            cableEpToNode[endpoint] = thisNode;
                        }
                        uncabledNodes.AddLast(thisNode);
                    }
                }
            }

            if (storageObject.Cables == null)
                return contextGate;

            foreach (SerializedGate.Cable cablestore in storageObject.Cables)
            {
                if (!idToNode.ContainsKey(cablestore.OutputConnection))
                    continue;
                ConnectionNode outputNode = idToNode[cablestore.OutputConnection];
                ConnectionNode inputNode = cableEpToNode[cablestore.EndPoint];
                Cable cable = new Cable(outputNode);
                foreach (Point point in cablestore.Points)
                    cable.AddSegment(point);
                cable.ConnectTo(inputNode, false);
                cables.Add(cable);
                uncabledNodes.Remove(inputNode);
            }

            foreach (ConnectionNode node in uncabledNodes)
            {
                Cable cable = new Cable(node.BackConnectedTo);
                cable.ConnectTo(node, false);
                cables.Add(cable);
            }

            return contextGate;
        }

        public static Gate Deserialize(SerializedGate storageObject)
        {
            Gate gate;
            switch (storageObject.Type)
            {
            case "ContextGate":
                gate = new ContextGate();
                break;
            case "AndGate":
                gate = new AndGate();
                break;
            case "OrGate":
                gate = new OrGate();
                break;
            case "NopGate":
                gate = new NopGate();
                break;
            case "InputSwitch":
                gate = new InputSwitch();
                break;
            case "OutputLight":
                gate = new OutputLight();
                break;
            case "SegmentDisplay":
                gate = new SegmentDisplay();
                break;
            default:
                throw new InvalidOperationException("Unknown type");
            }

            gate.Name = storageObject.Name;
            gate.Tag = storageObject.Tag;
            gate.Position = storageObject.Position;
            gate.Size = storageObject.Size;

            if (storageObject.InputConnections == null)
                storageObject.InputConnections = new int[0];
            if (storageObject.OutputConnections == null)
                storageObject.OutputConnections = new int[0];

            if (storageObject.Type == "ContextGate")
            {
                ContextGate contextGate = gate as ContextGate;
                var idToNode = new Dictionary<int, ConnectionNode>();
                var inputStores = new List<SerializedGate>();
                var outputStores = new List<SerializedGate>();
                var gateStores = new List<SerializedGate>();

                foreach (SerializedGate innerStore in storageObject.Context)
                {
                    switch (innerStore.Type)
                    {
                    case "InputSwitch":
                        inputStores.Add(innerStore);
                        break;
                    case "OutputLight":
                        outputStores.Add(innerStore);
                        break;
                    default:
                        gateStores.Add(innerStore);
                        break;
                    }
                }

                inputStores.Sort(ComparePosition);
                outputStores.Sort(ComparePosition);

                foreach (SerializedGate gateStore in gateStores)
                {
                    Gate innerGate = Deserialize(gateStore);
                    contextGate.Context.Add(innerGate);
                    for (int i = 0; i < gateStore.OutputConnections.Count(); i++)
                    {
                        int id = gateStore.OutputConnections[i];
                        if (id != 0)
                            idToNode[id] = innerGate.Output[i];
                    }
                }
                foreach (SerializedGate inputStore in inputStores)
                {
                    int id = inputStore.OutputConnections.First();
                    var inputNode = new InputNode(contextGate);
                    inputNode.Name = inputStore.Name;
                    contextGate.Input.Add(inputNode);
                    if (id != 0)
                        idToNode[id] = inputNode;
                }

                for (int i = 0; i < gateStores.Count; i++)
                {
                    SerializedGate innerStore = gateStores[i];
                    Gate innerGate = contextGate.Context[i];
                    for (int j = 0; j < innerStore.InputConnections.Count(); j++)
                    {
                        int id = innerStore.InputConnections[j];
                        if (id != 0)
                        {
                            if (!idToNode.ContainsKey(id))
                                throw new InvalidOperationException("Invalid connection");
                            ConnectionNode thisNode = innerGate.Input[j];
                            ConnectionNode otherNode = idToNode[id];
                            otherNode.NextConnectedTo.Add(thisNode);
                            thisNode.BackConnectedTo = otherNode;
                            //otherNode.IsEmpty = false;
                            //thisNode.IsEmpty = false;
                            thisNode.State = thisNode.IsInverted ? !otherNode.State : otherNode.State;
                        }
                    }
                }
                foreach (SerializedGate outputStore in outputStores)
                {
                    int id = outputStore.InputConnections.First();
                    OutputNode contextNode = new OutputNode(contextGate);
                    contextNode.Name = outputStore.Name;
                    contextGate.Output.Add(contextNode);
                    if (id != 0)
                    {
                        if (!idToNode.ContainsKey(id))
                            throw new InvalidOperationException("Invalid connection");
                        ConnectionNode otherNode = idToNode[id];
                        otherNode.NextConnectedTo.Add(contextNode);
                        contextNode.BackConnectedTo = otherNode;
                        //otherNode.IsEmpty = false;
                        //contextNode.IsEmpty = false;
                        contextNode.State = otherNode.State;
                    }
                }
            }
            else
            {
                foreach (int id in storageObject.InputConnections)
                    gate.Input.Add(new InputNode(gate));
                foreach (int id in storageObject.OutputConnections)
                    gate.Output.Add(new OutputNode(gate));
            }

            if (storageObject.InvertedInputs != null)
                foreach (int index in storageObject.InvertedInputs)
                {
                    gate.Input[index].Invert();
                    gate.Input[index].State = !gate.Input[index].State;
                }
            if (storageObject.InvertedOutputs != null)
                foreach (int index in storageObject.InvertedOutputs)
                    gate.Output[index].Invert();

            if (storageObject.InitialActiveOutputs != null)
                foreach (int index in storageObject.InitialActiveOutputs)
                    gate.Output[index].State = true;

            if (storageObject.RisingEdgeInputs != null)
                foreach (int index in storageObject.RisingEdgeInputs)
                    gate.Input[index].IsRisingEdge = true;

            if (storageObject.MasterSlaveOutputs != null)
                foreach (int index in storageObject.MasterSlaveOutputs)
                    gate.Output[index].IsMasterSlave = true;

            if (storageObject.CenteredInputs != null)
                foreach (int index in storageObject.CenteredInputs)
                    gate.Input[index].IsCentered = true;

            if (storageObject.CenteredOutputs != null)
                foreach (int index in storageObject.CenteredOutputs)
                    gate.Output[index].IsCentered = true;

            if (storageObject.Type == "InputSwitch")
                ((InputSwitch)gate).State = gate.Output.First().IsInverted ? !gate.Output.First().State : gate.Output.First().State;

            return gate;
        }

        private static int ComparePosition(SerializedGate a, SerializedGate b)
        {
            int res = (int)a.Position.Y - (int)b.Position.Y;
            if (res == 0)
                res = (int)a.Position.X - (int)b.Position.X;
            return res;
        }
    }
}
