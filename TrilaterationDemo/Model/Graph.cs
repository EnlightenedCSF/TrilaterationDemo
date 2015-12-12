using System.Collections.Generic;
using System.Linq;

namespace TrilaterationDemo.Model
{
    public class Graph
    {
        public List<List<LocationNode>> AdjacencyList { get; private set; }

        public List<LocationNode> RootNodes
        {
            get {
                return AdjacencyList.Select(list => list[0]).ToList();
            }
        } 

        public Graph()
        {
            AdjacencyList = new List<List<LocationNode>>();
        }

        public Graph AddNode(LocationNode node)
        {
            if (node == null)
                return this;
            
            var contains = AdjacencyList.Any(list => list.Contains(node));
            if (contains)
                return this;
            AdjacencyList.Add(new List<LocationNode>{node});
            return this;
        }

        public Graph ConnectNodes(LocationNode a, LocationNode b)
        {
            var containsA = AdjacencyList.Any(list => list.Contains(a));
            var containsB = AdjacencyList.Any(list => list.Contains(b));
            if (!containsA)
            {
                AddNode(a);
            }
            if (!containsB)
            {
                AddNode(b);
            }

            List<LocationNode>[] rootA = {new List<LocationNode>()};
            List<LocationNode>[] rootB = {new List<LocationNode>()};
            foreach (var list in AdjacencyList.Where(list => list[0] == a && rootA[0].Count == 0))
            {
                rootA[0] = list;
            }
            foreach (var list in AdjacencyList.Where(list => list[0] == b && rootB[0].Count == 0))
            {
                rootB[0] = list;
            }

            rootA[0].Add(b);
            rootB[0].Add(a);

            return this;
        }
    }
}
