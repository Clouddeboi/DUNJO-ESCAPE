using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //video followed: https://www.youtube.com/watch?v=MNUYe0PWNNs&t=182s&ab_channel=RehopeGames
    // AudioManager AudioManager;

    // private void Awake()
    // {
    //     AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    // }

    [SerializeField] GameObject pauseMenu;
    public void Pause()//pause button
    {
        pauseMenu.SetActive(true);//pauses the game vy setting the butto to true
        //AudioManager.PlaySFX(AudioManager.ButtonClick);
        Time.timeScale = 0;//pauses time
    }
    public void MainMenu()//return to main menu button- this button is also reused for the game over screen
    {
        SceneManager.LoadScene("MainMenu");//loads the main menu scene
        //AudioManager.PlaySFX(AudioManager.StartButtonClick);
        Time.timeScale = 1;//sets time back to normal from pause
    }
    public void Resume()//resume game Button
    {
        pauseMenu.SetActive(false);//sets the pause menu to false and lets us play the game again
        //AudioManager.PlaySFX(AudioManager.ButtonClick);
        Time.timeScale = 1;//sets time back to normal from pause
    }
    public void Restart()//restart level- this button is also used for the game over screen
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//gets the active scene from the build index and restarts it
        //AudioManager.PlaySFX(AudioManager.StartButtonClick);
        Time.timeScale = 1;//sets time back to normal from pause
    }

}
