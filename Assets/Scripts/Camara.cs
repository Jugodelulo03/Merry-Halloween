using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform target;            // Objeto que la c�mara seguir� (por ejemplo, el jugador)
    public float minX = 0f;             // L�mite m�nimo en el eje X
    public float maxX = 15.8f;          // L�mite m�ximo en el eje X
    public float smoothTime = 0.8f;     // Tiempo para que la c�mara alcance la posici�n (mayor valor = m�s retraso)

    private float fixedY;               // Posici�n fija en el eje Y
    private Vector3 velocity = Vector3.zero;  // Velocidad de la c�mara para SmoothDamp

    void Start()
    {

        // Guardamos la posici�n Y inicial para que permanezca fija
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        // Verifica que tenemos un objeto para seguir
        if (target != null)
        {
            // Calcula la posici�n objetivo en el eje X, limitada entre minX y maxX
            float targetX = Mathf.Clamp(target.position.x, minX, maxX);
            Vector3 targetPosition = new Vector3(targetX, fixedY, transform.position.z);

            // Desplaza la c�mara suavemente hacia la posici�n objetivo con SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}

