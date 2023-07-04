using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField] private Vector2Int dimensions = Vector2Int.one;
    [SerializeField] private Text gridOutput;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;

        // set up the scene
        StartCoroutine(Demo());
    }

    private IEnumerator Demo()
    {
        // it's a good idea to put a loading screen here

        // pool the matchables
        pool.PoolObjects(dimensions.x * dimensions.y * 2);

        // create the grid
        grid.InitializeGrid(dimensions);
        
        yield return null;

        StartCoroutine(grid.PopulateGrid(false, true));

        // then remove the loading screen down here


        // check for gridlock and offer a player a hint if they need it
        grid.CheckPossibleMoves();
    }

    public void NoMoreMoves()
    {
        // reward the player
        grid.MatchEverything();
    }
}
