using PostalCodeDistance.NETStandard;
using PostalCodeDistance.NETStandard.Entities;
using PostalCodeDistance.NETStandard.Locations;
using PostalCodeDistance.NETStandard.Readers;
using PostalCodeDistance.NETStandard.Relations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utf8Json;

namespace PostalCodeDistance.NETCore
{
    public class Program
    {
        /**
         * Change Your Path
         * */
        private const string postalCodePath = @"C:\Users\kimit\source\repos\PostalCodeDistance\Data\PostalCodes.csv";
        private const string addressFolder = @"C:\Users\kimit\source\repos\PostalCodeDistance\Data\Address";
        private const string locationPath = @"C:\Users\kimit\source\repos\PostalCodeDistance\Data\Locations.json";
        private const string pointRelationshipPath = @"C:\Users\kimit\source\repos\PostalCodeDistance\Data\PointRelationships.json";
        private const string imagePath = @"C:\Users\kimit\source\repos\PostalCodeDistance\Data\Image.jpg";

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // generateLocations();
            // generateRelations();

            var pointRelationships = JsonSerializer.Deserialize<IReadOnlyList<PointRelationship>>(File.ReadAllText(pointRelationshipPath));
            var nodeConnector = new NodeConnector();

            var sw = new Stopwatch();
            Console.WriteLine("connect start");
            sw.Start();
            var pointNodes = nodeConnector.Connect(pointRelationships);
            sw.Stop();
            Console.WriteLine($"connect stop: {sw.ElapsedMilliseconds}");

            var distanceCalculator = new DistanceCalculator();
            string source = "1520000";
            string target = "2610012";
            Console.WriteLine("calculate start");
            sw.Reset();
            sw.Start();
            Console.WriteLine($"{source}~{target}: {distanceCalculator.CalculateDistance(pointNodes, source, target)}m");
            sw.Stop();
            Console.WriteLine($"calculate stop: {sw.ElapsedMilliseconds}");
        }

        private static void generateLocations()
        {
            var prefectureList = new PrefectureList();
            var municipalityNameTransfer = new MunicipalityNameTransfer();

            var postalCodeFileReader = new PostalCodeFileReader(municipalityNameTransfer, new TownAreaNameTransfer());
            foreach (var postalCode in postalCodeFileReader.ReadFile(postalCodePath))
            {
                prefectureList.Add(postalCode);
            }

            var addressFileReader = new AddressFileReader(municipalityNameTransfer);
            foreach (var address in Directory.GetFiles(addressFolder).SelectMany(x => addressFileReader.ReadFile(Path.Combine(addressFolder, x))))
            {
                prefectureList.Add(address);
            }

            var locationMatcher = new LocationMatcher();
            IReadOnlyList<LocationMatch> foundLocations = locationMatcher.FindLocations(prefectureList);
            var locationCalculator = new LocationCalculator();
            IReadOnlyList<Location> locations = locationCalculator.Calculate(foundLocations);
            File.WriteAllText(locationPath, JsonSerializer.PrettyPrint(JsonSerializer.Serialize(locations)));
            Console.WriteLine("output Locations.json");
        }

        private static void generateRelations()
        {
            var locations = JsonSerializer.Deserialize<IReadOnlyList<Location>>(File.ReadAllText(locationPath));
            var points = locations.Select(x => x.ToPoint()).ToList();
            var relationConnector = new RelationConnector(new BarrierCollection());

            Console.WriteLine("computing PointRelationships...");
            var pointRelationships = relationConnector.ConnectPoints(points);
            File.WriteAllText(pointRelationshipPath, JsonSerializer.PrettyPrint(JsonSerializer.Serialize(pointRelationships)));
            Console.WriteLine("output PointRelationships.json");

            // longitude: 122.987678~145.794707
            // Console.WriteLine($"longitude: {locations.Select(x => x.Longitude).Min()}~{locations.Select(x => x.Longitude).Max()}");
            // latitude: 24.304386538461536~45.512988
            // Console.WriteLine($"latitude: {locations.Select(x => x.Latitude).Min()}~{locations.Select(x => x.Latitude).Max()}");

            Console.WriteLine("render Image.jpg...");
            int scale = 1000;
            int offsetX = 122;
            int offsetY = 24;
            int width = 24 * scale;
            int height = 22 * scale;
            var pen = Pens.Solid(Color.White, 1f);

            using (var image = new Image<Rgba32>(width, height))
            {
                foreach (var pointRelationship in pointRelationships)
                {
                    int x = (int)((pointRelationship.Longitude - offsetX) * scale);
                    int y = (int)((pointRelationship.Latitude - offsetY) * scale);
                    y = Math.Abs(height - y);
                    image[x, y] = Rgba32.White;
                    foreach (var relationPoint in pointRelationship.RelationPoints)
                    {
                        int rx = (int)((relationPoint.Longitude - offsetX) * scale);
                        int ry = (int)((relationPoint.Latitude - offsetY) * scale);
                        ry = Math.Abs(height - ry);
                        image.Mutate(z => z.DrawLines(pen, new PointF(x, y), new PointF(rx, ry)));
                    }
                }
                image.Save(imagePath);
            }
            Console.WriteLine("output Image.jpg");
        }
    }
}
