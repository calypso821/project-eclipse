using System;

using Microsoft.Xna.Framework;

namespace Eclipse.Engine.Core
{
    internal sealed class Transform
    {
        public GameObject GameObject { get; }
        internal bool IsRegistered { get; set; } = false;

        public Vector2 WorldPosition =>
            new Vector2(
                _worldMatrix.Translation.X,
                _worldMatrix.Translation.Y
            );

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                MarkDirty();
            }
        }

        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set
            {
                SetRotation(value);
                MarkDirty();
            }
        }

        private Vector2 _scale;
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                MarkDirty();
            }
        }

        private Matrix _localMatrix;
        public Matrix LocalMatrix => _localMatrix;

        private Matrix _worldMatrix;
        public Matrix WorldMatrix => _worldMatrix;

        // Shoud be probably moved into CharacterController
        //public Vector2 Direction { get; set; } = Vector2.UnitX;

        internal Transform(GameObject gameObject)
        {
            GameObject = gameObject;

            SetTransform(Vector2.Zero, 0, Vector2.One);
            //_position = Vector2.Zero;
            //_rotation = 0;
            //_scale = Vector2.One;

            // Mark dirty??? 
        }
        internal Transform(GameObject gameObject, Vector2 position, float rotation, Vector2 scale)
        {
            GameObject = gameObject;

            SetTransform(position, rotation, scale);
            //_position = position;
            //_rotation = rotation;
            //_scale = scale;
        }
        internal void SetTransform(Vector2 position, float rotation, Vector2 scale)
        {
            _position = position;
            _rotation = rotation;
            _scale = scale;
            MarkDirty();
        }

        private void MarkDirty()
        {
            // Set DirtyFlag + add to system (dirtyTransfrom)
            GameObject.Register(DirtyFlag.Transform);
        }

        internal void GetLocalTransform(out Vector2 position, out float rotation, out Vector2 scale)
        {
            position = _position;
            rotation = _rotation;
            scale = _scale;
        }

        internal void GetWorldTransform(out Vector2 position, out float rotation, out Vector2 scale)
        {
            _worldMatrix.Decompose(
                out Vector3 scale3,
                out Quaternion rotationQuat,
                out Vector3 translation3
            );

            position = new Vector2(translation3.X, translation3.Y);
            rotation = (float)Math.Atan2(
                2.0f * (rotationQuat.W * rotationQuat.Z),
                1.0f - 2.0f * (rotationQuat.Z * rotationQuat.Z)
            );
            scale = new Vector2(scale3.X, scale3.Y);
        }

        private void UpdateLocalMatrix()
        {
            // Order matters! Scale -> Rotate -> Translate
            // 3x3 matrix - homogenus coordainates
            _localMatrix = Matrix.CreateScale(new Vector3(_scale.X, _scale.Y, 1)) *
                          Matrix.CreateRotationZ(_rotation) *
                          Matrix.CreateTranslation(new Vector3(_position.X, _position.Y, 0));
        }

        internal void UpdateWorldTransform()  // Public interface
        {
            // object from dirtyTransfrom
            if (GameObject.HasDirtyFlag(DirtyFlag.Transform))
            {
                UpdateLocalMatrix();
                GameObject.ClearDirtyFlag(DirtyFlag.Transform);
            }

            // Object need only world matrix update
            UpdateWorldMatrix();

            foreach (var child in GameObject.Children)
            {
                child.Transform.UpdateWorldTransform();
            }
        }
        private void UpdateWorldMatrix()
        {
            // Get parent matrix else identity (root)
            var parentMatrix = GameObject.Parent?.Transform.WorldMatrix ?? Matrix.Identity;
            _worldMatrix = _localMatrix * parentMatrix;
        }

        // Direct matrix operations
        internal void Translate(Vector2 offset)
        {
            _position += offset;
            MarkDirty();
        }

        internal void Rotate(float angle)
        {
            SetRotation(_rotation + angle);
            MarkDirty();
        }
        internal void SetRotation(float angle)
        {
            _rotation = angle % (2 * MathF.PI);
            if (_rotation < 0)
                _rotation += 2 * MathF.PI;
        }

        internal void ScaleBy(Vector2 scale)
        {
            _scale *= scale;
            MarkDirty();
        }
    }
}