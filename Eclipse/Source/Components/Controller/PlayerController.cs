using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Eclipse.Engine.Managers;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Controller
{
    internal sealed class PlayerController : Controller
    {
        internal sealed override PassOrder PassOrder => PassOrder.Default;

        private readonly InputManager _inputManager;

        private CharacterController _characterController;
        private WeaponController _weaponController;

        internal PlayerController()
            : base()
        {
            _inputManager = InputManager.Instance;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            // Required component validation 
            _characterController = GameObject.GetComponent<CharacterController>() ??
                throw new ArgumentException("PlayerController requires CharacterController component");

            // Optional compoennt for animations
            _weaponController = GameObject.GetComponent<WeaponController>();
        }

        internal override void Update(GameTime gameTime)
        {
            HandleMovementInput();
            HandleWeaponInput();
            HandleActionInput();
        }
        private void HandleMovementInput()
        {
            // Basic movement
            var newDirection = _inputManager.GetMovementDirection();
            _characterController.Move(newDirection);

            // Jump input
            if (_inputManager.IsKeyDown(Keys.Space))
            {
                _characterController.Jump();
            }

            // Dash input
            if (_inputManager.IsKeyDown(Keys.LeftShift))
            {
                _characterController.Dash(newDirection);
            }
        }

        private void HandleWeaponInput()
        {
            if (_weaponController == null) return;

            // in Pixels!!!
            //Vector2 mouseRaw = _inputManager.GetMousePosition().ToVector2(); 

            // MousePosition * Camera.Position -> get Local position
            Vector2 mousePosition = _inputManager.WorldMousePosition; // In Units

            _weaponController.UpdateAimDirection(mousePosition);

            //if (_inputManager.GetLeftMouseDown())
            //{
            //    _weaponController.TryShoot();
            //}

            //if (_inputManager.IsLeftMouseHeld())
            //{
            //    _weaponController.TryAttack();
            //}

            if (_inputManager.GetLeftMouseDown())
            {
                // normal attack OR start charging
                _weaponController.StartAttack(); 
            }
            else if (_inputManager.GetLeftMouseUp())
            {
                // Release charge
                _weaponController.EndAttack(); 
            }

            //if (_inputManager.IsKeyDown(Keys.R))
            //{
            //    _weaponController.TryReload();
            //}

            // TODO
            // 1. Held
            //if (_inputManager.IsLeftMouseHeld())
            //    _weaponController.TryShoot();

            // 2. Aim
            // Maybe cursor when aim but slower???
            // Reduced movemnt speed if aiming?? 
            //if (_inputManager.GetRightMouseDown())
            //    _weaponController.ToggleAim();


            // Example: Charged shot
            //if (_inputManager.IsLeftMouseHeld())
            //{
            //    ChargeShot();
            //}
            //if (_inputManager.GetLeftMouseUp())
            //{
            //    ReleaseChargedShot();
            //}
        }

        private void HandleActionInput()
        {
            // Weapon switching
            if (_inputManager.IsKeyDown(Keys.D1))
            {
                _weaponController.SetActiveWeapon(0);
            }
            if (_inputManager.IsKeyDown(Keys.D2))
            {
                _weaponController.SetActiveWeapon(1);
            }
            if (_inputManager.IsKeyDown(Keys.D3))
            {
                _weaponController.SetActiveWeapon(2);
            }
            if (_inputManager.GetKeyDown(Keys.LeftControl))
            //if (_inputManager.GetRightMouseDown())
            {
                _characterController.ToggleColor();
            }



            // Right click actions
            //if (_inputManager.GetRightMouseDown())
            //{
            //    StartAction();
            //}
            //if (_inputManager.IsRightMouseHeld())
            //{
            //    ContinueAction();
            //}
            //if (_inputManager.GetRightMouseUp())
            //{
            //    FinishAction();
            //}
        }

        private void Shoot()
        {
            // Implement single shot logic
        }

        private void ChargeShot()
        {
            // Implement charge shot logic
        }

        private void ReleaseChargedShot()
        {
            // Implement release charged shot logic
        }

        private void StartAction()
        {
            // Implement start of alternative action
        }

        private void ContinueAction()
        {
            // Implement continuous alternative action
        }

        private void FinishAction()
        {
            // Implement finish of alternative action
        }
    }
}
