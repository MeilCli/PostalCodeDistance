using PostalCodeDistance.NETStandard.Entities;
using System;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Relations
{
    // max: 17963484
    // max: 3238549
    // max: 3226786
    // max: 3219382
    public class RelationConnector
    {
        // longitude: 122.987678~145.794707
        // latitude: 24.304386538461536~45.512988
        public const int Scale = 1000;
        public const int OffsetX = 122;
        public const int OffsetY = 24;
        public const int Width = 24 * Scale;
        public const int Height = 22 * Scale;
        private const int maxSearchLevel = Scale / 4;

        private readonly IBarrier barrier;

        public RelationConnector(IBarrier barrier) => this.barrier = barrier;

        public IReadOnlyList<PointRelationship> ConnectPoints(IReadOnlyList<Point> points)
        {
            var result = new List<PointRelationship>();

            List<Point>[,] map = packPoints(points);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    List<Point>? currentList = map[x, y];
                    if (currentList is null)
                    {
                        continue;
                    }

                    // 同一地点の最初の要素を隣接させる
                    var current = currentList[0];
                    var pointRelationship = new PointRelationship(current);

                    // 同一地点を連結させる
                    foreach (Point sameLocation in currentList)
                    {
                        if (current == sameLocation)
                        {
                            continue;
                        }
                        pointRelationship.AddRelationPoint(sameLocation);
                        var samePointRelationship = new PointRelationship(sameLocation);
                        samePointRelationship.AddRelationPoint(current);
                        result.Add(samePointRelationship);
                    }

                    // find for top direction
                    foreach (var foundPoint in findPointVertical(map, x, y, -1))
                    {
                        if (barrier.WillIntersect(current, foundPoint))
                        {
                            continue;
                        }
                        pointRelationship.AddRelationPoint(foundPoint);
                    }
                    // find for bottom direction
                    foreach (var foundPoint in findPointVertical(map, x, y, 1))
                    {
                        if (barrier.WillIntersect(current, foundPoint))
                        {
                            continue;
                        }
                        pointRelationship.AddRelationPoint(foundPoint);
                    }
                    // find for left direction
                    foreach (var foundPoint in findPointHorizontal(map, x, y, -1))
                    {
                        if (barrier.WillIntersect(current, foundPoint))
                        {
                            continue;
                        }
                        pointRelationship.AddRelationPoint(foundPoint);
                    }
                    // find for right direction
                    foreach (var foundPoint in findPointHorizontal(map, x, y, 1))
                    {
                        if (barrier.WillIntersect(current, foundPoint))
                        {
                            continue;
                        }
                        pointRelationship.AddRelationPoint(foundPoint);
                    }

                    result.Add(pointRelationship);
                }
            }

            return result;
        }

        private (int x, int y) toPosition(Point point)
        {
            int x = (int)((point.Longitude - OffsetX) * Scale);
            int y = (int)((point.Latitude - OffsetY) * Scale);
            return (x, y);
        }

        private List<Point>[,] packPoints(IReadOnlyList<Point> points)
        {
            List<Point>[,] map = new List<Point>[Width, Height];

            foreach (var point in points)
            {
                (int x, int y) = toPosition(point);
                if (map[x, y] is null)
                {
                    map[x, y] = new List<Point> { point };
                }
                else
                {
                    map[x, y].Add(point);
                }
            }

            return map;
        }

        private List<Point> findPointVertical(List<Point>[,] map, int baseX, int baseY, int dy)
        {
            var result = new List<Point>();

            int level = 1;
            for (int y = baseY + dy; 0 <= y && y < Height; y += dy)
            {
                bool isFound = false;
                for (int x = Math.Max(baseX - level, 0); 0 <= x && x < Width && x < baseX + level; x++)
                {
                    if (map[x, y] is null)
                    {
                        continue;
                    }
                    isFound = true;
                    // 同地点は隣接扱いしてるので辺の数を減らすために1つだけ隣接させる
                    result.Add(map[x, y][0]);
                }
                if (isFound)
                {
                    break;
                }
                if (level == maxSearchLevel)
                {
                    break;
                }
                level += 1;
            }

            return result;
        }

        private List<Point> findPointHorizontal(List<Point>[,] map, int baseX, int baseY, int dx)
        {
            var result = new List<Point>();

            int level = 1;
            for (int x = baseX + dx; 0 <= x && x < Width; x += dx)
            {
                bool isFound = false;
                for (int y = Math.Max(baseY - level, 0); 0 <= y && y < Height && y < baseY + level; y++)
                {
                    if (map[x, y] is null)
                    {
                        continue;
                    }
                    isFound = true;
                    // 同地点は隣接扱いしてるので辺の数を減らすために1つだけ隣接させる
                    result.Add(map[x, y][0]);
                }
                if (isFound)
                {
                    break;
                }
                if (level == maxSearchLevel)
                {
                    break;
                }
                level += 1;
            }

            return result;
        }
    }
}
