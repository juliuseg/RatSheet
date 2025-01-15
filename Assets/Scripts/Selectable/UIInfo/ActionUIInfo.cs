using UnityEngine;

[CreateAssetMenu(fileName = "new ActionUIInfo", menuName = "UI/ActionUIInfo", order = 1)]
public class ActionUIInfo : ScriptableObject {
    public Sprite icon;
    public string nameText;
    public string descriptionText;
}