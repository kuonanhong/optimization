﻿// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the Local1Shift local search operator.
    /// </summary>
    [TestFixture]
    public class Local1ShiftTimeWindowTests
    {
        /// <summary>
        /// Initializes for these tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };
        }

        /// <summary>
        /// Tests an infeasible route where a violated customer can be moved backwards.
        /// </summary>
        [Test]
        public void TestOneShiftViolatedBackward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.MoveViolatedBackward(problem, objective, route, out delta)); // shifts 2 after 0.

            // test result.
            Assert.AreEqual(1, delta);
            Assert.AreEqual(new int[] { 0, 2, 1, 3, 4 }, route.ToArray());

            // create a route with one shift.
            route = new Optimization.Tours.Tour(new int[] { 0, 4, 1, 3, 2 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            Assert.IsTrue(localSearch.MoveViolatedBackward(problem, objective, route, out delta)); // shifts 2 after 0

            // test result.
            Assert.AreEqual(5, delta);
            Assert.AreEqual(new int[] { 0, 2, 4, 1, 3 }, route.ToArray());

            // create a feasible route.
            route = new Optimization.Tours.Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            Assert.IsFalse(localSearch.MoveViolatedBackward(problem, objective, route, out delta));
        }

        /// <summary>
        /// Tests an infeasible route where a non-violated customer can be moved forward.
        /// </summary>
        [Test]
        public void TestOneShiftNonViolatedForward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.MoveNonViolatedForward(problem, objective, route, out delta)); // shifts 1 after 2.

            // test result.
            Assert.AreEqual(1, delta);
            Assert.AreEqual(new int[] { 0, 2, 1, 3, 4 }, route.ToArray());

            // create a feasible route.
            route = new Optimization.Tours.Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            Assert.IsFalse(localSearch.MoveNonViolatedForward(problem, objective, route, out delta));
        }

        /// <summary>
        /// Tests an infeasible route where a non-violated customer can be moved backward.
        /// </summary>
        [Test]
        public void TestOneShiftNonViolatedBackward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            problem.Times[0][3] = 1;
            problem.Times[3][1] = 1;
            problem.Windows[1] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.MoveNonViolatedBackward(problem, objective, route, out delta)); // shifts 3 after 0.

            // test result.
            Assert.AreEqual(7, delta);
            Assert.AreEqual(new int[] { 0, 3, 1, 2, 4 }, route.ToArray());

            // create a feasible route.
            route = new Optimization.Tours.Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            Assert.IsFalse(localSearch.MoveNonViolatedBackward(problem, objective, route, out delta));
        }

        /// <summary>
        /// Tests an infeasible route where a violated customer can be moved forward.
        /// </summary>
        [Test]
        public void TestOneShiftViolatedForward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            problem.Times[0][3] = 1;
            problem.Times[3][1] = 1;
            problem.Windows[1] = new TimeWindow()
            {
                Min = 1,
                Max = 3
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 0,
                Max = 2
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.MoveViolatedForward(problem, objective, route, out delta)); // shifts 1 after 3.

            // test result.
            Assert.AreEqual(25, delta);
            Assert.AreEqual(new int[] { 0, 3, 1, 2, 4 }, route.ToArray());

            // create a feasible route.
            route = new Optimization.Tours.Tour(new int[] { 0, 3, 1, 2, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            Assert.IsFalse(localSearch.MoveViolatedForward(problem, objective, route, out delta));
        }

        /// <summary>
        /// Tests the local1shift on an infeasible route where the last violated customer cannot be moved.
        /// </summary>
        [Test]
        public void TestFixedViolatedUnmovableCustomerValidForward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 4, 5, 10);
            problem.Times[0][1] = 2;
            problem.Times[1][2] = 2;
            problem.Times[2][3] = 2;
            problem.Times[3][4] = 2;
            problem.Times[4][0] = 2;
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.Apply(problem, objective, route, out delta)); // shifts 3 after 2

            // test result.
            Assert.AreEqual(23, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Tests the local1shift on an infeasible route where the last violated customer cannot be moved.
        /// </summary>
        [Test]
        public void TestFixedViolatedUnmovableCustomerValidBackward()
        {
            // create the problem and make sure 0->1->2->3->4 is the solution.
            var objective = new TSPTWFeasibleObjective();
            var problem = TSPTWHelper.CreateTSPTW(0, 4, 5, 10);
            problem.Times[0][1] = 2;
            problem.Times[1][2] = 2;
            problem.Times[2][3] = 2;
            problem.Times[3][4] = 2;
            problem.Times[4][0] = 2;
            problem.Windows[4] = new TimeWindow()
            {
                Min = 7,
                Max = 9
            };

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 3, 2, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new Local1Shift<TSPTWFeasibleObjective>();
            var delta = 0.0f;
            Assert.IsTrue(localSearch.MoveNonViolatedBackward(problem, objective, route, out delta)); // shifts 2 after 1

            // test result.
            Assert.AreEqual(23, delta);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Cleans up for these tests.
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            RandomGeneratorExtensions.Reset();
        }
    }
}