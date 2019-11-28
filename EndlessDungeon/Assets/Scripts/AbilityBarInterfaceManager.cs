using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarInterfaceManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private AbilityButton abilityButtonPrefab;

    private AbilityButton[] abilityButtons;

    private KeyCode[] bindings;

    private int Length => player.Abilities.Length;

    private void Start()
    {
        abilityButtons = new AbilityButton[Length];
        bindings = new KeyCode[Length];
        for (int i = 0; i < Length; i++)
        {
            abilityButtons[i] = Instantiate(abilityButtonPrefab, transform);
            abilityButtons[i].SetAbility(player.Abilities[i]);
            abilityButtons[i].SetPressed(false);
            bindings[i] = (KeyCode)PlayerPrefs.GetInt("abilities" + i);
        }

        SetDefaultBindings();
    }

    void Update()
    {
        for (int i = 0; i < Length; i++)
        {
            if (player.Abilities[i] == null)
                continue;
            abilityButtons[i].SetCooldown(player.Abilities.CooldownRemaining(i), player.Abilities.CooldownMax(i));
        }

        
    }

    

    public void SetAbility(Ability ability, int index)
    {
        if (index < 0 || index >= Length)
            return;
        player.Abilities.SetAbility(ability, index);
        abilityButtons[index].SetAbility(ability);
    }

    public bool AbilityHeld(int index)
    {
        return Input.GetKey(bindings[index]);
    }

    public void SetBinding(KeyCode key, int index)
    {
        if (index < 0 || index >= Length)
            return;
        abilityButtons[index].SetBinding(key);
        for (int i = 0; i < Length; i++)
        {
            if (PlayerPrefs.HasKey("abilities" + i ) && PlayerPrefs.GetInt("abilities" + i) == (int)key)
            {
                PlayerPrefs.DeleteKey("abilities" + i);
                bindings[i] = KeyCode.None;
            }
        }
        //TODO go through bindings for other actions

        PlayerPrefs.SetInt("ability" + index, (int)key);
        bindings[index] = key;
    }

    public KeyCode GetBinding(int index) => index < 0 || index >= bindings.Length ? KeyCode.None : bindings[index];

    [ContextMenu("Set Default Bindings")]
    private void SetDefaultBindings()
    {
        SetBinding(KeyCode.Q, 0);
        SetBinding(KeyCode.W, 1);
        SetBinding(KeyCode.E, 2);
        SetBinding(KeyCode.R, 3);
        SetBinding(KeyCode.Mouse0, 4);
        SetBinding(KeyCode.Mouse1, 5);
    }

    public void SetPressed(bool pressed, int index)
    {
        if (index < 0 || index >= Length)
            return;
        abilityButtons[index].SetPressed(pressed);
    }
}
