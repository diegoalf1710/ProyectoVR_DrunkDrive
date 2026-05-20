using UnityEngine;


    public class ItemDispenser : MonoBehaviour
    {
        [Header("Configuracion")]
        public GameObject itemPrefab;         
        public Transform spawnPoint;           

        private bool handInside = false;

        public bool IsHandInside => handInside;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("RightHand"))
                handInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("RightHand"))
                handInside = false;
        }
    }

