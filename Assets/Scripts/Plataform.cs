using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataform : MonoBehaviour
{
    public float queueTime = 3f;
    private float time = 0;
    public GameObject plataform;
    public float distance = 1.56f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject go = Instantiate(plataform);
            go.transform.parent = GameObject.Find("Plataform").transform;
            go.transform.position = new Vector3(-3.5756f + distance, -1.54f, 0);
            distance += 1.56f;
            Destroy(go, 13);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time > queueTime)
        {
            GameObject go = Instantiate(plataform);
            go.transform.parent = GameObject.Find("Plataform").transform;
            go.transform.position = new Vector3(3.5f, -1.54f, 0);

            time = 0;

            Destroy(go, 13);
        }
        time += Time.deltaTime;
    }
}
