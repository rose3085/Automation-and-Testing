using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4JTry
{
    class FindPath
    {
        static async Task Main(string[] args)
        {
            await new FindPath().GetPathWithInstructionsAsync();
        }

        public async Task<string> GetPathWithInstructionsAsync()
        {
            Console.WriteLine("Enter start location:");
            string startLocation = Console.ReadLine();
            Console.WriteLine("Enter destination location:");
            string endLocation = Console.ReadLine();
            var uri = "neo4j://127.0.0.1:7687";
            var username = "your-username";
            var password ="your-password";
            using var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
            await using var session = driver.AsyncSession();
            try
            {
                var query =
                    @"
            MATCH (start:Location {name: $start}), (end:Location {name: $end})
            CALL apoc.algo.dijkstra(start, end, 'CONNECTED_TO', 'weight') YIELD path, weight
            RETURN path, weight";

                var result = await session.RunAsync(
                    query,
                    new { start = startLocation, end = endLocation }
                );

                var output = new StringBuilder();
                await result.ForEachAsync(record =>
                {
                    var path = record["path"].As<IPath>();
                    var totalWeight = record["weight"].As<double>();

                    var nodes = path.Nodes;
                    var relationships = path.Relationships;

                    output.AppendLine($"Total path weight: {totalWeight}");

                    for (int i = 0; i < nodes.Count - 1; i++)
                    {
                        var currentNode = nodes[i];
                        var nextNode = nodes[i + 1];
                        var relationship = relationships[i];

                        var instruction = relationship.Properties["instruction"].As<string>();
                        output.AppendLine(
                            $"{currentNode["name"]} -> {instruction} -> {nextNode["name"]}"
                        );
                    }

                    output.AppendLine($"Final Destination: {nodes.Last()["name"]}");
                });

                return output.ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

}
