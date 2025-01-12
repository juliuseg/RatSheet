
public enum HitType
{
    Direct, // Single target
    AoE, // Area of effect
    DirectThenAoE, // Single target then AoE
    Self, // Self effect
}

public enum UnitTypes
{
    Light,
    Armored, 
    Shadow, // Pure shadow. No physical form. Zombies are not shadow even though they are made by it.
    Biological, // Rats and such. I think all shadows should have bonus damage towards biological generally. 
    Magic, // Lots of abilities
    
} 
// We could also have dirrect imunities: like you have to give magic damage to kill a shadow or something. Like bloons. Simple but fun in small game!