using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.Engine;
using Eclipse.Engine.Data;

namespace Eclipse.Components.Combat
{
    //internal class GunWeapon : Weapon
    //{
    //    public event Action OnReloadCompleted;
    //    public event Action<int, int> OnAmmoChanged;

    //    // Fields for frequently changing values
    //    internal int _ammoCount;

    //    internal bool _isReloading = false;
    //    private float _reloadTimer = 0f;

    //    internal float _fireTimer = 0f;
    //    internal float _fireInterval;

    //    internal GunWeapon(WeaponData weaponData)
    //        : base(weaponData)
    //    {
    //        _ammoCount = weaponData.MaxAmmo;
    //    }

    //    internal override void OnReset()
    //    {
    //        _ammoCount = WeaponData.MaxAmmo;
    //        OnAmmoChanged?.Invoke(_ammoCount, WeaponData.MaxAmmo);

    //        _fireTimer = 0f;
    //        _isReloading = false;
    //        _reloadTimer = 0f;
    //    }

    //    internal override void Update(GameTime gameTime)
    //    {
    //        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    //        // Update timers
    //        if (_fireTimer > 0)
    //        {
    //            _fireTimer -= dt;
    //        }


    //        // Handle reloading timer
    //        if (_isReloading)
    //        {
    //            _reloadTimer -= dt;
    //            if (_reloadTimer <= 0)
    //            {
    //                CompleteReload();
    //            }
    //        }

    //        // WeaponData.ReloadDuration)
    //        //UpdateSpread(gameTime);
    //    }
    //    internal (int currentAmmo, int maxAmmo) GetAmmoStatus()
    //    {
    //        return (_ammoCount, WeaponData.MaxAmmo);
    //    }
    //    internal void UpdateAmmoStatus()
    //    {
    //        OnAmmoChanged?.Invoke(_ammoCount, WeaponData.MaxAmmo);
    //    }

    //    private void CompleteReload()
    //    {
    //        _ammoCount = WeaponData.MaxAmmo;
    //        _isReloading = false;

    //        // Reset fire timer (can shoot)
    //        _fireTimer = 0;

    //        OnAmmoChanged?.Invoke(_ammoCount, WeaponData.MaxAmmo);
    //        OnReloadCompleted?.Invoke();
    //    }

    //    internal bool TryReload()
    //    {
    //        if (_isReloading || _ammoCount == WeaponData.MaxAmmo) return false;

    //        _isReloading = true;
    //        _reloadTimer = WeaponData.ReloadDuration;

    //        // TODO
    //        // Reloading state -> sprite ReloadingSpriteId
    //        // Create in WeaponFactory
    //        // Add SpriteAnimtor (idle, reloading)

    //        if (_audioSource != null && !string.IsNullOrEmpty(WeaponData.ReloadAudioId))
    //        {
    //            _audioSource.Play(WeaponData.ReloadAudioId);
    //        }

    //        return true;
    //    }
        //private void UpdateSpread(GameTime gameTime)
        //{
        //    if (CurrentSpread > (IsAiming ? Data.AimSpread : Data.Spread))
        //    {
        //        CurrentSpread = Mathf.Lerp(
        //            CurrentSpread,
        //            IsAiming ? Data.AimSpread : Data.Spread,
        //            Data.SpreadRecoveryRate * (float)gameTime.ElapsedGameTime.Seconds
        //        );
        //    }
        //}

        //internal void ToggleAim()
        //{
        //    IsAiming = !IsAiming;
        //    CurrentSpread = IsAiming ? Data.AimSpread : Data.Spread;
        //}
    //}
}
