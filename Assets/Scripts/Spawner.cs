using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float queueTime = 3f;
    private float time = 0;
    public GameObject obstacle;

    public float max;
    public float min;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time > queueTime)
        {
            GameObject go = Instantiate(obstacle);
            go.transform.position = new Vector3(3.5f, Random.Range(min, max), 0);
            
            time = 0;

            Destroy(go, 13);
        }

        time += Time.deltaTime;
    }
}
