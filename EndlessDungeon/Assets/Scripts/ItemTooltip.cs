using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField]
    private new Text name;
    [SerializeField]
    private Text type;
    [SerializeField]
    private Text dps;
    [SerializeField]
    private Text damage;
    [SerializeField]
    private Text speed;
    [SerializeField]
    private Text armour;
    [SerializeField]
    private Text block;
    [SerializeField]
    private Text durability;
    [SerializeField]
    private Text stats;
    [SerializeField]
    private Text requirements;
    [SerializeField]
    private Text description;

    private ItemDisplay display;
    private CanvasGroup canvasGroup;
    private Coroutine anim;
    private bool shown;

    public float fadeInDuration;
    public float fadeOutDuration;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    IEnumerator FadeIn()
    {
        for (float i = canvasGroup.alpha; i < 1; i += Time.deltaTime / fadeInDuration)
        {
            canvasGroup.alpha = i;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        for (float i = canvasGroup.alpha; i >= 0; i -= Time.deltaTime / fadeOutDuration)
        {
            canvasGroup.alpha = i;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    public void SetVisible(bool show)
    {
        if (show == shown)
            return;
        shown = show;

        if (anim != null) StopCoroutine(anim);

        if (show)
        {
            if (fadeInDuration > 0)
                anim = StartCoroutine(FadeIn());
            else
                canvasGroup.alpha = 1;
        }
        else
        {
            if (fadeOutDuration > 0)
                anim = StartCoroutine(FadeOut());
            else
                canvasGroup.alpha = 0;
        }
    }

    public void Inspect(ItemDisplay display)
    {
        transform.position = display.transform.position;

        if (this.display == display)
            return;

        this.display = display;

        if (display.Item is EquipmentObject)
        {
            EquipmentObject equipment = display.Item as EquipmentObject;

            name.color = equipment.ItemQualityColor;
            name.text = equipment.DisplayName;

            type.color = equipment.ItemQualityColor;
            type.text = equipment.DescriptiveName + " (" + equipment.ItemType.ToString().SplitCamelCase() + ")";
            type.gameObject.SetActive(true);

            if (equipment.ItemClass.HasDamage())
            {
                dps.text = equipment.DamagePerSecond.ToString("0.0") + " DPS";
                damage.text = equipment.Damage.Min().ToString("0") + "-" + equipment.Damage.Max().ToString("0") + " Damage";

                dps.gameObject.SetActive(true);
                damage.gameObject.SetActive(true);
            }
            else
            {
                dps.gameObject.SetActive(false);
                damage.gameObject.SetActive(false);
            }

            if (equipment.ItemClass.HasSpeed())
            {
                if (equipment.ItemClass.HasDamage() && equipment.ItemClass.HasBlock())
                    speed.text = equipment.AttacksPerSecond.ToString("0.00") + " Attacks/Blocks per Second";
                else if (equipment.ItemClass.HasDamage())
                    speed.text = equipment.AttacksPerSecond.ToString("0.00") + " Attacks per Second";
                else if (equipment.ItemClass.HasBlock())
                    speed.text = equipment.AttacksPerSecond.ToString("0.00") + " Blocks per Second";
                else
                    speed.text = equipment.AttacksPerSecond.ToString("0.00") + " Actions per Second";
                speed.gameObject.SetActive(true);
            }
            else
            {
                speed.gameObject.SetActive(false);
            }

            if (equipment.ItemClass.HasArmour())
            {
                armour.text = equipment.Armour + " Armour";
                armour.gameObject.SetActive(true);
            }
            else
            {
                armour.gameObject.SetActive(false);
            }

            if (equipment.ItemClass.HasBlock())
            {
                block.text = equipment.Block.ToString("0.#%") + " Chance to Block";
                block.gameObject.SetActive(true);
            }
            else
            {
                block.gameObject.SetActive(false);
            }

            if (equipment.DisplayQuantity)
            {
                if (equipment.Indestructible)
                    durability.text = "Indestructible";
                else if (equipment.Quantity > 0)
                    durability.text = equipment.Quantity + "/" + equipment.QuantityMax + " Durability";
                else
                    durability.text = "<color=red>" + equipment.Quantity + "/" + equipment.QuantityMax + " Durability</color>";
                durability.gameObject.SetActive(true);
            }
            else
            {
                durability.gameObject.SetActive(false);
            }

            if (equipment.StatsLength > 0)
            {
                string text = "";
                for (int i = 0; i < equipment.StatsLength; i++)
                {
                    ItemStat stat = equipment.Stat(i);
                    text += "\n" + stat.ToString();
                }
                stats.text = text;
                stats.gameObject.SetActive(true);
            }
            else
            {
                stats.gameObject.SetActive(false);
            }

            if (equipment.RequiresLevel > 1 || equipment.RequiresStrength + equipment.RequiresDexterity + equipment.RequiresMagic > 0)
            {

                string text = "";
                if (equipment.RequiresLevel > 1)
                {
                    text += "\n";
                    if (equipment.RequiresLevel > Player.Instance.CharacterLevel)
                        text += "<color=red>Requires Level: " + equipment.RequiresLevel + "</color>";
                    else
                        text += "Requires Level: " + equipment.RequiresLevel;
                }

                if (equipment.RequiresStrength > 0)
                {
                    text += "\n";
                    if (equipment.RequiresStrength > Player.Instance.Stats.Strength)
                        text += "<color=red>Requires Strength: " + equipment.RequiresStrength + "</color>";
                    else
                        text += "Requires Strength: " + equipment.RequiresStrength;
                }

                if (equipment.RequiresDexterity > 0)
                {
                    text += "\n";
                    if (equipment.RequiresDexterity > Player.Instance.Stats.Dexterity)
                        text += "<color=red>Requires Dexterity: " + equipment.RequiresDexterity + "</color>";
                    else
                        text += "Requires Dexterity: " + equipment.RequiresDexterity;
                }

                if (equipment.RequiresMagic > 0)
                {
                    text += "\n";
                    if (equipment.RequiresMagic > Player.Instance.Stats.Magic)
                        text += "<color=red>Requires Magic: " + equipment.RequiresMagic + "</color>";
                    else
                        text += "Requires Magic: " + equipment.RequiresMagic;
                }

                requirements.text = text;
                requirements.gameObject.SetActive(true);
            }
            else
            {
                requirements.gameObject.SetActive(false);
            }

            if (equipment.Description.Length > 0)
            {
                description.text = "\n" + equipment.Description;
                description.gameObject.SetActive(true);
            }
            else
            {
                description.gameObject.SetActive(false);
            }
        }
        else
        {

            ItemObject item = display.Item;

            name.color = item.ItemQualityColor;
            name.text = item.DisplayName;

            type.gameObject.SetActive(false);
            dps.gameObject.SetActive(false);
            damage.gameObject.SetActive(false);
            speed.gameObject.SetActive(false);
            armour.gameObject.SetActive(false);
            block.gameObject.SetActive(false);
            durability.gameObject.SetActive(false);
            stats.gameObject.SetActive(false);
            requirements.gameObject.SetActive(false);

            if (item.Description.Length > 0)
            {
                description.text = "\n" + item.Description;
                description.gameObject.SetActive(true);
            }
            else
            {
                description.gameObject.SetActive(false);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
