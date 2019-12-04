﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.StateNet.TableMachine.Events;
using Aptacode.StateNet.TableMachine.Inputs;
using Aptacode.StateNet.TableMachine.States;
using Aptacode.StateNet.TableMachine.Tables;
using Aptacode.StateNet.TableMachine.Transitions;
using NLog;

namespace Aptacode.StateNet.TableMachine
{
    public class TableEngine : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<State, List<Action>> _callbackDictionary;
        private bool _isRunning;
        private readonly TransitionTable _stateTransitionTable;
        private readonly ConcurrentQueue<Input> inputQueue;

        /// <summary>
        /// Governs the transitions between states based on the inputs it receives
        /// </summary>
        public TableEngine(TransitionTable stateTransitionTable)
        {
            _stateTransitionTable = stateTransitionTable;
            inputQueue = new ConcurrentQueue<Input>();
            _callbackDictionary = new Dictionary<State, List<Action>>();
        }

        public event TableEngineEvent OnInvalidTransition;

        public event TableEngineEvent OnTransition;

        private void NextTransition()
        {
            if (inputQueue.TryDequeue(out var input))
            {
                try
                {
                    var transition = _stateTransitionTable.Get(State, input);
                    var nextState = transition.Apply();
                    LastInput = input;
                    UpdateState(nextState);
                }
                catch
                {
                    Logger.Error("Queued transition was invalid");
                    OnInvalidTransition?.Invoke(this, new StateTransitionArgs(State, input, State));
                }
            }
        }

        private void SetInitialState(State initialState)
        {
            if (_stateTransitionTable.States.Contains(initialState))
            {
                State = initialState;
            }
            else
            {
                State = _stateTransitionTable.States.First();
            }
        }

        private void UpdateState(State nextState)
        {
            var oldState = State;
            State = nextState;
            OnTransition?.Invoke(this, new StateTransitionArgs(oldState, LastInput, nextState));
            _callbackDictionary[nextState].ForEach(callback => callback?.Invoke());
        }

        /// <summary>
        /// Apply the transition which relates to the given input on the current state
        /// </summary>
        /// <param name="input"></param>
        public void Apply(Input input) => inputQueue.Enqueue(input);

        /// <summary>
        /// Set a transition to 'Undefined'
        /// </summary>
        /// <param name="transition"></param>
        public void Clear(BaseTransition transition) => _stateTransitionTable.Clear(transition);

        public void Dispose() => Stop();

        public void Start(State initialState)
        {
            SetInitialState(initialState);

            new TaskFactory().StartNew(async () =>
            {
                _isRunning = true;

                while (_isRunning)
                {
                    NextTransition();
                    await Task.Delay(1).ConfigureAwait(false);
                }
            });
        }

        public void Stop() => _isRunning = false;

        public void Subscribe(State state, Action callback)
        {
            if (!_callbackDictionary.TryGetValue(state, out var listeners))
            {
                listeners = new List<Action> { callback };
                _callbackDictionary.Add(state, listeners);
            }

            listeners.Add(callback);
        }

        public void UnSubscribe(State state, Action callback)
        {
            if (_callbackDictionary.TryGetValue(state, out var listeners))
            {
                listeners.Remove(callback);
            }
        }

        /// <summary>
        /// Returns the last input
        /// </summary>
        public Input LastInput { get; private set; }

        /// <summary>
        /// Returns the current State
        /// </summary>
        public State State { get; private set; }
    }
}