using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D) , typeof(SelectableHPController)), RequireComponent(typeof(BuildingProduction))]
public class BuildingController : Selectable
{
    private BuildingAppearance buildingAppearance;

    [HideInInspector] public BuildingProduction buildingProduction;

    [SerializeField] private BuildingStats buildingStats;

    public override SelectableStats stats => buildingStats;

    private BuildingAbilities buildingAabilities;
    public override Abilities abilities => buildingAabilities;


    private void Start() {
        SetSelectable(team);
        buildingProduction = GetComponent<BuildingProduction>();
        buildingProduction.SetupBuildingProduction(buildingStats, team);

        buildingAabilities = new BuildingAbilities();

        buildingAabilities.SetAbilities(buildingStats.spawnObjects.Select(spawnObject => spawnObject.name).ToList());

        neighbors = gameObject.AddComponent<SelectableNeighborCollisionHandler>().GetNeighbors();

    }


    public override void SelectableDead(){
        Destroy(gameObject);
    }

    public void AddToProduction(int i){
        buildingProduction.AddToProduction(i);
    }

    public override void SetSelectable(int  _team)
    {
        base.SetSelectable(_team);

        buildingAppearance = new BuildingAppearance(selectionCircle, GetComponent<SpriteRenderer>(), team);
        

    }

    public override void SetSelectionCircleActive(int active)
    {
        buildingAppearance.SetSelectionCircleActive(active);
    }

    private void Update() {
        buildingProduction.UpdateQueue();
    }

    public override Abilities GetAbilities(){
        return abilities;
    }

}