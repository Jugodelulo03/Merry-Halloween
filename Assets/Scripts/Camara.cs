using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform target;            // Objeto que la cámara seguirá (por ejemplo, el jugador)
    public float minX = 0f;             // Límite mínimo en el eje X
    public float maxX = 15.8f;          // Límite máximo en el eje X
    public float smoothTime = 0.8f;     // Tiempo para que la cámara alcance la posición (mayor valor = más retraso)

    private float fixedY;               // Posición fija en el eje Y
    private Vector3 velocity = Vector3.zero;  // Velocidad de la cámara para SmoothDamp

    void Start()
    {

        // Guardamos la posición Y inicial para que permanezca fija
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        // Verifica que tenemos un objeto para seguir
        if (target != null)
        {
            // Calcula la posición objetivo en el eje X, limitada entre minX y maxX
            float targetX = Mathf.Clamp(target.position.x, minX, maxX);
            Vector3 targetPosition = new Vector3(targetX, fixedY, transform.position.z);

            // Desplaza la cámara suavemente hacia la posición objetivo con SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}

