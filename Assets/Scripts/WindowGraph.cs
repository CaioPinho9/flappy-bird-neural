using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private List<GameObject> gameObjectList;
    public List<int> score = new();
    private int lastScoreCount = 0;
    private int highScore;

    private void Update()
    {
        if (score.Count != lastScoreCount)
        {
            HighScore();
            ShowGraph(score);
            GameObject.Find("highScore").GetComponent<Text>().text = "HighScore " + highScore.ToString();
            GameObject.Find("lastScore").GetComponent<Text>().text = "LastScore " + score[^1].ToString();
            lastScoreCount = score.Count;
        }
    }

    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        score.Add(0);
    }

    private void HighScore()
    {
        foreach (int value in score)
        {
            if (value > highScore)
            {
                highScore = value;
            }
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = new(0, 0, 0, 0);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return gameObject;
    }

    private void ShowGraph(List<int> valueList)
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximun = valueList[0];
        float yMinimun = valueList[0];
        
        foreach (int value in valueList)
        {
            if (value > yMaximun)
            {
                yMaximun = value;
            }

            if (value < yMinimun)
            {
                yMinimun = value;
            }
        }

        yMaximun += (yMaximun - yMinimun) * .1f;
        yMinimun -= (yMaximun - yMinimun) * .1f;

        float xSize = graphWidth / (score.Count + 1);
        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimun) / (yMaximun - yMinimun)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);
            if(lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
        gameObject.GetComponent<Image>().color = new(0, 0, 1 ,.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.sizeDelta = new Vector2(distance * 2, 3f);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = dotPositionA + .5f * distance * dir;
        rectTransform.localEulerAngles = new(0, 0, GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
