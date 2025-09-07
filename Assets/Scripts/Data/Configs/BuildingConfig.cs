using UnityEngine;

namespace Data.Configs
{
    [CreateAssetMenu(
        fileName = nameof(BuildingConfig),
        menuName = "Config/" + nameof(BuildingConfig)
    )]
    public class BuildingConfig : ScriptableObject
    {
        public string id
        {
            get { return name; }
        }

        public int maxHp;
    }
}
