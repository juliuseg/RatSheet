using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class AgentAttackController : MonoBehaviour
{
    private Transform agentTransform;
    private AgentVelocity agentVelocity;
    private AgentStats agentStats;

    private Coroutine reloadCoroutine;


    public void Setup(Transform _agentTransform, AgentVelocity _agentVelocity, AgentStats _agentStats)
    {
        agentTransform = _agentTransform;
        agentVelocity = _agentVelocity;
        agentStats = _agentStats;
        

    }

    public AttackState HandleAttack(List<AgentControllerBoid> neighbors, int team)
    {
        if (reloadCoroutine != null) return AttackState.reloading;
        AttackState attackState = AttackState.idle;
        Vector2 enemyVelocity = Vector2.zero;
        AgentControllerBoid target = GetTarget(neighbors, team);

        if (target != null && AgentUtils.CanSeeOther(agentTransform.position, target.transform.position))
        {
            enemyVelocity = agentVelocity.GetVelocityFromEnemy(target);
        }

        if (enemyVelocity != Vector2.zero && target != null)
        {
            attackState = EngageTarget(target, enemyVelocity);
        }

        return attackState;
    }

    private AgentControllerBoid GetTarget(List<AgentControllerBoid> neighbors, int team)
    {
        return AgentUtils.GetClosestNeigborOnOtherTeam(neighbors, agentTransform.position, team);
    }

    private AttackState EngageTarget(AgentControllerBoid target, Vector2 enemyVelocity)
    {
        if (!agentVelocity.IsInAttackRange(target))
        {
            agentVelocity.SetVelocity(enemyVelocity);
            return AttackState.movingToAttack;
        }
        else
        {
            agentVelocity.SetVelocity(Vector2.zero);
            reloadCoroutine = StartCoroutine(AttackCooldown(target));
            return AttackState.reloading;
        }
    }

    private IEnumerator AttackCooldown(AgentControllerBoid target) {
        Attack(target, new Attack(agentStats.attack));
        yield return new WaitForSeconds(agentStats.attack.attackCD);
        reloadCoroutine = null;

    }

    private void Attack(AgentControllerBoid target, Attack attack){
        // Make the attack
        target.health.TakeDamage(attack.GetAttackDamage());


    }


    // If the target is in attack range then make the attack
    // Then stay still while cooldown the attack and try again.

    // So the agent is in stages during an attack:
    // 1. Moving to attack (skip if already in attack range)
    // 2. Attack
    // 3. Cooldown
    // 4. Repeat

    // Animation states:
    // 1. Idle: When not moving or cooldown. try to make cooldown same as attack animation
    // 2. Moving: When moving to attack or moving to target
    // 3. Attacking: When attacking


}
