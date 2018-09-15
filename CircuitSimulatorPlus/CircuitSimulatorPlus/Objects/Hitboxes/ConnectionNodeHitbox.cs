using System;

namespace CircuitSimulatorPlus
{
    public class ConnectionNodeHitbox : LineHitbox
    {
        ConnectionNode connectionNode;

        public ConnectionNodeHitbox(ConnectionNode connectionNode) : base(ConnectionNode.HitboxWidth, ConnectionNode.DistanceFactor)
        {
            this.connectionNode = connectionNode;

            vert = false;
        }

        public override void UpdateHitbox()
        {
            x = connectionNode.Position.X;
            y = connectionNode.Position.Y;
            z = connectionNode.CableAnchorPoint.X;
        }
    }
}
