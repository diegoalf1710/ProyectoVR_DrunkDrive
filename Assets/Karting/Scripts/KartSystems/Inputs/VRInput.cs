using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace KartGame.KartSystems
{
    public class VRInput : BaseInput
    {
        [Header("VR Thresholds")]
        public float AccelerateThreshold = 0.1f;
        public float BrakeThreshold = 0.1f;
        public float TurnDeadzone = 0.1f;

        // Devices cacheados
        private InputDevice _rightController;
        private InputDevice _leftController;

        private void TryGetDevices()
        {
            if (!_rightController.isValid)
            {
                var devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                    devices);
                if (devices.Count > 0) _rightController = devices[0];
            }

            if (!_leftController.isValid)
            {
                var devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
                    devices);
                if (devices.Count > 0) _leftController = devices[0];
            }
        }

        public override InputData GenerateInput()
        {
            TryGetDevices();

            // --- Acelerar: gatillo derecho ---
            float rightTrigger = 0f;
            _rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);

            // --- Frenar: gatillo izquierdo ---
            float leftTrigger = 0f;
            _leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTrigger);

            // --- Girar: joystick izquierdo eje X ---
            Vector2 leftStick = Vector2.zero;
            _leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);

            float turnInput = Mathf.Abs(leftStick.x) > TurnDeadzone ? leftStick.x : 0f;

            return new InputData
            {
                Accelerate = rightTrigger > AccelerateThreshold,
                Brake      = leftTrigger  > BrakeThreshold,
                TurnInput  = turnInput
            };
        }
    }
}
