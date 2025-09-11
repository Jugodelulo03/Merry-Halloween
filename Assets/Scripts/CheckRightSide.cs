using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRightSide : MonoBehaviour
{
    public static bool RightWall;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Debug.Log("Der");
            RightWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            RightWall = false;
        }
    }
}
