using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    static class StorageConverter
    {
        public static StorageObject ToStorageObject(Gate gate)
        {
            StorageObject store = new StorageObject();
            store.Name = gate.Name;
            store.Position = gate.Position;
            switch (gate.Type)
            {
                case Gate.GateType.Context:
                    store.Type = "Context";
                    break;
                case Gate.GateType.And:
                    store.Type = "And";
                    break;
                case Gate.GateType.Or:
                    store.Type = "Or";
                    break;
                case Gate.GateType.Identity:
                    store.Type = "Identity";
                    break;
                default:
                    throw new InvalidOperationException("Unknown type");
            }

            for (int i = 0; i < gate.Input.Count; i++)
            {
                if (gate.Input[i].IsInverted)
                {
                    if (store.InvertedInputs == null)
                        store.InvertedInputs = new List<int>();
                    store.InvertedInputs.Add(i);
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
            }

            if (gate.HasContext)
            {
                int nextId = 1;
                var nodeToId = new Dictionary<ConnectionNode, int>();
                store.Context = new List<StorageObject>();
                store.InnerInputConnections = new int[gate.Input.Count];
                for (int i = 0; i < gate.Input.Count; i++)
                {
                    InputNode contextInput = gate.Input[i];
                    if (contextInput.NextConnectedTo.Count > 0)
                    {
                        nodeToId[contextInput] = nextId;
                        foreach (ConnectionNode next in contextInput.NextConnectedTo)
                            nodeToId[next] = nextId;
                        nextId++;
                        store.InnerInputConnections[i] = nextId;
                    }
                    else
                    {
                        store.InnerInputConnections[i] = 0;
                    }
                }
                foreach (Gate innerGate in gate.Context)
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
                store.InnerOutputConnections = new int[gate.Output.Count];
                for (int i = 0; i < gate.Output.Count; i++)
                {
                    if (nodeToId.ContainsKey(gate.Output[i]))
                        store.InnerOutputConnections[i] = nodeToId[gate.Output[i]];
                    else
                        store.InnerOutputConnections[i] = 0;
                }
                foreach (Gate innerGate in gate.Context)
                {
                    StorageObject innerStore = ToStorageObject(innerGate);
                    innerStore.InputConnections = new int[innerGate.Input.Count];
                    for (int i = 0; i < innerGate.Input.Count; i++)
                    {
                        if (nodeToId.ContainsKey(innerGate.Input[i]))
                            innerStore.InputConnections[i] = nodeToId[innerGate.Input[i]];
                        else
                            innerStore.InputConnections[i] = 0;
                    }
                    innerStore.OutputConnections = new int[innerGate.Output.Count];
                    for (int i = 0; i < innerGate.Output.Count; i++)
                    {
                        if (nodeToId.ContainsKey(innerGate.Output[i]))
                            innerStore.OutputConnections[i] = nodeToId[innerGate.Output[i]];
                        else
                            throw new InvalidOperationException("Invalid connection");
                    }
                    store.Context.Add(innerStore);
                }
            }

            return store;
        }

        public static Gate ToGate(StorageObject storageObject)
        {
            Gate.GateType type;
            switch (storageObject.Type)
            {
                case "Context":
                    type = Gate.GateType.Context;
                    break;
                case "And":
                    type = Gate.GateType.And;
                    break;
                case "Or":
                    type = Gate.GateType.Or;
                    break;
                case "Identity":
                    type = Gate.GateType.Identity;
                    break;
                default:
                    throw new InvalidOperationException("Unknown type");
            }
            Gate gate = new Gate(type);
            gate.Name = storageObject.Name;
            gate.Position = storageObject.Position;

            if (storageObject.InputConnections == null)
                storageObject.InputConnections = new int[0];

            foreach (int id in storageObject.InputConnections)
                gate.Input.Add(new InputNode(gate));
            if (storageObject.InvertedInputs != null)
                foreach (int index in storageObject.InvertedInputs)
                    gate.Input[index].Invert();

            if (storageObject.OutputConnections == null)
                storageObject.OutputConnections = new int[0];

            foreach (int id in storageObject.OutputConnections)
                gate.Output.Add(new OutputNode(gate));
            if (storageObject.InvertedOutputs != null)
                foreach (int index in storageObject.InvertedOutputs)
                    gate.Output[index].Invert();
            if (storageObject.InitialActiveOutputs != null)
                foreach (int index in storageObject.InitialActiveOutputs)
                    gate.Output[index].State = true;

            if (type == Gate.GateType.Context)
            {
                var idToNode = new Dictionary<int, ConnectionNode>();
                for (int i = 0; i < storageObject.InnerInputConnections.Count(); i++)
                {
                    int id = storageObject.InnerInputConnections[i];
                    if (id != 0)
                        idToNode[id] = gate.Input[i];
                }
                foreach (StorageObject innerStore in storageObject.Context)
                {
                    Gate innerGate = ToGate(innerStore);
                    gate.Context.Add(innerGate);
                    for (int i = 0; i < innerStore.OutputConnections.Count(); i++)
                    {
                        int id = innerStore.OutputConnections[i];
                        if (id != 0)
                            idToNode[id] = innerGate.Output[i];
                    }
                }
                for (int i = 0; i < storageObject.Context.Count; i++)
                {
                    StorageObject innerStore = storageObject.Context[i];
                    Gate innerGate = gate.Context[i];
                    for (int j = 0; j < innerStore.InputConnections.Count(); j++)
                    {
                        int id = innerStore.InputConnections[j];
                        if (id != 0)
                        {
                            if (idToNode.ContainsKey(id))
                            {
                                ConnectionNode outputNode = idToNode[id];
                                outputNode.ConnectTo(innerGate.Input[j]);
                                innerGate.Input[j].State = outputNode.State;
                            }
                            else
                                throw new InvalidOperationException("Invalid connection");
                        }
                    }
                }
                for (int i = 0; i < storageObject.InnerOutputConnections.Count(); i++)
                {
                    int id = storageObject.InnerOutputConnections[i];
                    if (id != 0)
                    {
                        if (idToNode.ContainsKey(id))
                            idToNode[id].ConnectTo(gate.Output[i]);
                        else
                            throw new InvalidOperationException("Invalid connection");
                    }
                }
            }

            return gate;
        }
    }
}
