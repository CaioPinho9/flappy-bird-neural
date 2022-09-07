using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Config")]
    public int birdAmmount;
    public int birdSurviveAmmount;
    public GameObject prefab;

    [Header("Manage")]
    public int birdAlive;
    public GameObject bestBird;
    public List<GameObject> bestBirds;
    public GameObject[] birds;

    [Header("Timer")]
    public float time;
    public float queueTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        CreateBirds(birdAmmount);
        birds = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (time > queueTime)
        {
            Check();

            time = 0;
        }
        time += Time.deltaTime;

        if (birdAlive <= 0)
        {
            Restart();
        }
    }

    void CreateBirds(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            GameObject bird = Instantiate(prefab);
            bird.transform.parent = transform;
        }
    }

    void Check()
    {
        birdAlive = birdAmmount;
        bestBird = birds[0];
        foreach (GameObject bird in birds)
        {
            bird.GetComponent<Player>().visible = false;
            if (bird.GetComponent<Player>().gameOver)
            {
                birdAlive--;
                bird.GetComponent<Player>().visible = true;
            }

            if (bestBird.GetComponent<Player>().score < bird.GetComponent<Player>().score)
            {
                bestBird = bird;
                bird.GetComponent<Player>().visible = true;
            }

            if (bestBirds.Count < birdSurviveAmmount)
            {
                bestBirds.Add(bird);
                bird.GetComponent<Player>().visible = true;
            }
            else
            {
                for (int index = 0; index < bestBirds.Count; index++)
                {
                    if (bestBirds[index].GetComponent<Player>().score < bird.GetComponent<Player>().score)
                    {
                        bestBirds[index] = bird;
                        bird.GetComponent<Player>().visible = true;
                    }
                }
            }
        }
    }

    void Restart()
    {
        int index = 0;
        int lastIndex = -1;
        string dna = "";
        foreach (GameObject bird in birds)
        {
            int bestBirdIndex = (int)Math.Floor(index / (double)(birdAmmount / 10));
            GameObject birdMother = bestBirds[bestBirdIndex];

            if (bestBirdIndex != lastIndex)
            {
                dna = birdMother.GetComponent<Player>().Copy();
                lastIndex = bestBirdIndex;
            }

            bird.GetComponent<Player>().RestartNetwork();
            bird.GetComponent<Player>().Paste(dna);
            bird.GetComponent<Player>().Mutate();
            index++;
        }
    }
}
