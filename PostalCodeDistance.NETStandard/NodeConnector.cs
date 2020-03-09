using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard
{
    public class NodeConnector
    {
        private Dictionary<string, PointNode> cache = new Dictionary<string, PointNode>();

        public IReadOnlyList<PointNode> Connect(IReadOnlyList<PointRelationship> pointRelationships)
        {
            var result = new List<PointNode>();

            foreach (var pointRelationship in pointRelationships)
            {
                if (cache.TryGetValue(pointRelationship.Code, out PointNode current) is false)
                {
                    current = new PointNode(pointRelationship);
                    cache.Add(current.Code, current);
                }

                foreach (var relation in pointRelationship.RelationPoints)
                {
                    if (cache.TryGetValue(relation.Code, out PointNode relationLocation) is false)
                    {
                        relationLocation = new PointNode(relation);
                        cache.Add(relationLocation.Code, relationLocation);
                    }

                    current.AddNode(relationLocation);
                    relationLocation.AddNode(current);
                }

                result.Add(current);
            }

            return result;
        }
    }
}
