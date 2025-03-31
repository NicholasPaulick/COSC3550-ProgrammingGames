using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public int topScore = 0;
    public Text uiTopScore;
    

    private void Start() {
        // Check to see if there is a high score in PlayerPrefs
        if (!PlayerPrefs.HasKey("TopScore")) {
            // If not, initialize it to 0
            PlayerPrefs.SetInt("TopScore", topScore);
        }
        // Load the top score from PlayerPrefs
        topScore = PlayerPrefs.GetInt("TopScore");
        // Update the UI with the top score
        UpdateTopScore();
    }

    // Update UI with the top score
    public void UpdateTopScore() {
        uiTopScore.text = "Top Score: " + topScore;
    }

    // Starts the game by loading the first level
    public void StartNewGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ResetTopScore() {
        // Reset the top score in PlayerPrefs to 0
        PlayerPrefs.SetInt("TopScore", 1000000); // Set to a high number to avoid confusion
        // Update the local top score variable
        topScore = 1000000;
        // Update the UI to reflect the change
        UpdateTopScore();
    }

    public void QuitGame() {
        // Quit the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // For editor
        #else
            Application.Quit(); // For build
        #endif
    }
}
