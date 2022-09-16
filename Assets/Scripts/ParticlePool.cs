using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    //Static instance is created which helps for easy access of the script when neccessary
    public static ParticlePool instance;
    [SerializeField] private ParticleSystem destructableFX;
    [SerializeField] private ParticleSystem enemyFX;
    [SerializeField] private int numberOfDestFXToPool=10;
    [SerializeField] private int numberOfEnemyFXToPool=10;

    List<ParticleSystem> pooledDestructableFX=new List<ParticleSystem>();
    List<ParticleSystem> pooledEnemyFX=new List<ParticleSystem>();


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
        //effects are instantiated upto given limit and then deactivated and stored in corresponding pool lists
        for(int i=0;i<numberOfDestFXToPool;i++)
        {
            ParticleSystem dFX=Instantiate(destructableFX);
            dFX.gameObject.SetActive(false); 
            pooledDestructableFX.Add(dFX);
        }
        for(int i=0;i<numberOfEnemyFXToPool;i++)
        {
            ParticleSystem eFX=Instantiate(enemyFX);
            eFX.gameObject.SetActive(false); 
            pooledEnemyFX.Add(eFX);
        }
    }

    //Function for returning a destructableEffect form the pool
    public ParticleSystem GetDFX()
    {
        for(int i=0;i<pooledDestructableFX.Count;i++)
        {
            if(!pooledDestructableFX[i].gameObject.activeInHierarchy)
            {
                return pooledDestructableFX[i];
            }
        }
        return null;
    }

    //Function which return an enemy particle effect on calling
    public ParticleSystem GetEFX()
    {
        for(int i=0;i<pooledEnemyFX.Count;i++)
        {
            if(!pooledEnemyFX[i].gameObject.activeInHierarchy)
            {
                return pooledEnemyFX[i];
            }
        }
        return null;
    }
}
