using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float queueTime = 3f;
    private float time;
    public GameObject obstacle;
    private int id;

    public float max;
    public float min;

    // Start is called before the first frame update
    public void Start()
    {
        time = 0;
        id = 0;
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles != null)
        {
            foreach (GameObject obstacle in obstacles)
            {
                Destroy(obstacle);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time > queueTime)
        {
            GameObject go = Instantiate(obstacle);
            go.transform.position = new Vector3(3.5f, Random.Range(min, max), 0);
            go.GetComponent<Obstacle>().id = id;
            
            time = 0;

            Destroy(go, 13);
            id++;
        }

        time += Time.deltaTime;
    }
}
