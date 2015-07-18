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
using OsmSharp.Logistics.Solvers.VNS;

namespace OsmSharp.Logistics.Tests.Solvers.VNS
{
    /// <summary>
    /// Contains tests for the VNSSolver using mockup problem(s) and solution(s).
    /// </summary>
    [TestFixture]
    public class VNSSolverTests
    {
        /// <summary>
        /// Tests the name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create solver.
            var solver = new VNSSolver<ProblemMock, ObjectiveMock, SolutionMock>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock());

            Assert.AreEqual("VNS_[MOCK_GENERATOR_MOCK_PERTURBER_MOCK_LOCALSEARCH]", solver.Name);
        }

        /// <summary>
        /// Tests the VNSSolver without a stop condition but with a call to stop after a while.
        /// </summary>
        [Test]
        public void TestStop()
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            // create solver.
            var solver = new VNSSolver<ProblemMock, ObjectiveMock, SolutionMock>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock());

            // run solver but stop when after a few reported improvements.
            var intermediateCount = 0;
            var best = new SolutionMock() { Value = 1000 };
            solver.IntermidiateResult += (s) =>
            {
                intermediateCount++;
                if(intermediateCount > 5)
                { // this should never ever happen!
                    Assert.Fail("Solver has not stopped!");
                }
                if (intermediateCount == 5)
                { // stop solver.
                    solver.Stop();
                    best = s;
                }
            };
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());

            Assert.AreEqual(best.Value, solution.Value);
        }

        /// <summary>
        /// Tests the VNSSolver with a stop condition.
        /// </summary>
        [Test]
        public void TestStopCondition()
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            // create solver.
            var solver = new VNSSolver<ProblemMock, ObjectiveMock, SolutionMock>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock(), (i, p, o, s) =>
            {
                return s.Value < 100;
            });

            // run solver but stop when after a few reported improvements.
            var best = new SolutionMock() { Value = 1000 };
            solver.IntermidiateResult += (s) =>
            {
                best = s;
            };
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());

            Assert.AreEqual(best.Value, solution.Value);
            Assert.IsTrue(solution.Value < 100);
        }
    }
}