﻿using System.Collections.Generic;
using System.Linq;
using Aptacode.StateNet.Network;
using NUnit.Framework;

namespace Aptacode.StateNet.Tests.AttributeTests
{
    /// <summary>
    ///     Checks if the various states in a network are being assigned to correctly.
    ///     Primary focus is testing that the *initial* Network reflection is behaving as expected.
    /// </summary>
    public class BasicAttributeAssignmentTests
    {
        [Test]
        public void StatesCreatedUsingFields()
        {
            var network = new TwoStateStartToEndNetwork();
            var states = new List<State>(network.GetStates());

            Assert.AreEqual(2, states.Count);
            Assert.AreEqual("Start", network.StartTestState.Name);
            Assert.AreEqual("End", network.EndTestState.Name);
        }

        [Test(Description =
            "Should find 3 states when instantiating a class that has 3 properties with State Attributes")]
        public void StatesCreatedUsingProperties()
        {
            var network = new TwoStatePropertyAttributeNetwork();
            var states = new List<State>(network.GetStates());

            Assert.AreEqual(3, states.Count);
            Assert.AreEqual("Start", network.StartTestState.Name);
            Assert.AreEqual("End", network.EndTestState.Name);
            //Deliberately not using GetAll("Private"), as (for now) it also changes the network  
            Assert.AreEqual(1, network.GetStates().Count(state => state.Name == "Private"));
        }

        [Test]
        public void IsStartStateSetByFields()
        {
            var network = new TwoStateStartToEndNetwork();

            Assert.AreEqual("Start", network.StartTestState?.Name);
        }

        [Test(Description = "Should have an assigned start state based on use of StartStateAttribute")]
        public void IsStartStateSetUsingProperties()
        {
            var network = new TwoStatePropertyAttributeNetwork();

            Assert.AreEqual("Start", network.StartTestState?.Name);
        }

        [Test(Description = "Should create a 1-to-1 (one-way) connection between start and end states")]
        public void SimpleConnectionCreatedByFields()
        {
            //Arrange & Act
            var network = new TwoStateStartToEndNetwork();
            var connections = network[network.StartState];

            //Assert
            Assert.AreEqual(1, connections.Count(), "Should have only one connection");
            Assert.IsTrue(network.IsValid());
        }
    }
}