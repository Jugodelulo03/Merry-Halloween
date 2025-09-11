using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLeftSide : MonoBehaviour
{
    public static bool LeftWall;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Debug.Log("izq");
            LeftWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            LeftWall = false;
        }
    }
}
