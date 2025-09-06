using UnityEngine;

namespace Data
{
    public static class SomeConst
    {
        public const int PlayerLayerIndex = 18;
        public const int EnemyLayerIndex = 19;
        public static LayerMask PlayerLayerMask = 1 << PlayerLayerIndex;
        public static LayerMask EnemyLayerMask = 1 << EnemyLayerIndex;
    }
}
