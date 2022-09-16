using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // References
    public static PlayerController instance;

    // Here in our case,there will be only only one bomb at a time .So we just instantiate one ,deactivate and save it.Again activate whenever neccessary.
    [SerializeField] private GameObject bombPrefab;
    GameObject bomb;
    Animator anim;

    // Variables
    [SerializeField] private float speed=3f; 
    Vector3 moveInput;
    int count; //Variable for storing length of occupied tiles list
    int x,y;
    bool hasOccupied=false;
    Vector3 targetPos; // next target position of player 
    public int currentTile;  // current tile number in which player is standing
    
    int checkNumber;  // variable for storing the next tile number to which player has to be moved

    bool isFailed=false;
    bool isWalking=false;

    void Awake() 
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance=this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim=GetComponent<Animator>();
        targetPos=transform.position;
        
        count=GameController.instance.occupiedTiles.Count;

        //deactivate the bomb prefab and make this object active and deactivate when ever neccessry
        bomb=Instantiate(bombPrefab);
        bomb.SetActive(false);  

    }

    // Update is called once per frame
    void Update()
    {
        // only accept the input if the player has reached the tile corresponding to previous input otherwise continue moving
        if(Vector3.Distance(transform.position,targetPos)<=0.001f  && !isFailed)
        {
            isWalking=false;
            GetInput();
        }
        else
        {
            MoveToTile();
        }
        anim.SetBool("isWalking",isWalking);
        
    }

    void GetInput()
    {
        // Read the input and check the next position to which moved
        if(Input.GetAxisRaw("Horizontal")!=0 || Input.GetAxisRaw("Vertical")!=0)
        {
            if(Input.GetAxisRaw("Horizontal")!=0)
            {
                moveInput.Set(Input.GetAxisRaw("Horizontal"),0f,0f);
            }
            else if(Input.GetAxisRaw("Vertical")!=0)
            {
                moveInput.Set(0f,0f,Input.GetAxisRaw("Vertical"));
            }
            
            checkDirection();
            if(!hasOccupied)
            {
                // Update the current tile position of player and next target position
                currentTile=checkNumber;
                targetPos+=moveInput;
                isWalking=true;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && !bomb.activeInHierarchy)
        {
            Bomb();
        }
    }

    // Returns a true value if the next tile to which player has to be moved is occupied or outside our grid
    void checkDirection()
    {
        transform.LookAt(transform.position+moveInput);
        hasOccupied=false;
        currentTile=Mathf.RoundToInt((transform.position.x-0.5f)*1f+(14.5f-transform.position.z)*10f);
        Vector3 checkPos=transform.position+moveInput;
        

        // checks whether the input position is outside the grid or occupied by accessing occupied list in GameController script
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

    // Moves the player to next tile with the give speed
    void MoveToTile()
    {
        if(Vector3.Distance(transform.position,targetPos)>0.001f)
        {
            transform.position=Vector3.MoveTowards(transform.position,targetPos,speed*Time.deltaTime);
        }
    }

    // Activates the bomb ,set its position and also updates the tileposition in occupiedList
    void Bomb()
    {
        bomb.transform.position=transform.position;
        GameController.instance.occupiedTiles.Add(currentTile);
        
        bomb.SetActive(true);
        StartCoroutine(SetTimer());
    }


    // When Blasted by bomb..called by bomb script
    public void DestroyByBlast()
    {
        anim.SetTrigger("die");
        StartCoroutine(FailedByBomb());
    }

    IEnumerator FailedByBomb()
    {
        isFailed=true;
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.fail=true;
    }

    // Wait for 3 seconds to blast the bomb
    IEnumerator SetTimer()
    {
        yield return new WaitForSeconds(3f);
        bomb.GetComponent<Bomb>().Blast();
    }

    private void OnTriggerEnter(Collider other) {
       if(other.gameObject.CompareTag("Enemy"))
        {
            UIManager.instance.fail=true;
            anim.SetTrigger("die");
        } 
    }

    // This function gets called from the enemy script on death
    // Updates the ui manager
    public void EnemyDestroyed()
    {
        UIManager.instance.enemiesKilled++;
        UIManager.instance.ScoreUpdate();
    }
    

}
