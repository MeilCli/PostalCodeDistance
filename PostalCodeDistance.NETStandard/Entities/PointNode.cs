using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Entities
{
    public class PointNode : Point
    {
        private readonly List<PointNode> nodes = new List<PointNode>();

        public IReadOnlyList<PointNode> Nodes => nodes;

        public PointNode(Point point) : base(point) { }

        public void AddNode(PointNode node)
        {
            if (nodes.Contains(node))
            {
                return;
            }

            nodes.Add(node);
        }
    }
}
