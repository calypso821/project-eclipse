using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Systems;
using Eclipse.Engine.Systems.Physics;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Audio;

namespace Eclipse.Engine.Managers
{
    internal class SystemEntry
    {
        internal ISystem System { get; }
        internal int Priority { get; }

        internal SystemEntry(ISystem system, int priority)
        {
            System = system;
            Priority = priority;
        }
    }
    internal class DrawableSystemEntry
    {
        internal IDrawableSystem System { get; }
        internal int Priority { get; }

        internal DrawableSystemEntry(IDrawableSystem system, int priority)
        {
            System = system;
            Priority = priority;
        }
    }
    public class SystemManager : Singleton<SystemManager>
    {

        private Dictionary<SystemGroup, List<SystemEntry>> _systemGroups;
        private Dictionary<DirtyFlag, ComponentSystem> _componentSystems;
        private List<DrawableSystemEntry> _drawableSystems;
        internal IReadOnlyDictionary<DirtyFlag, ComponentSystem> ComponentSystems => _componentSystems;

        // Time-based CleanUp 
        private float _cleanupInterval = 3.0f;  // Every 3 second
        private float _cleanupTimer;

        public SystemManager()
        {
            _componentSystems = new Dictionary<DirtyFlag, ComponentSystem>();
            _systemGroups = new Dictionary<SystemGroup, List<SystemEntry>>();
            _drawableSystems = new List<DrawableSystemEntry>();

            _cleanupTimer = _cleanupInterval;

            // Initizle empty systemGroups
            foreach (SystemGroup group in Enum.GetValues(typeof(SystemGroup)))
            {
                _systemGroups[group] = new List<SystemEntry>();
            }
        }

        internal void RegisterSystem(ISystem system, SystemGroup group, int priority)
        {
            var systemEntry = new SystemEntry(system, priority);

            // SystemGroup -> Update()
            _systemGroups[group].Add(systemEntry);
            _systemGroups[group].Sort((a, b) => a.Priority.CompareTo(b.Priority));

            // ComponentSystem -> Register/Unregister internal components
            if (system is ComponentSystem componentSystem)
            {
                // Get DirtyFlag for each System
                DirtyFlag flag = GetSystemFlag(system);

                // Register only system with valid dirtyFlag
                if (flag != DirtyFlag.None)
                {
                    _componentSystems[flag] = componentSystem;
                }
            }
        }

        internal void RegisterDrawableSystem(IDrawableSystem drawableSystem, int priority)
        {
            var drawableEntry = new DrawableSystemEntry(drawableSystem, priority);

            _drawableSystems.Add(drawableEntry);
            _drawableSystems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        private DirtyFlag GetSystemFlag(ISystem system)
        {
            // Map systems to their corresponding flags
            return system switch
            {
                TransformSystem => DirtyFlag.Transform,
                PhysicsSystem => DirtyFlag.Collider | DirtyFlag.RigidBody,
                SpriteRenderer => DirtyFlag.Render,
                ControllerSystem => DirtyFlag.Controller,
                AnimationSystem => DirtyFlag.Animator,
                ProjectileSystem => DirtyFlag.Projectile,
                AbilitySystem => DirtyFlag.Ability,
                AudioSystem => DirtyFlag.Audio,
                VFXSystem => DirtyFlag.VFX,
                DestroySystem => DirtyFlag.Destroy,
                _ => DirtyFlag.None
                //_ => throw new ArgumentException($"Unknown system type: {system.GetType()}")
            };
        }

        internal void Update(GameTime gameTime)
        {
            // Early update phase
            foreach (var systemEntry in _systemGroups[SystemGroup.PreUpdate])
            {
                systemEntry.System.Update(gameTime);
            }

            // Physics update phase
            foreach (var systemEntry in _systemGroups[SystemGroup.PhysicsUpdate])
            {
                systemEntry.System.Update(gameTime);
            }

            // Post update phase
            foreach (var systemEntry in _systemGroups[SystemGroup.PostUpdate])
            {
                systemEntry.System.Update(gameTime);
            }

            _cleanupTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_cleanupTimer > 0) return;

            Cleanup();
            _cleanupTimer = _cleanupInterval;  // Reset timer
        }

        internal void Draw()
        {
            foreach (var drawableEntry in _drawableSystems)
            {
                var drawableSystem = drawableEntry.System;
                drawableSystem.Draw();
            }
        }

        private void Cleanup()
        {
            foreach (var componentSystem in _componentSystems.Values)
            {
                componentSystem.Cleanup();
            }
        }

        internal void Clear()
        {
            _cleanupTimer = 0f;
            foreach (var componentSystem in _componentSystems.Values)
            {
                componentSystem.Clear();
            }
        }
    }
}
