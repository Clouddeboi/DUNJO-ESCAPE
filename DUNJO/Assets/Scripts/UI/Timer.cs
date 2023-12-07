using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    //video followed: https://www.youtube.com/watch?v=POq1i8FyRyQ&ab_channel=RehopeGames
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI TimeCompleted;
    [SerializeField] float remainingTime;
    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject WinScreen;

    // Update is called once per frame
    void Update()
    {   //prevents timer going into -numbers
        if(remainingTime > 0)//if time is greater than 0 keep counting down as normal
        {
            remainingTime -= Time.deltaTime;   
        }
        else if(remainingTime < 0)//else set time remaining to 0
        {
            remainingTime = 0;
            Time.timeScale = 0;//freezes time
            GameOverScreen.SetActive(true);//sets panel to true
        }
        //remaining time maths for better format (min & secs)
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")//if object collided is tagged as player
        {
            Time.timeScale = 0;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            TimeCompleted.text = String.Format("{0:00}:{1:00}", minutes, seconds);
            WinScreen.SetActive(true);
        }
    }
    
}