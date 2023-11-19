using Apos.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Inputs.Interfaces;

namespace TheLostHopeEngine.EngineCode.Inputs
{
    public class InputBinding
    {
        public bool Editable { get; set; }
        public Keys KeyboadKey { get; set; }
        public GamePadButton GamepadButton { get; set; }
    }

    public static class InputBindingManager
    {

        private static Dictionary<string, InputBinding> _bindings;
        private static List<IInputBindingUser> _users;


        public static void Initialize(InputAsset inputAsset)
        {
            _users = new List<IInputBindingUser>();

            _bindings = new Dictionary<string, InputBinding>
            {
                {
                    "PlayerMoveRight",
                    new InputBinding
                    {
                        Editable = false,
                        KeyboadKey = inputAsset.K_PlayerMoveRightInput,
                        GamepadButton = inputAsset.G_PlayerMoveRightInput
                    }
                },
                {
                    "PlayerMoveLeft",
                    new InputBinding
                    {
                        Editable = false,
                        KeyboadKey = inputAsset.K_PlayerMoveLeftInput,
                        GamepadButton = inputAsset.G_PlayerMoveLeftInput
                    }
                },
                {
                    "PlayerJump",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_PlayerJumpInput,
                        GamepadButton = inputAsset.G_PlayerJumpInput
                    }
                },
                {
                    "PlayerRoll",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_PlayerRollInput,
                        GamepadButton = inputAsset.G_PlayerRollInput
                    }
                },
                {
                    "PlayerParry",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_PlayerParryInput,
                        GamepadButton = inputAsset.G_PlayerParryInput
                    }
                },
                {
                    "PlayerInteract",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_PlayerInteractInput,
                        GamepadButton = inputAsset.G_PlayerInteractInput
                    }
                },
                {
                    "GunShoot",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunShootInput,
                        GamepadButton = inputAsset.G_GunShootInput
                    }
                },
                {
                    "GunInitializeReload",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunInitializeReloadInput,
                        GamepadButton = inputAsset.G_GunInitializeReloadInput
                    }
                },
                {
                    "GunReloadUp",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunReloadUpInput,
                        GamepadButton = inputAsset.G_GunReloadUpInput
                    }
                },
                {
                    "GunReloadDown",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunReloadDownInput,
                        GamepadButton = inputAsset.G_GunReloadDownInput
                    }
                },
                {
                    "GunReloadLeft",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunReloadLeftInput,
                        GamepadButton = inputAsset.G_GunReloadLeftInput
                    }
                },
                {
                    "GunReloadRight",
                    new InputBinding
                    {
                        Editable = true,
                        KeyboadKey = inputAsset.K_GunReloadRightInput,
                        GamepadButton = inputAsset.G_GunReloadRightInput
                    }
                },
            };
        }


        public static void RegisterUser(IInputBindingUser user)
        {
            if (user != null && _users.Contains(user))
            {
                _users.Add(user);
            }
        }

        public static InputBinding GetInputBinding(string id)
        {
            if (_bindings.TryGetValue(id, out var binding))
            {
                return binding;
            }

            return null;
        }
        public static void UpdateInputBinding(string id, InputBinding newInputBinding)
        {
            if (newInputBinding != null)
            {
                _bindings[id] = newInputBinding;

                foreach (var user in _users)
                {
                    user.SetupInputBindings();
                }
            }
        }
    }
}
