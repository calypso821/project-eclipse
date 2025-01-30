using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.Engine;
using Eclipse.Components.Combat;
using Eclipse.Engine.Core;
using Eclipse.Entities.Characters;
using static System.Net.Mime.MediaTypeNames;

namespace Eclipse.Source.Engine.Managers
{
    // Manager for temporary UI elements
    internal class FeedbackManager : Singleton<FeedbackManager>
    {
        // Handles damage numbers, pickups, alerts, etc.
        // Any temporary visual feedback during gameplay

        //private ObjectPool<DamagePopup> _damagePopups;
        //private ObjectPool<NotificationPopup> _notifications;

        //public void ShowDamageNumber(Vector2 position, float damage)
        //{
        //    var popup = _damagePopups.Get();
        //    popup.Show(position, damage);
        //}

        //public void ShowNotification(string text, Vector2 position)
        //{
        //    var notification = _notifications.Get();
        //    notification.Show(text, position);
        //}
    }

    //Common temporary UI elements:

    //Damage numbers
    //Pickup notifications("+ 5 Ammo")
    //Achievement popups
    //Combat feedback("Headshot!")
    //Status effects("Poisoned!")

    //Key differences from permanent UI:

    //Use object pooling for efficiency
    //Have built-in lifetime/fade out
    //Created and destroyed frequently
    //Usually managed by a dedicated system
    //Don't need to persist between scenes


    //Static UI(Created in SetupUI):
    //Health/Stamina bars
    //Ammo counter
    //Minimap
    //Inventory slots
    //Ability cooldown indicators
    //Quest/Objective markers
    //Crosshair
    //Player status effects area
    //"Press E to interact" prompt
    //Currency display
    //Score/Points counter
    //Compass
    //Navigation arrows
    //State indicators("In Combat", "Stealth", "Safe Zone")
    //Weapon wheel/selector
    //Boss health bars(created disabled, enabled when boss spawns)

    //Temporary UI(Created during gameplay):
    //Damage numbers
    //Experience gained popups
    //Item pickup notifications
    //Achievement notifications
    //Combat feedback("Critical!", "Headshot!", "Combo x3!")
    //Tutorial tooltips
    //Status effect notifications("Poisoned!", "Stunned!")
    //Level up effects
    //Countdown timers for temporary effects
    //Enemy spawn indicators
    //Area of effect indicators
    //Projectile trajectory previews
    //Environmental damage warnings
    //Mission updates/alerts
}
