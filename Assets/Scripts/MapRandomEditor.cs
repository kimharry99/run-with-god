using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRandomEditor : MonoBehaviour
{
    struct StructMotion
    {
        public int direction;
        public bool isJump;
        public bool isAttack;

        public StructMotion(int _direction,bool _isAttack,bool _isJump)
        {
            direction = _direction;
            isJump = _isJump;
            isAttack = _isAttack;
        }
    }

    struct StructPos
    {
        public float x, y;

        public StructPos(float _x, float _y)
        {
            x = _x;
            y = _y;
        }
    }

    [Header("Prefab and MapLength")]
    public GameObject ground;
    public GameObject[] enemyArray = new GameObject[3];
    public GameObject blue;
    public GameObject red;
    public int mapLength;

    [Header("Baic Parameters")]
    public int notJumpLimit = 4;
    public int reverseRate = 20;
    public int jumpRate = 50;
    public int attackRate = 95;

    [Header("Upper Right Map Parameters")]
    public float heightUpper = 0.7f;
    public float distanceUpper = 0.8f;
    public float rateUpperPerJump = 0.75f;

    [Header("Just Right Map Parameters")]
    public float distanceJustRight = 3.0f;
    //public float rateJustRight = 0.3f;

    [Header("Lower Right Map Parameters")]
    public float heightLower = 0.7f;
    public float distanceLower = 0.5f;
    public float rateLowerPerFlat = 0.3f;

    private int[] randomDriectionParameter = new int[1000];
    private char[] directionPrintingArray = new char [1000];
    private int[] randomPlayerActionParameter = new int[1000];
    private StructMotion[] playerAction = new StructMotion[1000];
    private StructPos[] playerPos = new StructPos[1000];
    private bool leftSideCount = false;

    // Start is called before the first frame update
    void Start() {
        

        for (int i = 0; i < mapLength - 1; i++)
        {
            randomDriectionParameter[i] = (int)Random.Range(0f, 100f);
            randomPlayerActionParameter[i] = (int)Random.Range(0f, 100f);
        }

        for(int i = 0; i < mapLength - 1; i++)
        {
            StructMotion playerMotionState;
            playerMotionState = MakeNewMotionState(notJumpLimit, reverseRate, jumpRate, attackRate, i);

            playerAction[i] = playerMotionState;
        }

        playerPos[0] = new StructPos(0, 0);

        for (int i = 0; i < mapLength-1; i++)
        {
            StructPos tempPos = playerPos[i];
            tempPos = MakeNextPos(i, tempPos);
            playerPos[i + 1] = tempPos;
        }

        playerAction[mapLength - 1].direction = 1;
        playerAction[mapLength - 1].isJump = false;

        for (int i = 0; i < mapLength; i++)
        {
            if (i > 0)
            {
                if (playerPos[i].x == playerPos[i - 1].x)
                {
                    continue;
                }
            }
            GameObject instance = Instantiate(ground, transform.position +
               new Vector3(playerPos[i].x, playerPos[i].y, 0f), transform.rotation, transform.Find("Map")) as GameObject;
        }

        /*
        for(int i = 0; i < mapLength; i++)
        {
            if (playerAction[i].isJump)
            {
                GameObject jumpMark = Instantiate(red, transform.position +
                    new Vector3(playerPos[i].x + 0.05f, playerPos[i].y + heightUpper / 2, 0f), transform.rotation, transform.Find("Map")) as GameObject;

                if (playerAction[i].direction == -1)
                {
                    GameObject reverseMark = Instantiate(blue, transform.position +
                        new Vector3(playerPos[i].x - 0.05f, playerPos[i].y + heightUpper / 2, 0f), transform.rotation, transform.Find("Map")) as GameObject;
                }
            }
        }
        */

        for(int i = 0; i < mapLength; i++) {
            if (i > 0)
            {
                if (playerPos[i].x == playerPos[i - 1].x)
                {
                    continue;
                }
            }
            if (playerAction[i].isAttack)
            {
                GameObject instanEnemy = Instantiate(enemyArray[(int)Random.Range(0, enemyArray.GetLength(0) - 0.1f)], transform.position +
                    new Vector3(playerPos[i].x, playerPos[i].y + heightUpper/2, 0f), transform.rotation, transform.Find("Monsters")) as GameObject;
            }
        }
        /*
        GameObject tempMap = Instantiate(transform.Find("Map").gameObject,transform.position+new Vector3(0,1000,0),transform.rotation, transform);
        GameObject tempMonsters = Instantiate(transform.Find("Monsters").gameObject,transform.position+new Vector3(0,1000,0),transform.rotation, transform);
        */
    }

    private StructPos MakeNextPos(int i, StructPos tempPos)
    {
        if (playerAction[i].direction == 1)
        {
            
            if (playerAction[i].isJump)
            {
                //오른쪽 대각선 위 점프
                if (randomPlayerActionParameter[i] < jumpRate * rateUpperPerJump)
                {
                    tempPos.x++;
                    tempPos.x += distanceUpper;
                    tempPos.y += heightUpper;
                    leftSideCount = false;
                }
                //오른쪽 평지 점프
                else
                {
                    tempPos.x++;
                    tempPos.x += distanceJustRight;
                    leftSideCount = false;
                }
            }
            else
            {
                //오른쪽 내리막 길
                if (randomPlayerActionParameter[i] < rateLowerPerFlat * (100 - jumpRate) + jumpRate)
                {
                    tempPos.x++;
                    tempPos.x += distanceLower;
                    tempPos.y -= heightLower;
                    leftSideCount = false;
                }
                //오른쪽 직진
                else
                {
                    if(!leftSideCount)
                        tempPos.x++;
                }
            }
        }
        else
        {
            //왼쪽 대각선 위
            if (playerAction[i].isJump)
            {
                tempPos.x--;
                tempPos.x -= distanceUpper;
                tempPos.y += heightUpper;
                leftSideCount = true;
            }
        }
        return tempPos;
    }

    private StructMotion MakeNewMotionState(int notJumpLimit, int reverseRate, int jumpRate, int attackRate, int i)
    {
        StructMotion tempState;
        if (randomDriectionParameter[i] < reverseRate)
            tempState.direction = -1;
        else
            tempState.direction = 1;


        if (randomPlayerActionParameter[i] < jumpRate)
        {
            tempState.isJump = true;
        }
        else
        {
            bool tooFlat=true;
            if (i > notJumpLimit)
            {
                for(int j = i - notJumpLimit; j < i; j++)
                {
                    tooFlat = !playerAction[j].isJump && tooFlat;
                }
            }

            if (tooFlat)
            {
                if (Random.Range(0, jumpRate) < jumpRate * rateUpperPerJump)
                    randomPlayerActionParameter[i] = 0;
                else
                    randomPlayerActionParameter[i] = jumpRate - 1;
                tempState.isJump = true;
            }
            else
            {
                tempState.isJump = false;
            }
        }

        if (randomPlayerActionParameter[i] < attackRate)
        {
            tempState.isAttack = true;
        }
        else
            tempState.isAttack = false;
        return tempState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
