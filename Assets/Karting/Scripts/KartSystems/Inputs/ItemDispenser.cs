using UnityEngine;

namespace KartGame.VR
{
    /// <summary>
    /// Coloca este script en la caja de cervezas o cajetilla de cigarros.
    /// Requiere un Collider con "Is Trigger" activado.
    /// </summary>
    public class ItemDispenser : MonoBehaviour
    {
        [Header("Configuracion")]
        public GameObject itemPrefab;          // Prefab de la cerveza o cigarro
        public Transform spawnPoint;           // Punto donde aparece el objeto (puede ser la mano)

        private bool _handInside = false;

        public bool IsHandInside => _handInside;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("RightHand"))
                _handInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("RightHand"))
                _handInside = false;
        }

        /// <summary>
        /// Spawnea el item y lo retorna para que RightHandInteractor lo agarre.
        /// </summary>
        public GrabbableItem SpawnItem(Transform hand)
        {
            if (itemPrefab == null) return null;

            Vector3 pos = spawnPoint != null ? spawnPoint.position : hand.position;
            GameObject obj = Instantiate(itemPrefab, pos, hand.rotation);

            // Desactivar fisica mientras esta en la mano
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            return obj.GetComponent<GrabbableItem>();
        }
    }
}
