// We start with just this and expand it as we go because its hard to predict what we will need.

using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/Attack", order = 1)]

public class AttackConfig : ScriptableObject{
    [Range(0f, 10f)] public float attackRange = 1.5f;

    [Range(0f, 10f)] public float attackDamage = 1.0f;
    [Range(0f, 2f)] public float attackCD = 1.0f;
}

public class Attack
{
    private float attackRange;
    private float attackDamage;
    private float attackCD;

    public Attack(AttackConfig attackConfig)
    {
        attackRange = attackConfig.attackRange;
        attackDamage = attackConfig.attackDamage;
        attackCD = attackConfig.attackCD;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public float GetAttackCD()
    {
        return attackCD;
    }

}