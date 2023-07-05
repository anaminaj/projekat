using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private Cursor cursor;
    private AudioMixer audioMixer;
    private ScoreManager score;

    [SerializeField]
    private Fader loadingScreen,
                  darkener;

    [SerializeField]
    private Text finalScoreText;

    [SerializeField]
    private Movable resultsPage;

    [SerializeField]
    private bool levelIsTimed;

    [SerializeField]
    private LevelTimer timer;

    [SerializeField]
    private float timeLimit;

    [SerializeField] private Vector2Int dimensions = Vector2Int.one;
    [SerializeField] private Text gridOutput;

    [SerializeField] private bool debugMode;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
        cursor = Cursor.Instance;
        audioMixer = AudioMixer.Instance;
        score = ScoreManager.Instance;


        // set up the scene
        StartCoroutine(Demo());
    }

    // comment this out before building
    private void Update()
    {
        if (debugMode && Input.GetButtonDown("Jump"))
            NoMoreMoves();
    }

    private IEnumerator Demo()
    {
        // disable user input
        cursor.enabled = false;

        // unhide loading screen
        loadingScreen.Hide(false);

        //if the level is timed set the timer
        if (levelIsTimed)
            timer.SetTimer(timeLimit);

        // pool the matchables
        pool.PoolObjects(dimensions.x * dimensions.y * 2);

        // create the grid
        grid.InitializeGrid(dimensions);

        // fade out loading screen
        StartCoroutine(loadingScreen.Fade(0));

        // start backround music
        audioMixer.PlayMusic();

        // populate the grid
        yield return StartCoroutine(grid.PopulateGrid(false, true));

        // check for gridlock and offer a player a hint if they need it
        grid.CheckPossibleMoves();

        // enable user input
        cursor.enabled = true;
        //if the level is timed start the timer
        if (levelIsTimed)
            StartCoroutine(timer.Countdown());
    }

    public void NoMoreMoves()
    {
        // reward the player if the level is timed and he runs out of moves
        if(levelIsTimed)
            grid.MatchEverything();
        else
        GameOver();
       
    }

    public void GameOver()
    {
        //get and update the final score for the resukts page
        finalScoreText.text = score.Score.ToString();

        //disable the cursor
        cursor.enabled = false;

        //unhide the darkener
        darkener.Hide(false);
        StartCoroutine(darkener.Fade(0.75f));

        //move the results page onto the screen
        StartCoroutine(resultsPage.MoveToPosition(new Vector2(Screen.width/2,Screen.height/2)));
    }

    private IEnumerator Quit()
    {
        yield return StartCoroutine(loadingScreen.Fade(1));
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitButtonPressed()
    {
        StartCoroutine(Quit());
    }

    private IEnumerator Retry()
    {
        //fade off the darkener and move the results page off screen
        StartCoroutine(resultsPage.MoveToPosition(new Vector2(Screen.width / 2, Screen.height / 2) + Vector2.down * 1000));
        yield return StartCoroutine(darkener.Fade(0));
        darkener.Hide(true);

        //Reset the cursor,game grid, score, timer
        if (levelIsTimed)
            timer.SetTimer(timeLimit);
        cursor.Reset();
        score.Reset();


        yield return StartCoroutine(grid.Reset());         

        //allows the player to start playing again
        cursor.enabled = true;

        //if the level is timed start the timer
        if (levelIsTimed)
            StartCoroutine(timer.Countdown());
    }

    public void RetryButtonPressed()
    {
        StartCoroutine(Retry());
    }
}
