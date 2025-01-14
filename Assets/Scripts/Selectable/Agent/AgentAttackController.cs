using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class AgentAttackController : MonoBehaviour
{
    private Transform agentTransform;
    private AgentVelocity agentVelocity;
    private AgentStats agentStats;

    private bool attacking;

    private bool justAttacked;

    private Coroutine reloadCoroutine;

    private Rigidbody2D rb;



    public void Setup(Transform _agentTransform, AgentVelocity _agentVelocity, AgentStats _agentStats, Rigidbody2D _rb)
    {
        agentTransform = _agentTransform;
        agentVelocity = _agentVelocity;
        agentStats = _agentStats;
        rb = _rb;


        attacking = false;
    }

    public AttackState HandleAttack(List<Selectable> neighbors, int team, out Vector2 targetVelocity)
    {
        targetVelocity = Vector2.zero;

        // Check if attacking 
        if (attacking) {
            targetVelocity = agentVelocity.SetVelocity(Vector2.zero);
            rb.isKinematic = true;
            if (justAttacked){
                justAttacked = false;
                return AttackState.attacking;
            }
            return AttackState.reloading;
        }
        
        // Default to idle state
        AttackState attackState = AttackState.idle;

        // Get the target if there is one
        Selectable target = GetTarget(neighbors, team);
        //print("target: " + target);

        // Check if the target is in sight
        if (target != null && AgentUtils.CanSeeOther(agentTransform, target.transform))
        {
            // Get velocity to enemy. 
            Vector2 velocityToEnemy = agentVelocity.GetVelocityToEnemy(target);

            if (!agentVelocity.IsInAttackRange(target))
            {
                //print("not in attack range");
                // If we are not in attack range then move to attack
                targetVelocity = agentVelocity.SetVelocity(velocityToEnemy);
                attackState = AttackState.movingToAttack;
            }
            else
            {
                //print("Should attack here?");
                // Otherwise make the attack if we are not reloading
                if (reloadCoroutine == null)
                {
                    reloadCoroutine = StartCoroutine(AttackCooldown(target));
                    attackState = AttackState.attacking;
                } else {
                    targetVelocity = agentVelocity.SetVelocity(Vector2.zero);
                    attackState = AttackState.reloading;
                    // Although we are reloading, we can still move.
                    // But if we are in range, there is nothing to do. 
                }
            }
            
        }


        return attackState;
    }

    private Selectable GetTarget(List<Selectable> neighbors, int team)
    {
        return AgentUtils.GetClosestNeigborOnOtherTeamProritizeUnits(neighbors, agentTransform.position, team);
    }


    private IEnumerator AttackCooldown(Selectable target) {
        attacking = true;
        justAttacked = true; 
        yield return new WaitForSeconds(agentStats.attack.attackLenght/2);

        if (target != null && agentVelocity.IsInAttackRange(target)){
            Attack(target, new Attack(agentStats.attack));
        }
        yield return new WaitForSeconds(agentStats.attack.attackLenght/2);
        attacking = false;
        yield return new WaitForSeconds(agentStats.attack.attackCD - agentStats.attack.attackLenght);
        reloadCoroutine = null;



    }

    private void Attack(Selectable target, Attack attack){
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
