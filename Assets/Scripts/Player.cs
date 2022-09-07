using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    public NeuralNetwork network;
    public float velocity = 2.4f;
    public float jump;
    public float x;
    public float y;
    public float score;
    public bool visible = false;
    public bool gameOver = false;
    public float time;
    public float queueTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        gameOver = false;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.freezeRotation = true;
        rigidbody.isKinematic = false;
        GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, visible ? 1f : .5f);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        GetComponent<Animator>().enabled = true;
        network = new();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                rigidbody.velocity = Vector2.up * velocity;
            }

            if (time > queueTime)
            {
                RunNetwork();
                time = 0;
            }
            time += Time.deltaTime;

            if (jump > 0)
            {
                rigidbody.velocity = Vector2.up * velocity;
            }
        }
        score += Time.deltaTime;
        x = transform.position.x;
        y = transform.position.y;
    }

    private void RunNetwork()
    {
        network.Clear();
        network.Input();
        network.Forward();

        jump = (network.layer[network.lastLayer].neuron[0].output > 0) ? 1 : 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            gameOver = true;
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            GetComponent<SpriteRenderer>().color = Color.black;
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            GetComponent<Animator>().enabled = false;
        }
    }
}

public class NeuralNetwork
{
    public static int[] neuronsLayer = { 2, 3, 3, 1 };
    public int lastLayer = neuronsLayer.Length;

    public Layer[] layer = new Layer[neuronsLayer.Length];

    public string dna;
    public static int randomStart = 100;

    public NeuralNetwork()
    {
        for (int i = 0; i < neuronsLayer.Length; i++)
        {
            layer[i] = new()
            {
                //How many neurons in this layer
                neuronCount = neuronsLayer[i],
                //Which layer is this
                layerId = i,
                //Range of link weight
                randomStart = randomStart,
                //How long is the network
                networkSize = neuronsLayer.Length
            };
        }
    }

    public string Copy()
    {
        dna = "";
        //Weight/Bias;Weight/Bias;Weight/Bias;
        //";" separates different links, and "/" separates weight and bias
        foreach (Layer layer in layer)
        {
            foreach (Link link in layer.link)
            {
                dna += link.weight + "/" + link.bias + ";";
            }
        }
        return dna;
    }

    public void Paste(string dna)
    {
        string[] rna = dna.Split(";");
        int index = 0;
        //Weight/Bias;Weight/Bias;Weight/Bias;
        //";" separates different links, and "/" separates weight and bias
        foreach (Layer layer in layer)
        {
            foreach (Link link in layer.link)
            {
                String[] gene = rna[index].Split("/");
                link.weight = float.Parse(gene[0]);
                link.bias = float.Parse(gene[1]);
                index++;
            }
        }
    }
    
    public void CreateNeurons()
    {
        int neuronId = 0;
        //Create neurons to each layer
        foreach (Layer layer in layer)
        {
            neuronId = layer.CreateNeurons(neuronId);
        }
    }

    public void LinkLayers()
    {
        //Layer 1 connects with 2, etc
        for (int index = 0; index < layer.Length - 1; index++)
        {
            layer[index].LinkNeurons(layer[index], layer[index + 1]);

        }
    }

    public void Clear()
    {
        foreach (Layer layer in layer)
        {
            if (layer.layerId > 0)
            {
                //Reset neuron input
                for (int index = 0; index < layer.neuron.Count; index++)
                {
                    layer.neuron[index].input = 0;
                    layer.neuron[index].output = 0;
                }
            }
        }
    }

    public void Forward()
    {
        for (int index = 0; index < layer.Length; index++)
        {
            layer[index].Forward(layer[index]);
        }
    }
}

public class Layer
{
    //How many neurons there's in this layer
    public int neuronCount;
    //Id
    public int layerId;
    //Last layer
    public int networkSize;
    //Neurons in this layer
    public List<Neuron> neuron = new();
    //Connections that start in this layer
    public List<Link> link = new();
    public int randomStart;

    public Layer() {}

    public int CreateNeurons(int neuronId)
    {
        //Create neurons, limiting to how many must be
        for (int index = neuron.Count; index < neuronCount; index++)
        {
            neuron.Add(new(this, neuronId));
            neuronId++;
        }
        return neuronId;
    }

    public void LinkNeurons(Layer thisLayer, Layer nextLayer)
    {
        //Iterates to connect every neuron in layer 1 with each neuron in layer 2
        for (int thisIndex = 0; thisIndex < thisLayer.neuronCount; thisIndex++)
        {
            for (int nextIndex = 0; nextIndex < nextLayer.neuronCount; nextIndex++)
            {
                //Create link between neurons
                link.Add(new Link(thisLayer.neuron[thisIndex], nextLayer.neuron[nextIndex], UnityEngine.Random.Range(-randomStart, randomStart), UnityEngine.Random.Range(-randomStart, randomStart)));
            }
        }
    }
    public void Forward(Layer layer)
    {
        if (layer.layerId > 0)
        {
            for (int index = 0; index < layer.neuron.Count; index++)
            {
                //Debug.Log("ReLU");
                //Debug.Log(layer.neuron[index].input);
                layer.neuron[index].output = layer.neuron[index].ReLU();
                //Debug.Log(layer.neuron[index].output);
            }
        }

        if (layer.layerId < networkSize)
        {
            for (int index = 0; index < layer.link.Count; index++)
            {
                //Debug.Log("Weight");
                //Debug.Log(layer.link[index].neuron1.output);
                layer.link[index].neuron2.input += layer.link[index].Weight();
                //Debug.Log(layer.link[index].neuron2.input);
            }
        }
    }

}

public class Link
{
    //UI reference
    public GameObject render;

    //Beginning of link
    public Neuron neuron1;
    //Ending
    public Neuron neuron2;

    //Used to proccess data
    public float weight;
    public float bias;
    public Link(Neuron neuron1, Neuron neuron2, float weight, float bias)
    {
        this.neuron1 = neuron1;
        this.neuron2 = neuron2;
        this.weight = weight;
        this.bias = bias;
    }

    public float Weight()
    {
        return neuron1.output * weight + bias;
    }
}
public class Neuron
{
    //UI reference
    public GameObject render;

    //Where this neuron is located
    public Layer layer;
    public int neuronId;

    //Input and output
    public float input = 0;
    public float output = 0;

    public Neuron(Layer layer, int neuronId)
    {
        this.layer = layer;
        this.neuronId = neuronId;
    }

    //Rectifier
    public float ReLU()
    {
        //Activate if input > 0
        if (input > 0)
        {
            return input;
        }
        return 0;
    }
}
