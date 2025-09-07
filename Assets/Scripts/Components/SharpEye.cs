using Data;
using Data.Enums;
using UnityEngine;

namespace Components
{
    public class SharpEye : MonoBehaviour
    {
        private EUnit _unitType;
        private bool _isInitialized;

        public GameObject FindTarget(Vector2 forwardDirection, LayerMask mask, float castDistance = Mathf.Infinity)
        {
            if (!_isInitialized)
                return null;

            var layerMask = mask;

            // Если я нападающий, то здания тоже цель >>> добавляем в маску слой строений
            if (_unitType == EUnit.Enemy)
                layerMask |= SomeConst.BuildingLayerMask;

            var hit = Physics2D.Raycast(transform.position, forwardDirection, castDistance, layerMask);
            return hit.collider ? hit.collider.gameObject : null;
        }

        public void Initialize(EUnit unitType)
        {
            _unitType = unitType;
            _isInitialized = true;
        }
    }
}
