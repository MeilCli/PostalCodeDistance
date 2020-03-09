using PostalCodeDistance.NETStandard.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PostalCodeDistance.NETStandard
{
    public class DistanceCalculator
    {
        private class State : IComparable<State>
        {
            private readonly PointNode[] nodeLoci;
            private readonly PointNode goalNode;

            public PointNode Node { get; }

            public double TotalDistance { get; }

            public double GoalDistance { get; }

            public State(PointNode node, PointNode goalNode)
            {
                nodeLoci = Array.Empty<PointNode>();
                this.goalNode = goalNode;
                Node = node;
                GoalDistance = distance(node, goalNode);
            }

            private State(PointNode node, PointNode[] nodeLoci, PointNode goalNode, double totalDistance)
            {
                (Node, this.nodeLoci, this.goalNode, TotalDistance) = (node, nodeLoci, goalNode, totalDistance);
                GoalDistance = distance(node, goalNode);
            }

            public State Next(PointNode node)
            {
                var newNodeLoci = new PointNode[nodeLoci.Length + 1];
                Array.Copy(nodeLoci, newNodeLoci, nodeLoci.Length);
                newNodeLoci[newNodeLoci.Length - 1] = Node;
                return new State(node, newNodeLoci, goalNode, TotalDistance + distance(Node, node));
            }

            public bool HasLocus(PointNode node)
            {
                return nodeLoci.Contains(node);
            }

            private double toRadian(double angle)
            {
                return angle * Math.PI / 180;
            }

            private double distance(Point point1, Point point2)
            {
                // http://danielsaidi.com/blog/2011/02/04/calculate-distance-and-bearing-between-two-positions
                double r = 6378137.0;
                double latitude1 = toRadian(point1.Latitude);
                double latitude2 = toRadian(point2.Latitude);
                double dLatitude = toRadian(point2.Latitude - point1.Latitude);
                double dLongitude = toRadian(Math.Abs(point2.Longitude - point1.Longitude));

                double dPhi = Math.Log(Math.Tan(latitude2 / 2 + Math.PI / 4) / Math.Tan(latitude1 / 2 + Math.PI / 4));
                double q = Math.Cos(latitude1);

                if (dPhi != 0)
                {
                    q = dLatitude / dPhi;
                }
                if (Math.PI < dLongitude)
                {
                    dLongitude = 2 * Math.PI - dLongitude;
                }

                return Math.Sqrt(dLatitude * dLatitude + q * q * dLongitude * dLongitude) * r;
            }

            public int CompareTo(State other)
            {
                if (Node.Code == other.Node.Code)
                {
                    if (nodeLoci.SequenceEqual(other.nodeLoci))
                    {
                        return 0;
                    }
                    else
                    {
                        return nodeLoci.GetHashCode() - other.nodeLoci.GetHashCode();
                    }
                }

                if (GoalDistance < other.GoalDistance)
                {
                    return -1;
                }
                return 1;
            }
        }

        public double CalculateDistance(IReadOnlyList<PointNode> nodes, string sourceCode, string targetCode)
        {
            var sourceNode = nodes.First(x => x.Code == sourceCode);
            var targetNode = nodes.First(x => x.Code == targetCode);
            var sourceState = new State(sourceNode, targetNode);
            var queue = new SortedSet<State>();

            foreach (var relation in sourceNode.Nodes)
            {
                var newState = sourceState.Next(relation);
                queue.Add(newState);
            }

            State? targetState = null;

            while (queue.Count != 0)
            {
                var current = queue.First();

                if (current.Node.Code == targetCode)
                {
                    targetState = current;
                    break;
                }

                var adds = new List<State>();
                foreach (var relation in current.Node.Nodes)
                {
                    if (current.HasLocus(relation) is false)
                    {
                        var newState = current.Next(relation);
                        adds.Add(newState);

                        if (newState.Node.Code == targetCode)
                        {
                            targetState = newState;
                            queue.Clear();
                            break;
                        }
                    }
                }

                queue.Remove(current);

                // First() ~ Remove()の間にAddするとSortedSetの挙動がおかしくなる
                foreach (var add in adds)
                {
                    queue.Add(add);
                }
            }

            if (targetState is null)
            {
                throw new Exception("not found");
            }

            return targetState.TotalDistance;
        }
    }
}
