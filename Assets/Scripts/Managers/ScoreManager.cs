using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField]
    private Transform collectionPoint;

    [SerializeField]
    private Text scoreText, comboText;

    [SerializeField]
    private Slider comboSlider;

    private int score, comboMultiplier;

    public int Score
    {
        get
        {
            return score;
        }
    }
   
    private float timeSinceLastScore;
    
    [SerializeField]
    private float maxComboTime, currentComboTime;

    private bool timerIsActive;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;

        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);
    }


    public void AddScore(int amount)
    {
        score += amount * IncreaseCombo();
        scoreText.text = "Score: " + score;

        timeSinceLastScore = 0;

        if (!timerIsActive)
        {
            StartCoroutine(ComboTimer());
        }
    }
    private IEnumerator ComboTimer()
    {

        timerIsActive = true;
        comboText.enabled = true;
        comboSlider.gameObject.SetActive(true);


        do
        {
            timeSinceLastScore += Time.deltaTime;
            comboSlider.value = 1 - timeSinceLastScore / currentComboTime;
            yield return null;
        } while (timeSinceLastScore < currentComboTime);

        comboMultiplier = 0;
        comboText.enabled = false;
        comboSlider.gameObject.SetActive(false);

        timerIsActive = false;
    }
    private int IncreaseCombo()
    {
        comboText.text = "Combo x" + ++comboMultiplier;

        currentComboTime = maxComboTime - Mathf.Log(comboMultiplier) / 2;

        return comboMultiplier;
    }

    public IEnumerator ResolveMatch(Match toResolve, MatchType powerupUsed = MatchType.invalid)
    {
        Matchable powerupFormed = null;
        Matchable matchable;

        Transform target = collectionPoint;

        if (powerupUsed == MatchType.invalid && toResolve.Count > 3)
        {
            powerupFormed = pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);
            toResolve.RemoveMatchable(powerupFormed);
            target = powerupFormed.transform;
            powerupFormed.SortingOrder = 3;
        }

        for(int i = 0; i != toResolve.Count; ++i)
        {
            matchable = toResolve.Matchables[i];

            // only allow gems used as powerups to resolve gems
            if (powerupUsed != MatchType.match5 && matchable.IsGem)
                continue;

            // remove the matchables from the grid
            grid.RemoveItemAt(matchable.position);

            // move them off to the side of the screen simultaneously
            // and wait for the last one to finish
            if (i == toResolve.Count - 1)
                yield return StartCoroutine(matchable.Resolve(target));
            else
                StartCoroutine(matchable.Resolve(target));
        }

        // update the player's score
        AddScore(toResolve.Count * toResolve.Count);

        // if there was a powerup, reset the sorting order
        if (powerupFormed != null)
            powerupFormed.SortingOrder = 1;

        // yield return null
    }

}
