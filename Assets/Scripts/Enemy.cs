using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //This static enemy count variable is used to check how many enemies are present now,and it get reduced by one when an enemy dies
    static int totalEnemyCount=5;
    ParticleSystem enemyFX;
    bool randomMove=true;

    [SerializeField] private float speed=2f; 
    Vector3 moveInput;
    int count; //Variable for storing length of occupied tiles list
    int x,y;
    bool hasOccupied=false;
    Vector3 targetPos; // next target position of player 
    public int currentTile;  // current tile number in which player is standing
    
    int checkNumber;  // variable for storing the next tile number to which enemy has to be moved
    // Start is called before the first frame update
    void Start()
    {
        targetPos=transform.position;
        
        count=GameController.instance.occupiedTiles.Count;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for the enemy count and if it equals two start move away from player and else move in a random way 
        if(totalEnemyCount==2  && randomMove)
        {
            randomMove=false;
        }

        //Calculate input for movement if enemy reached target pos or else move to that position 
        if(Vector3.Distance(transform.position,targetPos)<=0.001f)
        {
            CalculateInput();
        }
        else
        {
            MoveToTile();
        }

        //Checks for player and detect whether touched the enemy or not
        if(Vector3.Distance(transform.position,PlayerController.instance.transform.position)<0.01f)
        {
            UIManager.instance.fail=true;
        }
    }

    void CalculateInput()
    {
        if(randomMove)
        {
            RandomMovements();
        }
        else
        {
            LongestDistanceMove();
        }
    }

    void RandomMovements()
    {
        int r=Mathf.RoundToInt(Random.Range(0.5f,4.5f));
        switch(r)
        {
        case 1:moveInput=Vector3.forward;
        break;
        case 2:moveInput=Vector3.back;
        break;
        case 3:moveInput=Vector3.right;
        break;
        case 4:moveInput=Vector3.left;
        break;
        default:break;
        }
        checkDirection();
        if(!hasOccupied)
        {
            // Update the current tile position of enemy and next target position
            currentTile=checkNumber;
            targetPos+=moveInput;
            transform.LookAt(targetPos);
        }
    }

    


    void LongestDistanceMove()
    {

        //Checking all the posiible movements and selecting the input that moves the enemy largest distance from player

        Vector3[] moveInputs=new Vector3[4];
        moveInputs[0]=Vector3.forward;
        moveInputs[1]=Vector3.back;
        moveInputs[2]=Vector3.right;
        moveInputs[3]=Vector3.left;
        Vector3 finalPos;
        List<int> indexes=new List<int>();
        List<float> distances=new List<float>();
        Vector3 playerPos=PlayerController.instance.transform.position;
        for(int i=0;i<4;i++)
        {
            moveInput=moveInputs[i];
            checkDirection();
            if(!hasOccupied)
            {
                // Adding the corresponding index and distance of that tile from player to corresponding lists 
                indexes.Add(i);
                finalPos=transform.position+moveInputs[i];
                float dist=Vector3.Distance(finalPos,playerPos);
                distances.Add(dist);
            }

        }
        float largestDistance=0f;
        //finding largest distance
        for(int j=0;j<indexes.Count;j++)
        {
            if(distances[j]>largestDistance)
            {
                largestDistance=distances[j];
            }
        }
        int moveInputIndexofLargestDistance;
        for(int j=0;j<indexes.Count;j++)
        {
            if(distances[j]==largestDistance)
            {
                moveInputIndexofLargestDistance=j;
                // Update the current tile position of enemy and next target position
        
                currentTile=checkNumber;
                targetPos+=moveInputs[moveInputIndexofLargestDistance];
                transform.LookAt(targetPos);
            }
        }
        
    }


    // Moves the enemy to next tile with the give speed
    void MoveToTile()
    {
        if(Vector3.Distance(transform.position,targetPos)>0.001f)
        {
            transform.position=Vector3.MoveTowards(transform.position,targetPos,speed*Time.deltaTime);
        }
    }
    

    //Checking the possibility of input using occupied tiles list
    void checkDirection()
    {
        
        hasOccupied=false;
        currentTile=Mathf.RoundToInt((transform.position.x-0.5f)*1f+(14.5f-transform.position.z)*10f);
        Vector3 checkPos=transform.position+moveInput;
        

        
        if((checkPos.x<0.5 || checkPos.x>9.5) || (checkPos.z<0.5 || checkPos.z>14.5))
        {
            hasOccupied=true;
        }
        else
        {
            checkNumber=Mathf.RoundToInt((checkPos.x-0.5f)*1f+(14.5f-checkPos.z)*10f);
            hasOccupied=GameController.instance.occupiedTiles.Contains(checkNumber);
        }
        
    }


    //This function gets called by the Bomb 
    // Activate a particle effect from the pooledList and calculate the value of current tile using position and removes that from occupiedList
    // So that from now on,it will not affect movement through that tile
    public void DestroyByBlast()
    {
        // The static variable enemyCount get Updated.This will get Reflected in all enemy scripts
        totalEnemyCount--;
        if(totalEnemyCount==0)
        {
            PlayerController.instance.gameObject.GetComponent<Animator>().SetTrigger("win");
        }
        PlayerController.instance.EnemyDestroyed();

        enemyFX=ParticlePool.instance.GetEFX();
        enemyFX.transform.position=transform.position;
        enemyFX.gameObject.SetActive(true);
        StartCoroutine(DestroyThis());
    }

    // This coroutine wait for the particle system to complete and after make it deactive and it can be called again from the pool
    // Also destroy this gameobject after this 
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(enemyFX.main.duration);
        enemyFX.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

}
