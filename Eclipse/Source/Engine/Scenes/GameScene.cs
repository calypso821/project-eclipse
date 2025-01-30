using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Scenes;

namespace Eclipse.Source.Engine.Scenes
{
    //internal abstract class GameScene : Scene
    //{
    //    protected HealthDisplayComponent _healthDisplay;
    //    protected AmmoDisplayComponent _ammoDisplay;

    //    protected override void Initialize()
    //    {
    //        SetupHUD();
    //        SpawnPlayer();
    //        SetupLevel();
    //    }

    //    private void SetupUI()
    //    {
    //        // Health UI
    //        var healthUI = new UIObject("HealthUI");
    //        _healthDisplay = healthUI.AddComponent<HealthDisplayComponent>();

    //        // Ammo UI
    //        var ammoUI = new UIObject("AmmoUI");
    //        _ammoDisplay = ammoUI.AddComponent<AmmoDisplayComponent>();

    //        UIManager.Instance.AddUIObject(healthUI);
    //        UIManager.Instance.AddUIObject(ammoUI);
    //    }


    //    private void SetupHUD()
    //    {
    //        // Common game HUD elements
    //        var healthUI = new UIObject("HealthUI");
    //        _healthDisplay = healthUI.AddComponent<HealthDisplayComponent>();
    //        // ... other HUD elements
    //    }

    //    // Each level implements its specific setup
    //    protected abstract void SetupLevel();
    //}
}
