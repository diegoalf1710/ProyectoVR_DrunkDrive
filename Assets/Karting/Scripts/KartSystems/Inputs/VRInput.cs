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

        private InputDevice _leftController;

        private void TryGetDevices()
        {
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

            
            float trigger = 0f;
            _leftController.TryGetFeatureValue(CommonUsages.trigger, out trigger);

            
            float grip = 0f;
            _leftController.TryGetFeatureValue(CommonUsages.grip, out grip);

       
            Vector2 leftStick = Vector2.zero;
            _leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);

            float turnInput = Mathf.Abs(leftStick.x) > TurnDeadzone ? leftStick.x : 0f;

            return new InputData
            {
                Accelerate = trigger > AccelerateThreshold,
                Brake = grip > BrakeThreshold,
                TurnInput = turnInput
            };
        }
    }
}