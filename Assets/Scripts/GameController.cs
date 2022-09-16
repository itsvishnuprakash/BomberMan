using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // References
    public static GameController instance;
    [SerializeField]private GameObject indestructableBlock;
    [SerializeField]private GameObject destructableBlock;
    [SerializeField]private GameObject enemy;
    [SerializeField]private GameObject player;

    // Variables
    public int rows=15;
    public int coloumns=10;
    float startX=0.5f;
    float startZ=14.5f;
    int placeddestructableCount=0;
    int placedenemyCount=0;
    int destructableBlockCount;
    int tileNumber;
    public int enemyCount=5;
    bool occupied;

    // List for storing the tiles that are assigned with an object
    public List<int> occupiedTiles=new List<int>();
    // Matrix for storing values to each tile
    int[,] tileMatrix;
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
    // Grid is considered as a matrix and objects are allowacated their positions randomly 
    // The corresponding objects are then instantiated in those positions
    // Also added corresponding tilenumbers of objects into a list for controlling the movements
    void Start()
    {
        tileMatrix=new int[rows,coloumns];
        Matrix();
        tileMatrix[0,0]=1;
        occupiedTiles.Add(0);

        destructableBlockCount=Mathf.RoundToInt(Random.Range(30f,50f));
        do
        {
            FindTiles();
        }while(placeddestructableCount<destructableBlockCount || placedenemyCount<enemyCount);
        occupiedTiles.Remove(0);
        occupiedTiles.Remove(1);
        occupiedTiles.Remove(10);

        // Coroutine is used for placing objects in order to get a nice effect
        StartCoroutine(PlaceObjects());
    }

    //This tilematrix is uded for assigning objects in tilemap
    void Matrix()
    {
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<coloumns;j++)
            {
                tileMatrix[i,j]=0;
            }
        }
    }

    // different values corresponding to different objects are given to tile matrix randomly
    void FindTiles()
    {
        tileNumber=0;
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<coloumns;j++)
            {
                occupied=CheckTile(tileNumber);
                if(!occupied)
                {
                    if(i+j==1)
                    {
                        occupiedTiles.Add(tileNumber);
                    }
                    else if(i%2!=0 && j%2!=0)
                    {
                        tileMatrix[i,j]=2;
                        occupiedTiles.Add(tileNumber);
                    }
                    else if(Mathf.RoundToInt(Random.Range(0f,80))%30==0 && placedenemyCount<enemyCount)
                    {
                        tileMatrix[i,j]=4;
                        occupiedTiles.Add(tileNumber);
                        placedenemyCount++;
                    }
                    else if(Mathf.RoundToInt(Random.Range(0f,100))%destructableBlockCount==0 && placeddestructableCount<destructableBlockCount)
                    {
                        tileMatrix[i,j]=3;
                        occupiedTiles.Add(tileNumber);
                        placeddestructableCount++;
                    }
                    
                }
                tileNumber++;

            }
        }
    }

    //Objects are instatiated corresponding to values in the tile matrix
    IEnumerator PlaceObjects()
    {
        float x=startX;
        float y=0f;
        float z=startZ;
        tileNumber=0;
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<coloumns;j++)
            {
                yield return new WaitForSecondsRealtime(0.018f);
                switch(tileMatrix[i,j])
                {
                    case 1:GameObject playerObject= Instantiate(player, new Vector3(x,y,z), Quaternion.identity);
                    break;
                    case 2:GameObject indestructableObject= Instantiate(indestructableBlock, new Vector3(x,y,z), Quaternion.identity);
                    break;
                    case 3:GameObject destructableObject= Instantiate(destructableBlock, new Vector3(x,y,z), Quaternion.identity);
                    break;
                    case 4:GameObject enemyObject=Instantiate(enemy, new Vector3(x,y,z), Quaternion.identity);
                    occupiedTiles.Remove(tileNumber);
                    break;
                    default:break;
                }
                x++;
                tileNumber++;
            }
            z--;
            x=startX;
        }
    }

    //This function returns a true value if the tile number given as argument is currently occupied by any object,else false
    bool CheckTile(int tile)
    {
        foreach(int i in occupiedTiles)
        {
            if(i==tile)
            {
                return true;
            }
        }
        return false;
    }


    private void Update() {
        occupiedTiles.Sort();
        if(Input.GetKeyDown(KeyCode.P))
        {   //for printing occupied tiles on pressing 'p'
            string s= " ";
            for(int i=0;i<occupiedTiles.Count;i++)
            {
                s+=occupiedTiles[i].ToString()+" , ";
            }
            Debug.Log(s);
        }
    }

}
