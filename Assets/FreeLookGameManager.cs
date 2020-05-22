using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookGameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject freeLookCamera;

    public KeyCode freeLookKey;

    bool freeLook = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(freeLookKey))
            EnableFreeLook();
    }

    public void EnableFreeLook()
    {
        freeLook = !freeLook;

        player.SetActive(!freeLook);
        freeLookCamera.SetActive(freeLook);
    }
}
