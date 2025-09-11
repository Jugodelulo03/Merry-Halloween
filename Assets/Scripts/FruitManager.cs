using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
public class FruitManager : MonoBehaviour
{
    //public Text levelCleared;

    public GameObject transition;
    private void Update()
    {
        AllFruitsCollected();
    }
    public void AllFruitsCollected()
    {
        if (transform.childCount == 0)
        {
            //levelCleared.gameObject.SetActive(true);
            transition.SetActive(true);

            Invoke("ChangeScene", 1);
        }
    }

    public void ChangeScene()
    {
        if (transition != null)
        {
            PlayerPrefs.DeleteKey("checkPointPositionX");
            PlayerPrefs.DeleteKey("checkPointPositionY");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
}
