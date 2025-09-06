using System.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private BulletItem bullet;
        private WeaponData _weaponData;
        private BulletData _bulletData;
        private bool _isInitialized;
        private bool _isReloading;
        private float _currentAmmo;
        private float _fireCooldown;

        public void Initialize(WeaponData weaponData, BulletData bulletData)
        {
            _weaponData = weaponData;
            _bulletData = bulletData;
            _currentAmmo = weaponData.AmmoCount;
            _isInitialized = true;
        }

        public async void TryFire(Vector3 direction, LayerMask mask, float damage)
        {
            if (!_isInitialized || _isReloading || _fireCooldown > 0 || _currentAmmo <= 0)
            {
                if (_currentAmmo <= 0 && !_isReloading)
                    await Reload();
                return;
            }

            _bulletData.Damage = damage;
            var bulletInstance = Instantiate(bullet, muzzle.position, muzzle.rotation);
            bulletInstance.Fire(_bulletData, direction, mask);

            _currentAmmo--;
            _fireCooldown = _weaponData.RateFire;

            _ = StartCooldown();

            if (_currentAmmo <= 0 && !_isReloading)
                await Reload();
        }

        private async Task StartCooldown()
        {
            while (_fireCooldown > 0)
            {
                _fireCooldown -= Time.deltaTime;
                await Task.Yield();
            }
        }

        private async Task Reload()
        {
            _isReloading = true;
            await Task.Delay((int)(_weaponData.Reload * 1000));
            _currentAmmo = _weaponData.AmmoCount;
            _isReloading = false;
        }

        private void OnValidate()
        {
            if (!muzzle)
                Debug.LogError($"{nameof(muzzle)} is null " + name, this);

            if (!bullet)
                Debug.LogError($"{nameof(bullet)} is null " + name, this);
        }
    }
}
