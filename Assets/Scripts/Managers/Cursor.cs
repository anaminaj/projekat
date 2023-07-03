using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cursor : Singleton<Cursor>
{
    public bool cheatMode;

    private MatchablePool pool;
    private MatchableGrid grid;

    private SpriteRenderer spriteRenderer;

    // which 2 matchables are currently selected?
    private Matchable[] selected;

    // these variables will be used to stretch and reposition the cursor to cover 2 matchables
    [SerializeField]
    private Vector2Int verticalStretch = new Vector2Int(1, 2), 
                       horizontalStretch = new Vector2Int(2, 1);

    [SerializeField]
    private Vector3 halfUp = Vector3.up / 2,
                    halfDown = Vector3.down / 2,
                    halfLeft = Vector3.left / 2,
                    halfRight = Vector3.right / 2;

    // since this is a singleton, using Init instead of Awake to initialize stuff
    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.enabled = false;

        selected = new Matchable[2];
    }
    
    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
    }

    private void Update()
    {
        if (!cheatMode || selected[0] == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            pool.ChangeType(selected[0], 0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            pool.ChangeType(selected[0], 1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            pool.ChangeType(selected[0], 2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            pool.ChangeType(selected[0], 3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            pool.ChangeType(selected[0], 4);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            pool.ChangeType(selected[0], 5);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            pool.ChangeType(selected[0], 6);
    }

    // select the 1st of 2 matchables, move the cursor to it, reset the size, and activate the sprite
    public void SelectFirst(Matchable toSelect)
    {
        selected[0] = toSelect;

        if (!enabled || selected[0] == null)
            return;

        transform.position = toSelect.transform.position;

        spriteRenderer.size = Vector2.one;
        spriteRenderer.enabled = true;
    }

    public void SelectSecond(Matchable toSelect)
    {
        selected[1] = toSelect;

        if(!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle || selected[0] == selected[1])
            return;

        if (SelectedAreAdjacent())
            StartCoroutine(grid.TrySwap(selected));

        SelectFirst(null);
    }

    private bool SelectedAreAdjacent()
    {
        if(selected[0].position.x == selected[1].position.x)
        {
            if(selected[0].position.y == selected[1].position.y + 1)
            {
                spriteRenderer.size = verticalStretch;
                transform.position += halfDown;
                return true;
            }
            else if(selected[0].position.y == selected[1].position.y - 1)
            {
                spriteRenderer.size = verticalStretch;
                transform.position += halfUp;
                return true;
            }
        }
        else if(selected[0].position.y == selected[1].position.y)
        {
            if(selected[0].position.x == selected[1].position.x + 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += halfLeft;
                return true;
            }
            else if(selected[0].position.x == selected[1].position.x - 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += halfRight;
                return true;
            }
        }
        return false;
    }
}
