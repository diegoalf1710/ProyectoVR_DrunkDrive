using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace KartGame.KartSystems
{
    public class VRSteeringWheelInput : BaseInput
    {
        [Header("Referencias")]
        public SteeringWheel SteeringWheel;

        [Header("Gatillos")]
        public float AccelerateThreshold = 0.1f;
        public float BrakeThreshold = 0.1f;

        private InputDevice _rightController;
        private InputDevice _leftController;

        private void TryGetDevices()
        {
            if (!_rightController.isValid)
            {
                var list = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, list);
                if (list.Count > 0) _rightController = list[0];
            }
            if (!_leftController.isValid)
            {
                var list = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, list);
                if (list.Count > 0) _leftController = list[0];
            }
        }

        public override InputData GenerateInput()
        {
            TryGetDevices();

            float rightTrigger = 0f;
            _rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);

            float leftTrigger = 0f;
            _leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTrigger);

            float turnInput = SteeringWheel != null ? SteeringWheel.NormalizedAngle : 0f;

            return new InputData
            {
                Accelerate = rightTrigger > AccelerateThreshold,
                Brake      = leftTrigger  > BrakeThreshold,
                TurnInput  = turnInput
            };
        }
    }
}
