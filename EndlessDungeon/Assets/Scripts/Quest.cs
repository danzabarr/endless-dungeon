using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public new string name;

    public string description;

    public QuestObjective[] objectives;

    [Tooltip("For no time limit use a negative number.")]
    public float timeLimit = -1;

    [Tooltip("Progress persists between new games.")]
    public bool persistent = false;

    [Tooltip("Can only be started once.")]
    public bool unique = false;

    public int goldReward;
    public int expReward;

    [Tooltip("Next quest to start upon completion.")]
    public Quest continuation;

}
