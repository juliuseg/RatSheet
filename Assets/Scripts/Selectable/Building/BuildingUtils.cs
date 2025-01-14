using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class BuildingUtils
{
    public static List<BuildingController> GetNeighborsAgents(List<Selectable> neighbors){ 
        return neighbors.Where(neighbor => neighbor is BuildingController).Cast<BuildingController>().ToList();
    }
}