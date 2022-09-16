using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int currentTile;
    GameObject[] objectsInTiles;
    [SerializeField] private LayerMask destroyable;
    
    
    //Called by player and makes near by objects deactive and remove current tile from occupied tiles
    public void Blast()
    {
        DestroyNearObjects();
        transform.gameObject.SetActive(false);

        currentTile=Mathf.RoundToInt((transform.position.x-0.5f)*1f+(14.5f-transform.position.z)*10f);
        GameController.instance.occupiedTiles.Remove(currentTile);
    }

    void DestroyNearObjects()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale, Quaternion.identity, destroyable);
        int i = 0;
        //Check when there is a new collider coming into contact with the box,loops through all of them and compare their tags to detect whether they are enemy or destructable block
        //And in that case access their corresponding script and call the destroying function
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].gameObject.CompareTag("Player"))
            {
                hitColliders[i].gameObject.GetComponent<PlayerController>().DestroyByBlast();
            }
            else if(hitColliders[i].gameObject.CompareTag("DestroyableBlock"))
            {
                hitColliders[i].gameObject.GetComponent<DestructableObject>().DestroyByBlast();
            }
            else if(hitColliders[i].gameObject.CompareTag("Enemy"))
            {
                hitColliders[i].gameObject.GetComponent<Enemy>().DestroyByBlast();
            }
            i++;
        }

    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (true)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale*2f);
    }
}
