using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour {
    protected Rigidbody2D rb; 

    [SerializeField] protected GameObject selectionCircle; 
    public int team;
    [HideInInspector] public SelectableHPController health;
    protected List<Selectable> neighbors = new List<Selectable>(); 

    [HideInInspector] public SelectableNeighborCollisionHandler neighborCollisionHandler;

    //public virtual void SetSelectable(int _team) { }
    public virtual void SetSelectionCircleActive (int active) { }

    public virtual Abilities abilities { get; protected set; }

    public virtual SelectableStats stats { get; protected set; }

    public virtual Abilities GetAbilities() { 
        print("GetAbilities() called from Selectable");
        return null; 
    }

    public virtual void SetSelectable(int  _team)
    {
        team = _team;

        health = GetComponent<SelectableHPController>();
        health.SetHealthInit(stats);
        health.OnDeath += SelectableDead;

        neighbors = gameObject.AddComponent<SelectableNeighborCollisionHandler>().GetNeighbors();

        rb = GetComponent<Rigidbody2D>();


    }

    public virtual void SelectableDead(){ }
}