﻿using System;
using Aptacode.StateNet.Engine;

namespace Aptacode.StateNet.Network
{
    /// <summary>
    /// Evaluates a string Expression given the EngineHistory to return an integer weight
    /// </summary>
    public class ConnectionWeight : IEquatable<ConnectionWeight>
    {
        private string _expression;
        private Func<EngineHistory, int> _weightFunction;
        private static ScriptEvaluator Evaluator { get; } = new ScriptEvaluator();

        public static readonly int DefaultWeight = 0;
        public static readonly string DefaultExpression = DefaultWeight.ToString();
        public static readonly Func<EngineHistory, int> DefaultWeightFunction = (_) => DefaultWeight;

        public ConnectionWeight() : this(1)
        {
        }

        public ConnectionWeight(int weight) : this(weight.ToString())
        {
        }

        public ConnectionWeight(string expression)
        {
            Expression = expression;
        }

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;

                if (GetStaticWeightFunction(value, out _weightFunction))
                {
                    return;
                }

                if (GetWeightFunction(value, out _weightFunction))
                {
                    return;
                }

                _expression = DefaultExpression;
                _weightFunction = DefaultWeightFunction;
            }
        }
        /// <summary>
        /// Evaluates the weightFunction given the EngineHistory to return an integer weight
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public int Evaluate(EngineHistory history)
        {
            var result = _weightFunction(history);
            return result >= 0 ? result : 0;
        }

        #region WeightFunctions
        private static bool GetStaticWeightFunction(string expression, out Func<EngineHistory, int> weightFunction)
        {
            if (int.TryParse(expression, out var weight))
            {
                weightFunction = (_) => weight;
                return true;
            }

            weightFunction = null;
            return false;
        }

        private static bool GetWeightFunction(string expression, out Func<EngineHistory, int> weightFunction)
        {
            try
            {
                weightFunction = Evaluator.Compile(expression);
                return true;
            }
            catch (Exception)
            {
                weightFunction = null;
                return false;
            }
        }

        #endregion

        #region Equality

        public bool Equals(ConnectionWeight other)
        {
            return other != null && Expression.Equals(other.Expression);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionWeight other && Equals(other);
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return Expression;
        }

        #endregion
    }
}