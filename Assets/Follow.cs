using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        this.transform.position = target.transform.position + offset;
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
    }
}
