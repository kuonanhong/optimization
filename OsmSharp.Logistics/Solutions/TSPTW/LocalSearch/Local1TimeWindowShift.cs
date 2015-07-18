﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solvers;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSPTW.LocalSearch
{
    /// <summary>
    /// A local search procedure to move around and improve the time window 'violations' in a solution.
    /// </summary>
    public class Local1TimeWindowShift : IOperator<ITSPTW, ITSPTWObjective, IRoute>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LCL_1SHFT_TW"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ITSPTWObjective objective)
        {
            return objective.Name == FeasibleObjective.FeasibleObjectiveName;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSPTW problem, ITSPTWObjective objective, IRoute solution, out double delta)
        {
            // STRATEGY: 
            // 1: try to move a violated customer backwards.
            // 2: try to move a non-violated customer forward.
            // 3: try to move a non-violated customer backward.
            // 4: try to move a violated customer forward.
            if(this.MoveViolatedBackward(problem, solution, out delta))
            { // success already, don't try anything else.
                return true;
            }
            if (this.MoveNonViolatedForward(problem, solution, out delta))
            { // success already, don't try anything else.
                return true;
            }
            if (this.MoveNonViolatedBackward(problem, solution, out delta))
            { // success already, don't try anything else.
                return true;
            }
            if (this.MoveViolatedForward(problem, solution, out delta))
            { // success already, don't try anything else.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The route.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool MoveViolatedBackward(ITSPTW problem, IRoute solution, out double delta)
        {
            // search for invalid customers.
            var enumerator = solution.GetEnumerator();
            double time = 0;
            double fitness = 0;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var window = problem.Windows[enumerator.Current];
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Weights[previous][enumerator.Current];
                }
                if (!window.IsValidAt(time) && position > 1)
                { // window is invalid and customer is not the first 'moveable' customer.
                    invalids.Add(new Tuple<int,int>(enumerator.Current, position));
                }

                // add the difference with the time window.
                fitness += window.MinDiff(time);

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach(var invalid in invalids)
            {
                // ok try the new position.
                for(var newPosition = 1; newPosition < invalid.Item2; newPosition++)
                {
                    var before = solution.GetCustomerAt(newPosition - 1);

                    // calculate new total min diff.
                    double newFitness = 0.0;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while(enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != invalid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Weights[previous][current];
                            }
                            newFitness += problem.Windows[current].MinDiff(time);
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Weights[current][invalid.Item1];
                                newFitness += problem.Windows[invalid.Item1].MinDiff(time);
                                previous = invalid.Item1;
                            }
                        }
                    }

                    if(newFitness < fitness)
                    {
                        delta = fitness - newFitness;
                        solution.ShiftAfter(invalid.Item1, before);
                        return true;
                    }
                }
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The route.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool MoveNonViolatedForward(ITSPTW problem, IRoute solution, out double delta)
        {
            // search for valid customers.
            var enumerator = solution.GetEnumerator();
            double time = 0;
            double fitness = 0;
            var position = 0;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var window = problem.Windows[enumerator.Current];
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Weights[previous][enumerator.Current];
                }
                if (position > 0 && position < problem.Weights.Length - 1 && window.IsValidAt(time))
                { // window is invalid and customer is not the first 'moveable' customer.
                    valids.Add(new Tuple<int, int>(enumerator.Current, position));
                }

                // add the difference with the time window.
                fitness += window.MinDiff(time);

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = valid.Item2 + 1; newPosition < problem.Weights.Length; newPosition++)
                {
                    var before = solution.GetCustomerAt(newPosition);

                    // calculate new total min diff.
                    double newFitness = 0.0;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != valid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Weights[previous][current];
                            }
                            newFitness += problem.Windows[current].MinDiff(time);
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Weights[current][valid.Item1];
                                newFitness += problem.Windows[valid.Item1].MinDiff(time);
                                previous = valid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    {
                        delta = fitness - newFitness;
                        solution.ShiftAfter(valid.Item1, before);
                        return true;
                    }
                }
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The route.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool MoveNonViolatedBackward(ITSPTW problem, IRoute solution, out double delta)
        {
            // search for the valid customers.
            var enumerator = solution.GetEnumerator();
            double time = 0;
            double fitness = 0;
            var position = 0;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var window = problem.Windows[enumerator.Current];
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Weights[previous][enumerator.Current];
                }
                if (position > 1 && window.IsValidAt(time))
                { // window is invalid and customer is not the first 'moveable' customer.
                    valids.Add(new Tuple<int, int>(enumerator.Current, position));
                }

                // add the difference with the time window.
                fitness += window.MinDiff(time);

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = 1; newPosition < valid.Item2; newPosition++)
                {
                    var before = solution.GetCustomerAt(newPosition - 1);

                    // calculate new total min diff.
                    double newFitness = 0.0;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != valid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Weights[previous][current];
                            }
                            newFitness += problem.Windows[current].MinDiff(time);
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Weights[current][valid.Item1];
                                newFitness += problem.Windows[valid.Item1].MinDiff(time);
                                previous = valid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    {
                        delta = fitness - newFitness;
                        solution.ShiftAfter(valid.Item1, before);
                        return true;
                    }
                }
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The route.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool MoveViolatedForward(ITSPTW problem, IRoute solution, out double delta)
        {
            // search for the invalid customers.
            var enumerator = solution.GetEnumerator();
            double time = 0;
            double fitness = 0;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var window = problem.Windows[enumerator.Current];
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Weights[previous][enumerator.Current];
                }
                if (!window.IsValidAt(time) && position > 0 && position < problem.Weights.Length)
                { // window is invalid and customer is not the first 'moveable' customer.
                    invalids.Add(new Tuple<int, int>(enumerator.Current, position));
                }

                // add the difference with the time window.
                fitness += window.MinDiff(time);

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var invalid in invalids)
            {
                // ok try the new position.
                for (var newPosition = invalid.Item2 + 1; newPosition < problem.Weights.Length; newPosition++)
                {
                    var before = solution.GetCustomerAt(newPosition);

                    // calculate new total min diff.
                    double newFitness = 0.0;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != invalid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Weights[previous][current];
                            }
                            newFitness += problem.Windows[current].MinDiff(time);
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Weights[current][invalid.Item1];
                                newFitness += problem.Windows[invalid.Item1].MinDiff(time);
                                previous = invalid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    {
                        delta = fitness - newFitness;
                        solution.ShiftAfter(invalid.Item1, before);
                        return true;
                    }
                }
            }
            delta = 0;
            return false;
        }

    }
}