using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager singleton;

    public KeyCode loadCheckpointKey;

    Transform lastCheckpointReached;
    Transform player;

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<InertiaCC>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(loadCheckpointKey))
            LoadCheckpoint();
    }

    public void newCheckpointReached(Transform position)
    {
        lastCheckpointReached = position;
    }

    public void LoadCheckpoint()
    {
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = lastCheckpointReached.transform.position;
        player.transform.rotation = lastCheckpointReached.transform.rotation;

    }

}
