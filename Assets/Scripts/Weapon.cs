using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Data.Configs;
using UnityEngine;

namespace DefaultNamespace
{
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private BulletItem bulletPrefab;

        private WeaponConfig _weaponConfig;
        private BulletConfig _bulletConfig;

        private bool _isInitialized;
        private bool _isReloading;
        private int _currentAmmo;
        private float _fireCooldown;

        public void Initialize(WeaponConfig weaponConfig, BulletConfig bulletConfig)
        {
            _weaponConfig = weaponConfig;
            _bulletConfig = bulletConfig;
            _currentAmmo = weaponConfig.AmmoCount;
            _isInitialized = true;
        }

        public async Task TryFireAsync(Vector3 direction, LayerMask mask, Vector2 forwardDirection)
        {
            if (!_isInitialized || _isReloading || _fireCooldown > 0 || _currentAmmo <= 0)
            {
                if (_currentAmmo <= 0 && !_isReloading)
                    StartCoroutine(ReloadCoroutine());
                return;
            }

            try
            {
                var bulletInstance = await CreateBullet(forwardDirection);

                if (!bulletInstance)
                    return;

                bulletInstance.Launch(_bulletConfig, direction, mask);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, this);
            }

            _currentAmmo--;
            _fireCooldown = _weaponConfig.RateFire;
            StartCoroutine(StartCooldownCoroutine());

            if (_currentAmmo <= 0 && !_isReloading)
                StartCoroutine(ReloadCoroutine());
        }

        private async Task<BulletItem> CreateBullet(Vector2 forwardDirection)
        {
            var bulletInstance = await InstantiateAsync(bulletPrefab, muzzle.position, muzzle.rotation);

            if (bulletInstance.FirstOrDefault() is not { } bulletInst)
                return null;

            RotateBullet(bulletInst, forwardDirection);
            return bulletInst;
        }

        /// <summary>
        /// Поворачиваем снаярд, т.к. по дефолту он направлен вверх
        /// </summary>
        private static void RotateBullet(BulletItem bulletInst, Vector2 forwardDirection)
        {
            var bulletDir = forwardDirection.normalized;
            var angle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg - 90f;
            bulletInst.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private IEnumerator StartCooldownCoroutine()
        {
            yield return new WaitForSeconds(_fireCooldown);
            _fireCooldown = 0;
        }

        private IEnumerator ReloadCoroutine()
        {
            if (_isReloading)
                yield break;

            _isReloading = true;

            yield return new WaitForSeconds(_weaponConfig.Reload);

            _currentAmmo = _weaponConfig.AmmoCount;
            _isReloading = false;
        }

        private void OnValidate()
        {
            if (!muzzle)
                Debug.LogError($"{nameof(muzzle)} is null " + name, this);

            if (!bulletPrefab)
                Debug.LogError($"{nameof(bulletPrefab)} is null " + name, this);
        }
    }
}
