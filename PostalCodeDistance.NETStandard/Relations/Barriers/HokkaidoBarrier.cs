using PostalCodeDistance.NETStandard.Entities;

namespace PostalCodeDistance.NETStandard.Relations.Barriers
{
    public class HokkaidoBarrier : IBarrier
    {
        private readonly IBarrier[] barriers = new IBarrier[]
        {
            new NemuroBarrier(),
            new TugaruBarrier()
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

        private class NemuroBarrier : Barrier
        {
            protected override (double x, double y)[] GetBarrierPaths() => new (double, double)[]
            {
                (44.591980, 145.571415),
                (43.763490, 145.286245),
                (43.438979, 145.551046),
                (43.417846, 145.844946),
                (42.853009, 145.938063)
            };
        }

        private class TugaruBarrier : Barrier
        {
            protected override (double x, double y)[] GetBarrierPaths() => new (double, double)[]
            {
                (41.230684, 139.997216),
                (41.753139, 141.632406)
            };
        }
    }
}
