﻿// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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

namespace Itinero.Optimization.Tours
{
    /// <summary>
    /// Represents a pair (or a connection between two adjacent customers).
    /// </summary>
    public struct Pair
    {
        /// <summary>
        /// Creates a new pair.
        /// </summary>
        public Pair(int from, int to)
            : this()
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// Returns a description of this edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(this.From.ToInvariantString());
            stringBuilder.Append("->");
            stringBuilder.Append(this.To.ToInvariantString());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                this.To.GetHashCode();
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair)
            {
                return ((Pair)obj).From == this.From &&
                    ((Pair)obj).To == this.To;
            }
            return false;
        }
    }
}