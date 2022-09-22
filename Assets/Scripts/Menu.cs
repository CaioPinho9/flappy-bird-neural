using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static int birdAmount;
    public static int birdSurviveAmount;
    public static float randomAmount;
    
    public void ChangeScene()
    {
        birdAmount = int.Parse(GameObject.Find("InputBirdAmount").GetComponent<TMP_InputField>().text);
        birdSurviveAmount = int.Parse(GameObject.Find("InputBirdSurviveAmount").GetComponent<TMP_InputField>().text);
        randomAmount = float.Parse(GameObject.Find("InputRandomAmount").GetComponent<TMP_InputField>().text);

        SceneManager.LoadScene("AIScene");
    }
}
