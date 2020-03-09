using PostalCodeDistance.NETStandard.Entities;
using PostalCodeDistance.NETStandard.Relations.Barriers;

namespace PostalCodeDistance.NETStandard.Relations
{
    public class BarrierCollection : IBarrier
    {
        private readonly IBarrier[] barriers = new IBarrier[]
        {
            new HokkaidoBarrier(),
            new HonsyuuBarrier(),
            new SikokuBarrier(),
            new KyuusyuuBarrier()
        };

        public bool WillIntersect(Point point1, Point point2)
        {
            foreach (var barrier in barriers)
            {
                if (barrier.WillIntersect(point1, point2))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
