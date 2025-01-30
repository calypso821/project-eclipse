using Microsoft.Xna.Framework;
using System;
using Eclipse.Components.Controller;
using Eclipse.Components.Animation;
using Eclipse.Components.Combat;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems
{
    internal class ProjectileSystem : ComponentSystem<Projectile>
    {
        //internal override void UpdateComponent(Projectile component, GameTime gameTime)
        //{
        //    component.Update(gameTime);
        //}

        //For debugging
        //public override void Update(GameTime gameTime)
        //{
        //    int cnt = 0;
        //    foreach (var component in _components)
        //    {
        //        cnt++;
        //        if (!component.Enabled || !component.GameObject.Active)
        //            continue;

        //        component.Update(gameTime);
        //        Console.WriteLine(component.GameObject.Transform.Position);
        //    }
            
        //}
    }
    internal class AbilitySystem : ComponentSystem<Ability>
    {
        //internal override void UpdateComponent(Animator component, GameTime gameTime)
        //{
        //    component.Update(gameTime);
        //}
    }

    internal class AnimationSystem : ComponentSystem<Animator>
    {
        //internal override void UpdateComponent(Animator component, GameTime gameTime)
        //{
        //    component.Update(gameTime);
        //}
    }
}
