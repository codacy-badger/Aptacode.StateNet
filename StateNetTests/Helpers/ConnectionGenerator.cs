﻿using System.Collections.Generic;
using Aptacode.StateNet.Connections;

namespace Aptacode.StateNet.Tests.Helpers
{
    public static class ConnectionGenerator
    {
        public static List<Connection> Generate(string fromState = "defaultState", string input = "defaultInput",
            params int[] choices)
        {
            var output = new List<Connection>();
            for (var i = 0; i < choices.Length; i++)
            {
                output.Add(new Connection(fromState, input, i.ToString(),
                    new Connections.ConnectionWeight(choices[i])));
            }

            return output;
        }

        public static List<Connection> Generate(params int[] choices)
        {
            var output = new List<Connection>();
            for (var i = 0; i < choices.Length; i++)
            {
                output.Add(new Connection("defaultState", "defaultInput", i.ToString(),
                    new Connections.ConnectionWeight(choices[i])));
            }

            return output;
        }
    }
}