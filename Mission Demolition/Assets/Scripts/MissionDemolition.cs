using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Inscribed")]
    public Text uitLevel;
    public Text uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    // This will hold the reference to the win popup UI
    public GameObject winPopup;
    public Text finalScoreText;
    public Text topScoreText;
    

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    private void Start() {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel() {
        // Git rid of the old castle
        if (castle != null) Destroy(castle);

        // Destroy the old projectile
        Projectile.DESTROY_PROJECTILES();

        // Instantiate the new castle
        if (level < levelMax) {
            castle = Instantiate<GameObject>(castles[level]);
            castle.transform.position = castlePos;

            // Reset the goal
            Goal.goalMet = false;

            UpdateGUI();

            mode = GameMode.playing;
        }

        // Zoom out to show both
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI() {
        // Show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    private void Update() {
        UpdateGUI();

        // Check for level end
        if (mode == GameMode.playing && Goal.goalMet) {
            // If the goal is met, stop the game
            mode = GameMode.levelEnd;
            // Zoom out to show both
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            
            // Start the next level after a delay
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel() {
    level++;
    if (level == levelMax) {
        // Show win popup UI
        winPopup.SetActive(true);

        // Update score display
        finalScoreText.text = "Your Score: " + shotsTaken;

        // Rename variable here to avoid the duplicate declaration error.
        int currentTopScore = PlayerPrefs.GetInt("TopScore", int.MaxValue);
        if (shotsTaken < currentTopScore) {
            PlayerPrefs.SetInt("TopScore", shotsTaken);
            topScoreText.text = "New High Score! 🎉";
        } else {
            topScoreText.text = "Top Score: " + currentTopScore;
        }
    }
    StartLevel();
}

    // Static method that allows code anywhere to increment the shots taken
    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    // Static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE() {
        return S.castle;
    }

    public void PlayAgain() {
        Time.timeScale = 1f;
        // reload current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    }

    public void ReturnToMenu() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
