using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SelectableUIInfo", menuName = "UI/SelectableUIInfo", order = 1)]
public class SelectableUIInfo : ScriptableObject {
    public string nameText;
    public string descriptionText;
    public Sprite portrait;
    public Sprite miniPortrait;
    public List<ActionUIInfo> actionsInfos;
    
}