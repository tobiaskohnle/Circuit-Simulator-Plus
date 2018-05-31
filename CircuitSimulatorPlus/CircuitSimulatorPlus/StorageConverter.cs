using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSimulatorPlus
{
    static class StorageConverter
    {
        static int nextId = 1;
        static Dictionary<ConnectionNode, int> ids = new Dictionary<ConnectionNode, int>();

        public static StorageObject ToStorageObject(Gate gate)
        {
            StorageObject store = new StorageObject();
            store.Name = gate.Name;
            store.Position = gate.Position;
            store.Type = gate.Type.ToString();
            
            if (gate.HasContext)
            {
                store.Context = new List<StorageObject>();
                foreach (Gate innerGate in gate.Context)
                    store.Context.Add(ToStorageObject(innerGate));
            }

            store.OutputConnections = new int[gate.Output.Count];
            for (int i = 0; i < gate.Output.Count; i++)
            {
                OutputNode outNode = gate.Output[i];
                if (!ids.ContainsKey(outNode))
                {
                    if (outNode.IsEmpty)
                        ids[outNode] = 0;
                    else
                    {
                        bool found = false;
                        foreach (ConnectionNode nextNode in outNode.NextConnectedTo)
                        {
                            if (ids.ContainsKey(nextNode))
                            {
                                ids[outNode] = ids[nextNode];
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            ids[outNode] = nextId++;
                    }
                }
                store.OutputConnections[i] = ids[outNode];

                if (outNode.IsInverted)
                {
                    if (store.InvertedOutputs == null)
                        store.InvertedOutputs = new List<int>();
                    store.InvertedOutputs.Add(i);
                }
            }

            store.InputConnections = new int[gate.Input.Count];
            for (int i = 0; i < gate.Input.Count; i++)
            {
                InputNode inNode = gate.Input[i];
                if (!ids.ContainsKey(inNode))
                {
                    ConnectionNode backNode = inNode.BackConnectedTo;
                    if (inNode.IsEmpty)
                        ids[inNode] = 0;
                    else if (ids.ContainsKey(backNode))
                        ids[inNode] = ids[backNode];
                    else
                        ids[inNode] = nextId++;
                }
                store.InputConnections[i] = ids[inNode];

                if (inNode.IsInverted)
                {
                    if (store.InvertedOutputs == null)
                        store.InvertedOutputs = new List<int>();
                    store.InvertedOutputs.Add(i);
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
                case "Not":
                    type = Gate.GateType.Not;
                    break;
                case "Identity":
                    type = Gate.GateType.Identity;
                    break;
                default:
                    throw new Exception("Unknown Type");
            }
            Gate gate = new Gate(type);
            gate.Name = storageObject.Name;
            gate.Position = storageObject.Position;
            if (type == Gate.GateType.Context)
            {
                foreach (StorageObject innerStore in storageObject.Context)
                    gate.Context.Add(ToGate(innerStore));
            }
            foreach (int id in storageObject.InputConnections)
            {
                gate.Input.Add(new InputNode(gate));
            }
            foreach (int id in storageObject.OutputConnections)
            {
                gate.Output.Add(new OutputNode(gate));
            }

            return gate;
        }
    }
}
