using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    int currentTile;
    ParticleSystem dFX;


    //This function gets called by the Bomb 
    // Activate a particle effect from the pooledList and calculate the value of current tile using position and removes that from occupiedList
    // So that from now on,it will not affect movement through that tile
    public void DestroyByBlast()
    {
        currentTile=Mathf.RoundToInt((transform.position.x-0.5f)*1f+(14.5f-transform.position.z)*10f);
        GameController.instance.occupiedTiles.Remove(currentTile);

        dFX=ParticlePool.instance.GetDFX();
        dFX.transform.position=transform.position;
        dFX.gameObject.SetActive(true);
        StartCoroutine(DestroyThis());
    }
    // This coroutine wait for the particle system to complete and after make it deactive and it can be called again from the pool
    // Also destroy this gameobject after this 
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(dFX.main.duration);
        dFX.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
