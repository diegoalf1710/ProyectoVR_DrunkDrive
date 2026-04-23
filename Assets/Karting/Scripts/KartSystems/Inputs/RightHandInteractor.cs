using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace KartGame.VR
{
    /// <summary>
    /// Coloca este script en el GameObject de la mano derecha (dentro del XROrigin).
    /// Este GameObject debe tener el tag "RightHand" y un Collider con Is Trigger activado.
    /// </summary>
    public class RightHandInteractor : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform handAnchor;   // El transform donde se sujeta el objeto (este mismo GO o un hijo)

        // Estado
        private InputDevice _rightController;
        private GrabbableItem _heldItem   = null;
        private ItemDispenser _nearDispenser = null;

        private bool _gripLastFrame = false;
        private bool _aButtonLastFrame = false;

        // Para calcular velocidad al soltar
        private Vector3 _prevPosition;
        private Vector3 _handVelocity;

        private void Start()
        {
            if (handAnchor == null) handAnchor = transform;
            _prevPosition = transform.position;
        }

        private void TryGetDevice()
        {
            if (!_rightController.isValid)
            {
                var devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
                    devices);
                if (devices.Count > 0) _rightController = devices[0];
            }
        }

        private void Update()
        {
            TryGetDevice();

            // Calcular velocidad de la mano
            _handVelocity = (transform.position - _prevPosition) / Time.deltaTime;
            _prevPosition = transform.position;

            // Leer inputs
            _rightController.TryGetFeatureValue(CommonUsages.grip, out float grip);
            _rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButton); // Boton A

            bool gripPressed  = grip > 0.7f;
            bool gripDown     = gripPressed && !_gripLastFrame;
            bool gripUp       = !gripPressed && _gripLastFrame;
            bool aButtonDown  = aButton && !_aButtonLastFrame;

            // --- AGARRAR ---
            if (gripDown && _heldItem == null && _nearDispenser != null && _nearDispenser.IsHandInside)
            {
                GrabbableItem spawned = _nearDispenser.SpawnItem(handAnchor);
                if (spawned != null)
                {
                    _heldItem = spawned;
                    _heldItem.Grab(handAnchor);
                }
            }

            // --- SOLTAR ---
            if (gripUp && _heldItem != null)
            {
                _heldItem.Release(_handVelocity);
                _heldItem = null;
            }

            // --- ACTIVAR (boton A): destapar cerveza / encender cigarro ---
            if (aButtonDown && _heldItem != null && !_heldItem.IsActivated)
            {
                _heldItem.Activate();
            }

            _gripLastFrame    = gripPressed;
            _aButtonLastFrame = aButton;
        }

        // Detectar dispensers cercanos
        private void OnTriggerEnter(Collider other)
        {
            ItemDispenser dispenser = other.GetComponent<ItemDispenser>();
            if (dispenser != null)
                _nearDispenser = dispenser;
        }

        private void OnTriggerExit(Collider other)
        {
            ItemDispenser dispenser = other.GetComponent<ItemDispenser>();
            if (dispenser != null && _nearDispenser == dispenser)
                _nearDispenser = null;
        }
    }
}
