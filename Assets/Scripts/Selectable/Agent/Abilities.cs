using UnityEngine;
using System.Collections.Generic;

public abstract class Abilities{
    public List<string> abilityNames = new List<string>();

    public void SetAbilities(List<string> _abilityNames){
        abilityNames = _abilityNames;
    }
    
}

public class AgentAbilities : Abilities{
    
}

public class BuildingAbilities : Abilities{
    
}