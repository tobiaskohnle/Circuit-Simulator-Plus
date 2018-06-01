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
        static Dictionary<ConnectionNode, int> nodeToId = new Dictionary<ConnectionNode, int>();
        static Dictionary<int, List<ConnectionNode>> idToNodes = new Dictionary<int, List<ConnectionNode>>();

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
                if (!nodeToId.ContainsKey(outNode))
                {
                    if (outNode.IsEmpty)
                        nodeToId[outNode] = 0;
                    else
                    {
                        bool found = false;
                        foreach (ConnectionNode nextNode in outNode.NextConnectedTo)
                        {
                            if (nodeToId.ContainsKey(nextNode))
                            {
                                nodeToId[outNode] = nodeToId[nextNode];
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            nodeToId[outNode] = nextId++;
                    }
                }
                store.OutputConnections[i] = nodeToId[outNode];

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
                if (!nodeToId.ContainsKey(inNode))
                {
                    ConnectionNode backNode = inNode.BackConnectedTo;
                    if (inNode.IsEmpty)
                        nodeToId[inNode] = 0;
                    else if (nodeToId.ContainsKey(backNode))
                        nodeToId[inNode] = nodeToId[backNode];
                    else
                        nodeToId[inNode] = nextId++;
                }
                store.InputConnections[i] = nodeToId[inNode];

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
                InputNode node = new InputNode(gate);
                gate.Input.Add(node);
                if (id != 0)
                {
                    if (idToNodes.ContainsKey(id))
                    {
                        foreach (ConnectionNode connected in idToNodes[id])
                        {
                            if (connected == node.BackConnectedTo)
                                continue;
                            bool isAlreadyConneced = false;
                            foreach (ConnectionNode alreadyConnected in node.NextConnectedTo)
                            {
                                if (connected == alreadyConnected)
                                {
                                    isAlreadyConneced = true;
                                    break;
                                }
                            }
                            if (!isAlreadyConneced)
                            {
                                connected.ConnectTo(node);
                            }
                        }
                    }
                    else
                        idToNodes[id] = new List<ConnectionNode>();
                    idToNodes[id].Add(node);
                }
            }
            foreach (int index in storageObject.InvertedInputs)
                gate.Input[index].Invert();
            foreach (int id in storageObject.OutputConnections)
            {
                OutputNode node = new OutputNode(gate);
                gate.Output.Add(node);
                if (id != 0)
                {
                    if (idToNodes.ContainsKey(id))
                    {
                        foreach (ConnectionNode connected in idToNodes[id])
                        {
                            if (connected == node.BackConnectedTo)
                                continue;
                            bool isAlreadyConneced = false;
                            foreach (ConnectionNode alreadyConnected in node.NextConnectedTo)
                            {
                                if (connected == alreadyConnected)
                                {
                                    isAlreadyConneced = true;
                                    break;
                                }
                            }
                            if (!isAlreadyConneced)
                            {
                                node.ConnectTo(connected);
                            }
                        }
                    }
                    else
                        idToNodes[id] = new List<ConnectionNode>();
                    idToNodes[id].Add(node);
                }
            }
            foreach (int index in storageObject.InvertedOutputs)
                gate.Output[index].Invert();

            return gate;
        }
    }
}
