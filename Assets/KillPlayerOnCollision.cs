using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        InertiaCC cc = other.gameObject.GetComponent<InertiaCC>();

        if (cc)
        {
            CheckpointManager.singleton.LoadCheckpoint();
        }
    }
}
