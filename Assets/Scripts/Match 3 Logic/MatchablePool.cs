using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] colors;

    [SerializeField] private Sprite match4Powerup;
    [SerializeField] private Sprite match5Powerup;
    [SerializeField] private Sprite crossPowerup;

    // get a matchable from the pool and randomize its type
    public void RandomizeType(Matchable toRandomize)
    {
        int random = Random.Range(0, howManyTypes);

        toRandomize.SetType(random, sprites[random], colors[random]);
    }

    // randomize the type of a matchable
    public Matchable GetRandomMatchable()
    {
        Matchable randomMatchable = GetPooledObject();

        RandomizeType(randomMatchable);

        return randomMatchable;
    }

    // increment the type of a matchable and return its new type
    public int NextType(Matchable matchable)
    {
        int nextType = (matchable.Type + 1) % howManyTypes;

        matchable.SetType(nextType, sprites[nextType], colors[nextType]);

        return nextType;
    }

    public Matchable UpgradeMatchable(Matchable toBeUpgraded, MatchType type)
    {
        if(type == MatchType.cross)
            return toBeUpgraded.Upgrade(MatchType.cross, crossPowerup);

        if (type == MatchType.match4)
            return toBeUpgraded.Upgrade(MatchType.match4, match4Powerup);
        
        if (type == MatchType.match5)
            return toBeUpgraded.Upgrade(MatchType.match5, match5Powerup);

        Debug.LogWarning("Tried to upgrade a matchable with an invalid match type.");
        return toBeUpgraded;
    } 

    // manually set the type of a matchable, used for testing obscure cases
    public void ChangeType(Matchable toChange, int type)
    {
        toChange.SetType(type, sprites[type], colors[type]);
    }
}
