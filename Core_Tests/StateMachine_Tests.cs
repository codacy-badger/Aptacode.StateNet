using Aptacode_StateMachine.StateNet.Core;
using NUnit.Framework;
using System;
using static Aptacode_StateMachine.StateNet.Core.BinaryTransitionAcceptanceResult;

namespace Aptacode.StateNet.Core_Tests
{
    public class StateMachine_Tests
    {
        public enum States { Begin, Playing, Paused, End };
        public enum Actions { Play, Pause, Stop };

        StateMachine<States, Actions> stateMachine;
        bool canPlay;

        [SetUp]
        public void Setup()
        {
            canPlay = true;
            stateMachine = new StateMachine<States, Actions>(States.Begin);
            stateMachine.Define(new BinaryTransition<States, Actions>(States.Begin, Actions.Play, States.Playing, States.End, new Func<BinaryTransitionAcceptanceResult>(() => {

                if(canPlay)
                    return new BinaryTransitionAcceptanceResult(BinaryChoice.Left, "Started Playing");
                else
                    return new BinaryTransitionAcceptanceResult(BinaryChoice.Right, "Could not start playing");

            }), "Start Playing"));

            stateMachine.Define(new InvalidTransition<States, Actions>(States.Begin, Actions.Pause, "Must be Playing to Pause"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Begin, Actions.Stop, States.End, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Stopped");
            }), "Stop before playing"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Playing, Actions.Play, States.Playing, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Kept playing");
            }), "Already Playing"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Playing, Actions.Pause, States.Paused, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Paused playback");
            }), "Already Playing"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Playing, Actions.Stop, States.End, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Stopped");
            }), "Stopped"));

            stateMachine.Define(new BinaryTransition<States, Actions>(States.Paused, Actions.Play, States.Playing, States.End, new Func<BinaryTransitionAcceptanceResult>(() => {

                if (canPlay)
                    return new BinaryTransitionAcceptanceResult(BinaryChoice.Left, "Resumed Playback");
                else
                    return new BinaryTransitionAcceptanceResult(BinaryChoice.Right, "Could not Resumed Playback");

            }), "Resume Playback"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Paused, Actions.Pause, States.Paused, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Already Paused");
            }), "Already Paused"));

            stateMachine.Define(new UnaryTransition<States, Actions>(States.Paused, Actions.Stop, States.End, new Func<UnaryTransitionAcceptanceResult>(() => {
                return new UnaryTransitionAcceptanceResult("Stopped");
            }), "Stopped"));


            stateMachine.Define(new InvalidTransition<States, Actions>(States.End, Actions.Play, "Cannot play from end state"));
            stateMachine.Define(new InvalidTransition<States, Actions>(States.End, Actions.Pause, "Cannot pause from end state"));
            stateMachine.Define(new InvalidTransition<States, Actions>(States.End, Actions.Stop, "Cannot stop from end state"));

        }

        [Test]
        public void InitialState()
        {
            Assert.AreEqual(States.Begin, stateMachine.State, "Initial state should be 'Begin'");
        }

        [Test]
        public void TransitionEvent()
        {
            stateMachine.OnTransition += (s, e) =>
            {
                Assert.AreEqual(States.Begin, e.OldState, "Should have been in the 'Begin' state");
                Assert.AreEqual(Actions.Play, e.Action, "Action should have been 'Play'");
                Assert.AreEqual(States.Playing, e.NewState, "Should have been in the 'Begin' state");
                Assert.AreEqual(States.Playing, stateMachine.State, "StateMachine state should be updated");
            };

            stateMachine.Apply(Actions.Play);
        }

        [Test]
        public void ValidUnaryTransition()
        {
            Assert.AreEqual(States.Begin, stateMachine.State, "Initial state should be 'Begin'");
            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.Playing, stateMachine.State, "should have moved into 'Playing' state");
        }

        [Test]
        public void DuplicateUnaryTransition()
        {
            Assert.Throws<DuplicateTransitionException<States,Actions>>(() => {
                stateMachine.Define(new UnaryTransition<States, Actions>(States.Begin, Actions.Pause, States.Paused, new Func<UnaryTransitionAcceptanceResult>(() => {
                    return new UnaryTransitionAcceptanceResult("Paused");
                }), "Paused before playing"));
            });
        }


        [Test]
        public void MultipleTransitions()
        {
            Assert.AreEqual(States.Begin, stateMachine.State, "Initial state should be 'Begin'");
            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.Playing, stateMachine.State, "should have moved into 'Playing' state");
            stateMachine.Apply(Actions.Pause);
            Assert.AreEqual(States.Paused, stateMachine.State, "should have moved into 'Paused' state");
            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.Playing, stateMachine.State, "should have moved into 'Playing' state");
            stateMachine.Apply(Actions.Pause);
            Assert.AreEqual(States.Paused, stateMachine.State, "should have moved into 'Paused' state");
            stateMachine.Apply(Actions.Stop);
            Assert.AreEqual(States.End, stateMachine.State, "should have moved into 'End' state");
        }

        [Test]
        public void BinaryTransition()
        {
            Assert.AreEqual(States.Begin, stateMachine.State, "Initial state should be 'Begin'");
            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.Playing, stateMachine.State, "should have moved into 'Playing' state");
            stateMachine.Apply(Actions.Pause);
            Assert.AreEqual(States.Paused, stateMachine.State, "should have moved into 'Paused' state");

            canPlay = false;

            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.End, stateMachine.State, "should have moved into 'End' state");
        }

        [Test]
        public void InvalidTransition()
        {
            Assert.AreEqual(States.Begin, stateMachine.State, "Initial state should be 'Begin'");
            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.Playing, stateMachine.State, "should have moved into 'Playing' state");
            stateMachine.Apply(Actions.Pause);
            Assert.AreEqual(States.Paused, stateMachine.State, "should have moved into 'Paused' state");

            canPlay = false;

            stateMachine.Apply(Actions.Play);
            Assert.AreEqual(States.End, stateMachine.State, "should have moved into 'End' state");

            Assert.Throws<InvalidTransitionException<States, Actions>>(() => {
                stateMachine.Apply(Actions.Play);
            });

            Assert.AreEqual(States.End, stateMachine.State, "should have remained in the 'End' state");
        }


    }
}