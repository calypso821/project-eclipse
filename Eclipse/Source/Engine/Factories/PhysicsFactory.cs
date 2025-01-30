using System;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;
using Eclipse.Engine.Config;
using Eclipse.Engine.Data;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Factories
{
    internal class PhysicsFactory : Singleton<PhysicsFactory>
    {
        internal Collider2D CreateHitboxCollider(HitboxData config)
        {

            var collider = config.ColliderType switch
            {
                ColliderType.Box => new BoxCollider2D(config.Width, config.Height, new Vector2(0, 0)),
                //ColliderType.Circle => new CircleCollider2D(data.Width / 2f, data.Offset),
                _ => throw new ArgumentException($"Unsupported collider type: {config.ColliderType}")
            };

            collider.Layer = CollisionLayer.Hitbox;
            return collider;
        }

        // TODO: RigidBody Data, Config
        //internal static RigidBody2D CreateRigidBody(RigidBodyData data)
        //{
        //    var rigidBody = new RigidBody2D
        //    {
        //        Mass = data.Mass,
        //        IsKinematic = data.IsKinematic,
        //        // Other properties...
        //    };
        //    return rigidBody;
        //}
    }
}
