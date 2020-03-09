using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Relations
{
    public abstract class Barrier : IBarrier
    {
        private ((int x, int y) p1, (int x, int y) p2)[]? computedBarrierLines;

        protected abstract (double x, double y)[] GetBarrierPaths();

        public bool WillIntersect(Point point1, Point point2)
        {
            static (int x, int y) compute(Point point)
            {
                int x = (int)((point.Latitude - RelationConnector.OffsetX) * RelationConnector.Scale);
                int y = (int)((point.Longitude - RelationConnector.OffsetY) * RelationConnector.Scale);
                return (x, y);
            }

            var (a, b) = (compute(point1), compute(point2));

            foreach (var (p1, p2) in computeBarrierLines())
            {
                // https://www.hiramine.com/programming/graphics/2d_segmentintersection.html
                int t1 = p1.x - a.x;
                int t2 = p1.y - a.y;
                int t3 = (b.x - a.x) * (p2.y - p1.y) - (b.y - a.y) * (p2.x - p1.x);
                int t4 = (p2.y - p1.y) * t1 - (p2.x - p1.x) * t2;
                int t5 = (b.y - a.y) * t1 - (b.x - a.x) * t2;

                if (t3 == 0)
                {
                    if (t4 == 0 && t5 == 0)
                    {
                        // 重複
                        return true;
                    }
                    // 平行
                    return false;
                }

                double r = (double)t4 / t3;
                double s = (double)t5 / t3;

                if (0 <= r && r <= 1 && 0 <= s && s <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        private ((int x, int y) p1, (int x, int y) p2)[] computeBarrierLines()
        {
            if (computedBarrierLines is { })
            {
                return computedBarrierLines;
            }

            static (int, int) compute((double x, double y) point)
            {
                int x = (int)((point.x - RelationConnector.OffsetX) * RelationConnector.Scale);
                int y = (int)((point.y - RelationConnector.OffsetY) * RelationConnector.Scale);
                return (x, y);
            }

            var barrierPaths = GetBarrierPaths();
            var result = new List<((int, int), (int, int))>();

            for (int i = 0; i < barrierPaths.Length - 1; i++)
            {
                result.Add((compute(barrierPaths[i]), compute(barrierPaths[i + 1])));
            }

            computedBarrierLines = result.ToArray();

            return computedBarrierLines;
        }
    }
}
