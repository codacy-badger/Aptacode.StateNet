﻿using Aptacode.StateNet.NodeMachine.Choices;
using Aptacode.StateNet.NodeMachine.Nodes;
using Aptacode.StateNet.NodeMachine.Options;
using System;
using System.Linq;

namespace Aptacode.StateNet.NodeMachine.Choosers.Deterministic
{
    public class SeptenaryDeterministicChooser : DeterministicChooser<SeptenaryChoice>
    {
        public SeptenaryDeterministicChooser(Node option1, Node option2, Node option3, Node option4, Node option5, Node option6, Node option7) : base(new SeptenaryOptions(option1, option2, option3, option4, option5, option6, option7), SeptenaryChoice.Item1) { }
    }
}