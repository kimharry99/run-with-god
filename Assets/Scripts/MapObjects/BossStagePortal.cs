using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BossStagePortal : MonoBehaviour
{

    //portal근처에서는 카메라가 플레이어를 추적하지 않는다. 카메라의 제한 범위를 조절하는 변수.
    public bool isPortal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPortal)
        {
            if (collision.tag == "Player")
            {
                if (GameManager.inst.SelectedTrust.trustType == TrustType.ACTION)
                    //Debug.Log("To Boss1");
                    SceneManager.LoadScene("Boss_Butcher");
                else if (GameManager.inst.SelectedTrust.trustType == TrustType.ATTACK)
                    //Debug.Log("To Boss2");
                    SceneManager.LoadScene("Boss_Vampire");
                else
                    //Debug.Log("To Boss3");
                    SceneManager.LoadScene("Boss");
                //SceneManager.LoadScene("InGameScene");
            }
        }
        else
        {
            if (collision.tag == "Player")
            {
                Camera.main.GetComponent<CameraController>().isFollowingPlayer = false;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isPortal)
        {
            Camera.main.GetComponent<CameraController>().isFollowingPlayer = true;
        }
    }
}
