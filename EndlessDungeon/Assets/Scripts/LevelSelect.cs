using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    private Dictionary<int, LevelData> cachedLevelData = new Dictionary<int, LevelData>();

    [SerializeField]
    private LevelGenerationPreset[] levelPresets;

    private int currentLevel = -1;

    public void LoadLevel(int level)
    {
        if (level == currentLevel)
            return;

        if (cachedLevelData.TryGetValue(level, out LevelData data))
        {

        }
        else
        {

        }
    }
}
