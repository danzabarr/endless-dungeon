using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelList : MonoBehaviour
{
    [SerializeField]
    private StatsPanelTitle sectionTitlePrefab;

    [SerializeField]
    private StatsPanelEntry entryPrefab;

    [SerializeField]
    private StatsPanelTooltip tooltip;

    private Stats stats;

    private Dictionary<string, StatsPanelEntry> dictionary = new Dictionary<string, StatsPanelEntry>();
    private Dictionary<string, StatsPanelTitle> titles = new Dictionary<string, StatsPanelTitle>();

    public void Start()
    {
        stats = Player.Instance.Stats;

        Title("Core Stats");
        Set("Strength", "{0:0}", stats.Strength, 
            "<b>Strength</b>\nIncreases your <b>Maximum Health</b>, and <b>Weapon Damage</b>.");

        Set("Dexterity", "{0:0}", stats.Dexterity, 
            "<b>Dexterity</b>\nIncreases your spell <b>Cast Speed</b> and <b>Weapon Speed</b>.");

        Set("Magic", "{0:0}", stats.Magic, 
            "<b>Magic</b>\nIncreases your <b>Health Regeneration</b> rate and <b>Spell Damage</b>.");

        Title("Survivability");

        Set("Maximum Health", "{0:0}", stats.MaxHealth, 
            "<b>Maximum Health</b>\nYour total maximum health. More health will help you survive bigger hits and for longer periods of time before healing.");

        Set("Health Regeneration", "{0:0.##}/s", stats.RegenHealth, 
            "<b>Health Regeneration</b>\nThe rate at which your health automatically regenerates. A negative value will cause you to automatically lose health over time.");

        Set("Chance to Block", "{0:0.##%}", stats.BlockChance, 
            "<b>Chance to Block</b>\nYour chance to block incoming melee and ranged attacks, up to a maximum of 75%. Most damage from spells, periodic and area of effect abilities cannot be blocked. A blocked attack deals no damage.");

        Set("Block Speed", "{0:0.##}s", stats.BlockDuration,
            "<b>Hit Recovery Time</b>\nThe amount of time you take to recover after blocking a critical hit.");

        Set("Hit Recovery Speed", "{0:0.##}s", stats.HitRecoveryDuration,
            "<b>Hit Recovery Time</b>\nThe amount of time you take to recover after being critically hit.");

        Set("Armour", "{0:0}", stats.Armour,
            "<b>Armour</b>\nThe combined armour of all your equipment. Armour reduces the amount of physical damage received, up to a maximum of 75%.");

        Set("Physical Mitigation", "{0:0.##%}", stats.PhysicalMitigation,
            "<b>Physical Mitigation</b>\nThe amount of physical damage mitigated as a result of your armour.");

        Set("Fire Resistance", "{0:0.#%}", stats.FireMitigation, 
            "<b>Fire Resistance</b>\nReduces the amount of fire damage received, up to a maximum of 75%.", 30);

        Set("Cold Resistance", "{0:0.#%}", stats.ColdMitigation, 
            "<b>Cold Resistance</b>\nReduces the amount of cold damage received, up to a maximum of 75%.");

        Set("Lightning Resistance", "{0:0.#%}", stats.LightningMitigation, 
            "<b>Lightning Resistance</b>\nReduces the amount of lightning damage received, up to a maximum of 75%.");

        Set("Poison Resistance", "{0:0.#%}", stats.PoisonMitigation, 
            "<b>Poison Resistance</b>\nReduces the amount of poison damage received, up to a maximum of 75%.");

        Set("Shadow Resistance", "{0:0.#%}", stats.ShadowMitigation, 
            "<b>Shadow Resistance</b>\nReduces the amount of shadow damage received, up to a maximum of 75%.");

        Set("Holy Resistance", "{0:0.#%}", stats.HolyMitigation, 
            "<b>Holy Resistance</b>\nReduces the amount of holy damage received, up to a maximum of 75%.");

        Title("Spells");

        Set("Cast Speed", "{0:+0.##%}", stats.CastSpeed - 1, 
            "<b>Cast Speed</b>\nSpeed multiplier for casting and channelling spells.");


        Set("Fire Spell Damage", "{0:+0.##%}", stats.FireSpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for fire spells.", 30);

        Set("Cold Spell Damage", "{0:+0.##%}", stats.ColdSpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for cold spells.");

        Set("Lightning Spell Damage", "{0:+0.##%}", stats.LightningSpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for lightning spells.");

        Set("Poison Spell Damage", "{0:+0.##%}", stats.PoisonSpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for poison spells.");

        Set("Shadow Spell Damage", "{0:+0.##%}", stats.ShadowSpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for shadow spells.");

        Set("Holy Spell Damage", "{0:+0.##%}", stats.HolySpellDamage - 1,
            "<b>Fire Spell Damage</b>\nDamage multiplier for holy spells.");


        Title("Weapons");

        //Title("Main Hand Weapon");
        Set("Main Hand Weapon Damage", "{0:0}-{1:0}", stats.MainHandDamage,
            "<b>Main Hand Weapon Damage</b>\nThe minimum and maximum damage dealt by the weapon equipped in your main hand.", 30);

        Set("Main Hand Weapon Speed", "{0:0.##}/s", stats.MainHandAttacksPerSecond, 
            "<b>Main Hand Weapon Speed</b>\nThe number of standard attacks performed per second with the weapon equipped in your main hand. Certain abilities will take longer or shorter than this to perform. Check the ability tooltip for the actual attack speed for a certain ability.");

        //Title("Off Hand Weapon");
        Set("Off Hand Weapon Damage", "{0:0}-{1:0}", stats.OffHandDamage, 
            "<b>Off Hand Weapon Damage</b>\nThe minimum and maximum damage dealt by the weapon equipped in your off hand.", 30);

        Set("Off Hand Weapon Speed", "{0:0.##}/s", stats.OffHandAttacksPerSecond, 
            "<b>Off Hand Weapon Speed</b>\nThe number of standard attacks performed per second with the weapon equipped in your off hand. Certain abilities will take longer or shorter than this to perform. Check the ability tooltip for the actual attack speed for a certain ability.");
    }

    public StatsPanelEntry this[string name] => dictionary[name];

    public void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        Set("Strength", stats.Strength);
        Set("Dexterity", stats.Dexterity);
        Set("Magic", stats.Magic);

        Set("Maximum Health", stats.MaxHealth);
        Set("Health Regeneration", stats.RegenHealth);
        Set("Chance to Block", stats.BlockChance);
        Set("Block Speed", stats.BlockDuration);
        Set("Hit Recovery Speed", stats.HitRecoveryDuration);
        Set("Armour", stats.Armour);
        Set("Physical Mitigation", stats.PhysicalMitigation);

        Set("Fire Resistance", stats.FireMitigation);
        Set("Cold Resistance", stats.ColdMitigation);
        Set("Lightning Resistance", stats.LightningMitigation);
        Set("Poison Resistance", stats.PoisonMitigation);
        Set("Shadow Resistance", stats.ShadowMitigation);
        Set("Holy Resistance", stats.HolyMitigation);

        Set("Cast Speed", stats.CastSpeed - 1);

        Set("Fire Spell Damage", stats.FireSpellDamage - 1);

        Set("Cold Spell Damage", stats.ColdSpellDamage - 1);

        Set("Lightning Spell Damage", stats.LightningSpellDamage - 1);

        Set("Poison Spell Damage", stats.PoisonSpellDamage - 1);

        Set("Shadow Spell Damage", stats.ShadowSpellDamage - 1);

        Set("Holy Spell Damage", stats.HolySpellDamage - 1);

        if (stats.MainHandEquipped && stats.MainHandItemClass.HasMeleeDamage())
        {
            //Title("Main Hand Weapon");
            Set("Main Hand Weapon Damage", stats.MainHandDamage);
            Set("Main Hand Weapon Speed", stats.MainHandAttacksPerSecond);
        }
        else
        {
            //HideTitle("Main Hand Weapon");
            Hide("Main Hand Weapon Damage");
            Hide("Main Hand Weapon Speed");
        }

        if (stats.OffHandEquipped && stats.OffHandItemClass.HasMeleeDamage())
        {
            //Title("Off Hand Weapon");
            Set("Off Hand Weapon Damage", stats.OffHandDamage);
            Set("Off Hand Weapon Speed", stats.OffHandAttacksPerSecond);
        }
        else
        {
            //HideTitle("Off Hand Weapon");
            Hide("Off Hand Weapon Damage");
            Hide("Off Hand Weapon Speed");
        }
    }

    private void Title(string name)
    {
        if (titles.TryGetValue(name, out StatsPanelTitle title))
        {
            title.gameObject.SetActive(true);
        }
        else
        {
            StatsPanelTitle newTitle = Instantiate(sectionTitlePrefab, transform);
            newTitle.text.text = name;
            titles.Add(name, newTitle);
        }
    }

    private void HideTitle(string name)
    {
        if (titles.TryGetValue(name, out StatsPanelTitle title))
        {
            title.gameObject.SetActive(false);
        }
    }

   

    private void Set(string name, string format, float value, string tooltipText = "", float height = 20)
    {
        if (dictionary.TryGetValue(name, out StatsPanelEntry entry))
        {
            entry.TooltipHover.tooltip = tooltip;
            entry.TooltipHover.text = tooltipText;
            entry.Format = format;
            entry.FloatValue = value;
            entry.gameObject.SetActive(true);
            (entry.transform as RectTransform).sizeDelta = new Vector2(0, height);
        }
        else
        {
            StatsPanelEntry newEntry = Instantiate(entryPrefab, transform);
            dictionary[name] = newEntry;
            newEntry.TooltipHover.tooltip = tooltip;
            newEntry.TooltipHover.text = tooltipText;
            newEntry.Name = name;
            newEntry.FloatValue = value;
            newEntry.Format = format;
            (newEntry.transform as RectTransform).sizeDelta = new Vector2(0, height);
        }
    }

    private void Set(string name, float value)
    {
        if (dictionary.TryGetValue(name, out StatsPanelEntry entry))
        {
            entry.FloatValue = value;
            entry.gameObject.SetActive(true);
        }
        else
        {
            StatsPanelEntry newEntry = Instantiate(entryPrefab, transform);
            newEntry.Name = name;
            newEntry.FloatValue = value;
            dictionary[name] = newEntry;
        }
    }

    private void Set(string name, string format, Vector2 value, string tooltipText, float height = 20)
    {
        if (dictionary.TryGetValue(name, out StatsPanelEntry entry))
        {
            entry.TooltipHover.tooltip = tooltip;
            entry.TooltipHover.text = tooltipText;
            entry.VectorValue = value;
            entry.Format = format;
            entry.gameObject.SetActive(true);
            (entry.transform as RectTransform).sizeDelta = new Vector2(0, height);
        }
        else
        {
            StatsPanelEntry newEntry = Instantiate(entryPrefab, transform);
            dictionary[name] = newEntry;
            newEntry.TooltipHover.tooltip = tooltip;
            newEntry.TooltipHover.text = tooltipText;
            newEntry.Name = name;
            newEntry.VectorValue = value;
            newEntry.Format = format;
            (newEntry.transform as RectTransform).sizeDelta = new Vector2(0, height);
        }
    }

    public void Set(string name, Vector2 value)
    {
        if (dictionary.TryGetValue(name, out StatsPanelEntry entry))
        {
            entry.VectorValue = value;
            entry.gameObject.SetActive(true);
        }
        else
        {
            StatsPanelEntry newEntry = Instantiate(entryPrefab, transform);
            newEntry.Name = name;
            newEntry.VectorValue = value;
            dictionary[name] = newEntry;
        }
    }

    private void Hide(string name)
    {
        if (dictionary.TryGetValue(name, out StatsPanelEntry entry))
        {
            entry.gameObject.SetActive(false);
        }
    }

    public void Remove(string key)
    {
        if (dictionary.TryGetValue(key, out StatsPanelEntry existing))
        {
            dictionary.Remove(key);
            DestroyImmediate(existing.gameObject);
        }
        else
        {
            dictionary.Remove(key);
        }
    }

    public void Clear()
    {
        dictionary.Clear();
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);
    }
}
