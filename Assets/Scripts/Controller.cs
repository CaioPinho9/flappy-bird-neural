using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("Config")]
    public int birdAmmount;
    public int birdSurviveAmmount;
    public float randomAmmount;
    public GameObject prefab;

    [Header("Manage")]
    public int birdAlive;
    public GameObject bestBird;
    public List<GameObject> bestBirds;
    public GameObject[] birds;
    private GameObject nextObstacle;
    private int obstacleId = 0;
    private int gen = 1;

    [Header("Timer")]
    public float time;
    public float queueTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        CreateBirds(birdAmmount);
        birds = GameObject.FindGameObjectsWithTag("Player");
        birds[0].GetComponent<Player>().Start();
        GameObject.Find("UI").GetComponent<NetworkUI>().Build(birds[0]);
        GameObject.Find("alive").GetComponent<Text>().text = "Birds " + birdAmmount.ToString() + " / " + birdAmmount.ToString();
        Check();
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

        if (nextObstacle == null || nextObstacle.transform.position.x < -.26f)
        {
            DetectObstacle();
        }

        ObstacleCoords();

        if (birdAlive <= 0)
        {
            Check();
            Restart();
        }
    }

    void CreateBirds(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            GameObject bird = Instantiate(prefab);
            bird.transform.parent = transform;
            bird.GetComponent<Player>().id = i;
        }
    }

    void Check()
    {
        birdAlive = birdAmmount;
        bestBird = birds[0];
        bestBirds.Clear();
        foreach (GameObject bird in birds)
        {
            bird.GetComponent<Player>().visible = false;
            if (bird.GetComponent<Player>().gameOver)
            {
                birdAlive--;
                GameObject.Find("alive").GetComponent<Text>().text = "Birds " + birdAlive.ToString() + " / " + birdAmmount.ToString();
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
                int index = 0;
                for (int i = 0; i < bestBirds.Count; i++)
                {
                    for (int j = 0; j < bestBirds.Count; j++)
                    {
                        if (bestBirds[i].GetComponent<Player>().score < bestBirds[j].GetComponent<Player>().score &&
                            bestBirds[i].GetComponent<Player>().score < bestBirds[index].GetComponent<Player>().score)
                        {
                            index = i;
                        }
                    }
                }

                if (bestBirds[index].GetComponent<Player>().score < bird.GetComponent<Player>().score)
                {
                    bestBirds[index] = bird;
                    bird.GetComponent<Player>().visible = true;
                }
            }
        }
        GameObject.Find("best").GetComponent<Text>().text = "Best ID " + bestBird.GetComponent<Player>().id.ToString();
    }

    void UpdateUI()
    {
        gen++;
        GameObject.Find("UI").GetComponent<NetworkUI>().Build(bestBird);
        GameObject.Find("Window Chart").GetComponent<WindowGraph>().score.Add((int)bestBird.GetComponent<Player>().score);
        GameObject.Find("gen").GetComponent<Text>().text = "Gen " + gen.ToString();
    }

    void Restart()
    {
        obstacleId = 0;
        int index = 0;
        int lastIndex = -1;
        string dna = "";
        birdAlive = birdAmmount;
        UpdateUI();

        Debug.Log(bestBird.GetComponent<Player>().network.Copy());

        foreach (GameObject bird in birds)
        {
            bool isBest = false;
            foreach (GameObject best in bestBirds)
            {
                if (bird.GetComponent<Player>().id == best.GetComponent<Player>().id)
                {
                    isBest = true;
                }
            }
            if (isBest)
            {
                bird.GetComponent<Player>().Restart();
            } 
            else
            {
                int bestBirdIndex = (int)Math.Floor((double)index * (1 + randomAmmount) * (double)(birdSurviveAmmount / (double)(birdAmmount - birdSurviveAmmount)));
                bird.GetComponent<Player>().Restart();
                if (bestBirdIndex < bestBirds.Count)
                {
                    GameObject birdMother = bestBirds[bestBirdIndex];

                    if (bestBirdIndex != lastIndex)
                    {
                        dna = birdMother.GetComponent<Player>().network.Copy();
                        lastIndex = bestBirdIndex;
                    }
                    bird.GetComponent<Player>().network.Paste(dna);
                    bird.GetComponent<Player>().network.Mutate();
                }
                else
                {
                    bird.GetComponent<Player>().network.Random();
                }
                index++;
            }
        }
        GameObject.Find("ObstacleSpawner").GetComponent<Spawner>().Start();
    }

    void DetectObstacle()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle.GetComponent<Obstacle>().id == obstacleId)
            {
                nextObstacle = obstacle;
                obstacleId++;
                break;
            }
        }
    }

    void ObstacleCoords()
    {
        foreach (GameObject bird in birds)
        {
            if (nextObstacle != null)
            {
                bird.GetComponent<Player>().obstacleX = nextObstacle.transform.position.x;
                bird.GetComponent<Player>().obstacleY = nextObstacle.transform.position.y;
            } 
            else
            {
                bird.GetComponent<Player>().obstacleX = 0;
                bird.GetComponent<Player>().obstacleY = 0;
            }
        }
    }
}
