﻿using System;

namespace CircuitSimulatorPlus
{
    public class ConnectionNodeHitbox : LineHitbox
    {
        ConnectionNode connectionNode;

        public ConnectionNodeHitbox(ConnectionNode connectionNode) : base(ConnectionNode.HitboxWidth)
        {
            this.connectionNode = connectionNode;

            vert = false;
            UpdateHitbox();
        }

        public override void UpdateHitbox()
        {
            x = connectionNode.Position.X;
            y = connectionNode.Position.Y;
            z = connectionNode.CableAnchorPoint.X;
        }
    }
}
