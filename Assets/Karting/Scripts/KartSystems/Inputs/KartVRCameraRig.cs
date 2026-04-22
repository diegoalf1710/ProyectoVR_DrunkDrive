using UnityEngine;

namespace KartGame.KartSystems
{
    /// <summary>
    /// Ancla el XR Origin a un punto del kart (ej: el asiento) sin ser hijo de él.
    /// Coloca este script en el XR Origin. El XR Origin debe estar en la RAÍZ de la escena.
    /// </summary>
    public class KartVRCameraRig : MonoBehaviour
    {
        [Header("Referencia al punto de anclaje en el kart")]
        [Tooltip("Crea un GameObject hijo del kart llamado 'DriverSeat' y asígnalo aquí")]
        public Transform DriverSeatAnchor;

        [Header("Opciones")]
        [Tooltip("Si true, también copia la rotación del kart (recomendado para karts)")]
        public bool FollowRotation = true;

        void LateUpdate()
        {
            if (DriverSeatAnchor == null) return;

            // Seguir posición exacta del asiento
            transform.position = DriverSeatAnchor.position;

            // Seguir rotación del kart (solo Y para no inclinar al jugador)
            if (FollowRotation)
            {
                Vector3 kartEuler = DriverSeatAnchor.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, kartEuler.y, 0f);
            }
        }
    }
}
