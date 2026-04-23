using UnityEngine;

namespace KartGame.VR
{
    public enum ItemType { Beer, Cigarette }

    /// <summary>
    /// Coloca este script en el prefab de la cerveza o cigarro.
    /// Requiere: Rigidbody + Collider.
    /// </summary>
    public class GrabbableItem : MonoBehaviour
    {
        [Header("Tipo de objeto")]
        public ItemType itemType;

        [Header("Activacion")]
        public GameObject sealedVisual;      // Visual tapado (tapa, cigarro sin encender)
        public GameObject activatedVisual;   // Visual destapado / encendido
        public ParticleSystem activatedFX;   // Humo del cigarro o burbuja de cerveza

        [Header("Uso (proximidad a la cara)")]
        public float useDistance = 0.15f;    // Distancia a la cabeza para "tomar/fumar"
        public float useInterval = 1f;       // Segundos entre cada "sorbo/calada"

        // Estado
        public bool IsActivated { get; private set; } = false;
        public bool IsHeld { get; private set; } = false;

        private Transform _cameraTransform;
        private Rigidbody _rb;
        private Transform _handAnchor;
        private float _useTimer = 0f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _cameraTransform = Camera.main?.transform;

            // Estado inicial: mostrar visual sellado
            if (sealedVisual) sealedVisual.SetActive(true);
            if (activatedVisual) activatedVisual.SetActive(false);
            if (activatedFX) activatedFX.Stop();
        }

        private void Update()
        {
            if (!IsHeld || !IsActivated) return;
            if (_cameraTransform == null) return;

            // Seguir a la mano
            transform.position = _handAnchor.position;
            transform.rotation = _handAnchor.rotation;

            // Detectar proximidad a la cara
            float dist = Vector3.Distance(transform.position, _cameraTransform.position);
            if (dist <= useDistance)
            {
                _useTimer += Time.deltaTime;
                if (_useTimer >= useInterval)
                {
                    _useTimer = 0f;
                    OnUsed();
                }
            }
            else
            {
                _useTimer = 0f;
            }
        }

        /// <summary>
        /// Activa el objeto: destapa cerveza o enciende cigarro.
        /// </summary>
        public void Activate()
        {
            if (IsActivated) return;
            IsActivated = true;

            if (sealedVisual) sealedVisual.SetActive(false);
            if (activatedVisual) activatedVisual.SetActive(true);
            if (activatedFX) activatedFX.Play();

            Debug.Log($"[GrabbableItem] {itemType} activado!");
        }

        /// <summary>
        /// Llama a esto desde RightHandInteractor cuando el grip se presiona.
        /// </summary>
        public void Grab(Transform handAnchor)
        {
            IsHeld = true;
            _handAnchor = handAnchor;
            _rb.isKinematic = true;
            transform.SetParent(handAnchor);
        }

        /// <summary>
        /// Llama a esto desde RightHandInteractor cuando el grip se suelta.
        /// </summary>
        public void Release(Vector3 velocity)
        {
            IsHeld = false;
            _handAnchor = null;
            transform.SetParent(null);
            _rb.isKinematic = false;
            _rb.velocity = velocity;  // Hereda velocidad de la mano al soltar

            if (activatedFX) activatedFX.Stop();
        }

        private void OnUsed()
        {
            // Aqui conectaras despues los efectos de borrachera / stats
            Debug.Log($"[GrabbableItem] Usando {itemType} cerca de la cara!");
        }
    }
}