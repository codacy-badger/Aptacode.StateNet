﻿using System.Collections.Generic;
using Aptacode.StateNet.Connections;

namespace Aptacode.StateNet.Interfaces
{
    public interface INodeGraph
    {
        IEnumerable<Node> GetAll();

        IEnumerable<Node> GetEndNodes();

        bool IsValid();

        Node StartNode { get; set; }

        Node this[string nodeName] { get; }
        NodeConnections this[string nodeName, string action] { get; }
        NodeConnections this[Node node, string action] { get; }
    }

}
