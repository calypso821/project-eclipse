
using Eclipse.Components.Controller;
using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    // TODO: Component -> UIComponent
    internal class AmmoDisplay : UIWidget
    {
        private UIText _ammoCount;
        private WeaponController _weaponController;

        internal AmmoDisplay(UIText ammoCount)
        {
            _ammoCount = ammoCount;
        }

        internal void Configure(WeaponController weaponController)
        {
            _weaponController = weaponController;
            //var ammoStatus = weaponController.GetAmmoStatus();
            //UpdateAmmoDisplay(ammoStatus.currentAmmo, ammoStatus.maxAmmo);

            // Subscribe to weapon switches
            //_weaponController.OnAmmoChanged += UpdateAmmoDisplay;
        }

        private void UpdateAmmoDisplay(int currentAmmo, int totalAmmo)
        {
            _ammoCount.Text = $"{currentAmmo}/{totalAmmo}";
        }

        internal override void OnDestroy()
        {
            if (_weaponController != null)
            {
                //_weaponController.OnAmmoChanged -= UpdateAmmoDisplay;
            }
            base.OnDestroy();
        }
    }
}
