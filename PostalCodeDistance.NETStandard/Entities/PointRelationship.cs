using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PostalCodeDistance.NETStandard.Entities
{
    public class PointRelationship : Point
    {
        private List<Point> relationPoints = new List<Point>();

        [DataMember(Name = "relations")]
        public IReadOnlyList<Point> RelationPoints {
            get => relationPoints;
            set => relationPoints = new List<Point>(value);
        }

        public PointRelationship() : base() { }

        public PointRelationship(Point point) : base(point) { }

        public void AddRelationPoint(Point point)
        {
            if (relationPoints.Contains(point))
            {
                return;
            }
            relationPoints.Add(point);
        }
    }
}
