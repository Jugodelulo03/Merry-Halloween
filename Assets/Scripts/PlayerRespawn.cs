using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity; // 👈 para usar FMOD

public class PlayerRespawn : MonoBehaviour
{
    private float checkPointPositionX, checkPointPositionY;

    public Animator animator;

    [Header("FMOD Events")]
    [SerializeField] private EventReference muerteEvent;

    void Start()
    {
        /*
        if (PlayerPrefs.GetFloat("checkPointPositionX") != 0)
        {
            transform.position = (new Vector2(
                PlayerPrefs.GetFloat("checkPointPositionX"),
                PlayerPrefs.GetFloat("checkPointPositionY")));
        }
        */
    }

    public void ReachedCheckpoint(float x, float y)
    {
        PlayerPrefs.SetFloat("checkPointPositionX", x);
        PlayerPrefs.SetFloat("checkPointPositionY", y);
    }

    public void PlayerDamaged()
    {
        if (animator.GetBool("Navidad"))
        {
            animator.Play("Hit 0");
        }
        else
        {
            animator.Play("Hit");
        }

        // 👇 Reemplazo de muerte.Play()
        RuntimeManager.PlayOneShot(muerteEvent, transform.position);

        Invoke("ReloadScene", 0.05f);
    }

    void ReloadScene()
    {
        float x = PlayerPrefs.GetFloat("checkPointPositionX");
        float y = PlayerPrefs.GetFloat("checkPointPositionY");

        transform.position = new Vector2(x, y);

        if (animator.GetBool("Navidad"))
        {
            animator.Play("Idle 0");
        }
        else
        {
            animator.Play("Idle");
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
