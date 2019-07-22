using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRandomEditor : MonoBehaviour
{
    struct posAndAction
    {
        public int x, y;
        public string action;
        public bool direction;
        public bool isJump;
        public bool isAttack;

        public posAndAction(int _x, int _y, string _action,bool _direction,bool _isAttack,bool _isJump)
        {
            x = _x;
            y = _y;
            action = _action;
            direction = _direction;
            isJump = _isJump;
            isAttack = _isAttack;
        }
    }
    public GameObject ground;
    public GameObject[] enemy = new GameObject[3];
    public GameObject blue;
    public GameObject red;
    public int mapLength;

    private int[] randomDriectionParameter = new int[100];
    public char[] direction = new char[100];
    private int[] randomPlayerActionParameter = new int[100];
    private posAndAction[] playerCoordinate = new posAndAction[100];
    // Start is called before the first frame update
    void Start() {

        playerCoordinate[0] = new posAndAction(0, 0, "attack",true,true,false);
        int jumpCount = 0;
        for(int i = 0; i < mapLength-1; i++)
        {
            randomDriectionParameter[i] = (int)Random.Range(0f, 20f);
            randomPlayerActionParameter[i] = (int)Random.Range(0f, 20f);

            if (randomDriectionParameter[i] < 2)
            {
                playerCoordinate[i].direction = false;
                playerCoordinate[i + 1].x = playerCoordinate[i].x - 1;
            }
            else
            {
                playerCoordinate[i].direction = true;
                playerCoordinate[i + 1].x = playerCoordinate[i].x + 1;
            }

            if (jumpCount < 4)
            {
                if (randomPlayerActionParameter[i] < 3)
                {
                    playerCoordinate[i].isJump = true;
                    jumpCount = 0;
                    playerCoordinate[i + 1].y = playerCoordinate[i].y + 1;
                }
                else
                {
                    playerCoordinate[i].isJump = false;
                    jumpCount++;
                    playerCoordinate[i + 1].y = playerCoordinate[i].y;
                }
            }
            else
            {
                playerCoordinate[i].isJump = true;
                jumpCount = 0;
                playerCoordinate[i + 1].y = playerCoordinate[i].y + 1;
            }

            if (randomPlayerActionParameter[i] > 19)
            {
                playerCoordinate[i].isAttack = false;
            }
            else
                playerCoordinate[i].isAttack = true;
        }
        playerCoordinate[mapLength - 1].direction = true;
        playerCoordinate[mapLength - 1].isJump = false;

        for(int i = 0; i < mapLength; i++)
        {
            GameObject instance = Instantiate(ground, transform.position+
                new Vector3(playerCoordinate[i].x, 0.8f*playerCoordinate[i].y, 0f),transform.rotation,transform.Find("Map")) as GameObject;
            if (!playerCoordinate[i].direction)
            {
                GameObject reverseMark = Instantiate(blue, transform.position+
                    new Vector3(playerCoordinate[i].x-0.05f, 0.8f * playerCoordinate[i].y+0.5f, 0f), transform.rotation,transform.Find("Map")) as GameObject;
            }
            if (playerCoordinate[i].isJump)
            {
                GameObject jumpMark = Instantiate(red, transform.position+
                    new Vector3(playerCoordinate[i].x + 0.05f, 0.8f * playerCoordinate[i].y + 0.5f, 0f), transform.rotation,transform.Find("Map")) as GameObject;
            }
            if (playerCoordinate[i].isAttack)
            {
                GameObject instanEnemy = Instantiate(enemy[(int)Random.Range(0,enemy.GetLength(0)-0.1f)], transform.position+
                    new Vector3(playerCoordinate[i].x, 0.8f * playerCoordinate[i].y + 0.5f, 0f), transform.rotation,transform.Find("Map")) as GameObject;
            }
        }

        GameObject temp = Instantiate(transform.Find("Map").gameObject,transform.position+new Vector3(0,1000,0),transform.rotation, transform);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
