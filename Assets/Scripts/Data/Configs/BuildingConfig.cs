using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Config/Building", order = 1)]
public class BuildingConfig : ScriptableObject
{
    public string id
    {
        get
        {
            return name;
        }
    } 
     
    public int maxHp;
}