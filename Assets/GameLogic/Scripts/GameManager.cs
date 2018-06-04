using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text timerText;
    public Text scoreText;
    public Text throttleText;

    //public GameObject throttlePopupPrefab; //?????????

    public static GameManager StaticManager = null;

    private int score;
    private float startTimeOffset;
    private bool isTiming;

    private void Awake()
    {
        if (StaticManager == null)
            StaticManager = this;
        else if (StaticManager != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        score = 0;

        timerText.text = "00:00:00";
        scoreText.text = "000,000,000";
        throttleText.text = "0%";

        startTimeOffset = 0.0f;
        isTiming = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (isTiming)
        {
            // Update the timer text:
            float timeVal = Time.timeSinceLevelLoad - startTimeOffset;

            int ms = (int)((timeVal % 1) * 100);
            string msStr = ms.ToString();
            if (msStr.Length < 2)
                msStr = "0" + msStr;

            timeVal = (int)timeVal;

            int sec = (int)(timeVal % 60);
            string secStr = sec.ToString();
            if (secStr.Length < 2)
                secStr = "0" + secStr;

            timeVal /= 60;

            int min = (int)(timeVal % 60);
            string minStr = min.ToString();
            if (minStr.Length < 2)
                minStr = "0" + minStr;

            timerText.text = minStr + ":" + secStr + ":" + msStr;
        }

	}

    public void UpdateThrottleValue(float normalizedThrottleValue)
    {
        if (normalizedThrottleValue >= 0)
        {
            normalizedThrottleValue *= 100;
            normalizedThrottleValue = Mathf.Round(normalizedThrottleValue);
            throttleText.text = normalizedThrottleValue.ToString() + "%";


            //Instantiate<GameObject>(throttlePopupPrefab, this.gameObject.transform);
        }
    }

    public void AddPoints(int newPoints)
    {
        // Note: Max score string length = "999,999,999". Will wrap arount to "000,000,000", but actual score value is maintained... Shouldn't be a problem.
        score += newPoints;

        string scoreStr = score.ToString();
        scoreStr = "00000000" + scoreStr;
        scoreStr = scoreStr.Substring(scoreStr.Length - 9, 3) + "," + scoreStr.Substring(scoreStr.Length - 6, 3) + "," + scoreStr.Substring(scoreStr.Length - 3, 3);
        
        scoreText.text = scoreStr;
    }

    public void StartTimer()
    {
        startTimeOffset = Time.timeSinceLevelLoad;
        isTiming = true;
    }

    public void StopTimer()
    {
        isTiming = false;
    }
}
