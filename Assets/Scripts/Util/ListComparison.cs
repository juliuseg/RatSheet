using System.Collections.Generic;
using System.Linq;

public class ListComparison
{
    // Function to find elements in both lists
    public static List<Selectable> FindInBoth(
        List<Selectable> listA, 
        List<Selectable> listB)
    {
        return listA.Intersect(listB).ToList();
    }

    // Function to find elements in listB but not in listA
    public static List<Selectable> FindInBNotInA(
        List<Selectable> listA, 
        List<Selectable> listB)
    {
        return listB.Except(listA).ToList();
    }
}

