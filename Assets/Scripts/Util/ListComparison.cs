using System.Collections.Generic;
using System.Linq;

public class ListComparison
{
    // Function to find elements in both lists
    public static List<AgentControllerBoid> FindInBoth(
        List<AgentControllerBoid> listA, 
        List<AgentControllerBoid> listB)
    {
        return listA.Intersect(listB).ToList();
    }

    // Function to find elements in listB but not in listA
    public static List<AgentControllerBoid> FindInBNotInA(
        List<AgentControllerBoid> listA, 
        List<AgentControllerBoid> listB)
    {
        return listB.Except(listA).ToList();
    }
}

