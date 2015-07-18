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

using NUnit.Framework;
using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solutions.TSP.GA.Operators;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.GA.Operators
{
    /// <summary>
    /// Holds tests for the EAX operator.
    /// </summary>
    [TestFixture]
    public class EAXOperatorTests
    {
        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverFixed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);

            // create solutions.
            var solution1 = new Route(new int[] { 0, 1, 2, 3, 4 }, 4);
            var solution2 = new Route(new int[] { 0, 1, 3, 2, 4 }, 4);

            // execute crossover.
            var crossover = new EAXOperator();
            double fitness;
            var result = crossover.Apply(problem, new MinimumWeightObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverClosed()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);

            // create solutions.
            var solution1 = new Route(new int[] { 0, 1, 2, 3, 4 }, 0);
            var solution2 = new Route(new int[] { 0, 1, 3, 2, 4 }, 0);

            // execute crossover.
            var crossover = new EAXOperator();
            double fitness;
            var result = crossover.Apply(problem, new MinimumWeightObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        /// <summary>
        /// Tests the EAX operator on a problem with 5 customers.
        /// </summary>
        [Test]
        public void Test5CrossOverOpen()
        {
            // create problem.
            var problem = TSPHelper.CreateTSP(0, 5, 10);

            // create solutions.
            var solution1 = new Route(new int[] { 0, 1, 2, 3, 4 }, null);
            var solution2 = new Route(new int[] { 0, 1, 3, 2, 4 }, null);

            // execute crossover.
            var crossover = new EAXOperator();
            double fitness;
            var result = crossover.Apply(problem, new MinimumWeightObjective(), solution1, solution2, out fitness);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }
    }
}