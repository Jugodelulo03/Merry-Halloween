using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class PlayerRespawn : MonoBehaviour
{
    private float checkPointPositionX,checkPointPositionY ;

    public Animator animator;

    public AudioSource muerte;
    void Start()
    {
        /*
        if (PlayerPrefs.GetFloat("checkPointPositionX")!=0)
        {
            transform.position = (new Vector2(PlayerPrefs.GetFloat("checkPointPositionX"), PlayerPrefs.GetFloat("checkPointPositionY")));
        }*/
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
        muerte.Play();
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
