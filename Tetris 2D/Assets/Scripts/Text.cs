using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Text : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public float RemainingTime = 120f;

    // Start is called before the first frame update
    void Start()
    {
        RemainingTime = 120f;
    }

    // Update is called once per frame
    void Update()
    {
        RemainingTime -= Time.deltaTime;

        // Score
        text1.text = "Score: " + BlockSpawner.score.ToString();

        int minutes = Mathf.FloorToInt(RemainingTime / 60); // Converting float to minutes and secounds
        int seconds = Mathf.FloorToInt(RemainingTime % 60);

        // Timer
        text2.text = "Remaining Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);

        // Reset 
        if (RemainingTime <= 0)
        {
            SceneManager.LoadScene(1); // Reset the screen
        }
    }
}
