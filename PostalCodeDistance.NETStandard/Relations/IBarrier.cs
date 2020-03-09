using PostalCodeDistance.NETStandard.Entities;

namespace PostalCodeDistance.NETStandard.Relations
{
    public interface IBarrier
    {
        public bool WillIntersect(Point point1, Point point2);
    }
}
