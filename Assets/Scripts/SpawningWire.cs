using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningWire : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Wire")]
    [SerializeField] GameObject wireFence;

    private GameObject lastFenceLeft = null;
    private GameObject lastFenceRight = null;
    private List<GameObject> Fences = new List<GameObject>();

    public void spawnWire(int direction)
    {
            if (direction == -1 && Fences.Count < 10)
            {
                if (lastFenceLeft == null)
                    lastFenceLeft = Instantiate(Resources.Load<GameObject>("Fence"), new Vector3(player.transform.position.x , -2, player.transform.position.z + 3), Quaternion.identity) as GameObject;
                else
                {
                    Fences.Add(lastFenceLeft);
                    lastFenceLeft = Instantiate(Resources.Load<GameObject>("Fence"), new Vector3(lastFenceLeft.transform.position.x + 11.7f, -2, player.transform.position.z + 3), Quaternion.identity) as GameObject;
                }
                lastFenceLeft.transform.localScale = new Vector3(4, 14, 13);
                lastFenceLeft.transform.localEulerAngles = Vector3.zero;
                lastFenceLeft.transform.parent = wireFence.transform;
            }
            else if (direction == 1 && Fences.Count < 10)
            {
                if (lastFenceRight == null)
                    lastFenceRight = Instantiate(Resources.Load<GameObject>("Fence"), new Vector3(player.transform.position.x, -2, player.transform.position.z - 4), Quaternion.identity) as GameObject;
                else
                {
                    Fences.Add(lastFenceRight);
                    lastFenceRight = Instantiate(Resources.Load<GameObject>("Fence"), new Vector3(lastFenceRight.transform.position.x + 11.7f, -2, player.transform.position.z - 4), Quaternion.identity) as GameObject;
                }
                lastFenceRight.transform.localScale = new Vector3(4, 14, 13);
                lastFenceRight.transform.localEulerAngles = Vector3.zero;
                lastFenceRight.transform.parent = wireFence.transform;
            }
            else if(direction == 0)
                DestroyFences();
    }

    private void DestroyFences()
    {
        if (Fences.Count != 0)
        {
            for (int i = 0; i < Fences.Count; i++)
            {
                Destroy(Fences[i]);
                Destroy(lastFenceLeft);
                Destroy(lastFenceRight);
                lastFenceLeft = null;
                lastFenceRight = null;
            }
        }

        Fences = new List<GameObject>();
    }
}
