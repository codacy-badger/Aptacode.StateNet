﻿namespace Aptacode.StateNet.TransitionResult
{
    public enum BinaryChoice
    {
        Left,
        Right
    }

    public class BinaryTransitionResult : TransitionResult
    {
        public BinaryTransitionResult(BinaryChoice choice, string message, bool success) : base(message, success)
        {
            Choice = choice;
        }

        public BinaryTransitionResult(BinaryChoice choice, string message) : this(choice, message, true)
        {
        }

        public BinaryChoice Choice { get; set; }
    }
}