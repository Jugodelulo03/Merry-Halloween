using UnityEngine;
using UnityEngine.EventSystems;

public class AutoSelectButton : MonoBehaviour
{
    [Header("Botón a seleccionar automáticamente")]
    public GameObject firstSelected;

    void OnEnable()
    {
        // Limpia selección previa
        EventSystem.current.SetSelectedGameObject(null);

        // Selecciona el botón inicial
        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}
