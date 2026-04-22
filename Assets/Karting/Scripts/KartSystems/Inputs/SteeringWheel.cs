using UnityEngine;

namespace KartGame.KartSystems
{
    [RequireComponent(typeof(Rigidbody))]
    public class SteeringWheel : MonoBehaviour
    {
        [Header("Resistencia")]
        public float OneHandDamping = 80f;   // Difícil con una mano
        public float TwoHandDamping = 10f;   // Fluido con dos manos

        // Cuántas manos lo están agarrando
        private int _grabCount = 0;
        private HingeJoint _hinge;
        private Rigidbody _rb;

        // Ángulo actual normalizado para el input (-1 a 1)
        public float NormalizedAngle { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _hinge = GetComponent<HingeJoint>();

            // Configurar el hinge para girar solo en su eje local
            _rb.constraints = RigidbodyConstraints.FreezePosition
                            | RigidbodyConstraints.FreezeRotationX
                            | RigidbodyConstraints.FreezeRotationY;

            ApplyDamping();
        }

        // Llamado por XRGrabInteractable
        public void OnGrab()
        {
            _grabCount = Mathf.Min(_grabCount + 1, 2);
            ApplyDamping();
        }

        public void OnRelease()
        {
            _grabCount = Mathf.Max(_grabCount - 1, 0);
            ApplyDamping();

            // Volver al centro suavemente si nadie lo sostiene
            if (_grabCount == 0)
                StartCoroutine(ReturnToCenter());
        }

        private void ApplyDamping()
        {
            float damping = _grabCount >= 2 ? TwoHandDamping : OneHandDamping;
            _rb.angularDrag = damping;
        }

        private void Update()
        {
            // Convertir rotación Z local a -1..1
            float angle = transform.localEulerAngles.z;
            if (angle > 180f) angle -= 360f;
            NormalizedAngle = Mathf.Clamp(angle / 180f, -1f, 1f);
        }

        private System.Collections.IEnumerator ReturnToCenter()
        {
            float returnSpeed = 90f; // grados por segundo
            while (_grabCount == 0 && Mathf.Abs(transform.localEulerAngles.z) > 1f)
            {
                float angle = transform.localEulerAngles.z;
                if (angle > 180f) angle -= 360f;
                float step = -Mathf.Sign(angle) * returnSpeed * Time.deltaTime;
                transform.Rotate(0f, 0f, step, Space.Self);
                yield return null;
            }
        }
    }
}
