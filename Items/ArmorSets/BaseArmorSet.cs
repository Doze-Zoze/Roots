using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    /// <summary>
    /// Overrides the entire stats of a given armor set and allows setting your own.
    /// </summary>
    public abstract class BaseArmorSet : GlobalItem
    {
        public abstract string SetID { get; }
        public virtual List<int> HeadsToApplyTo { get; } = [];
        public virtual List<int> ChestsToApplyTo { get; } = [];
        public virtual List<int> LegsToApplyTo { get; } = [];

        public virtual ArmorID SetBonusTiedTo { get; } = ArmorID.Head;

        public enum ArmorID
        {
            Head,
            Chest,
            Legs
        }
        public virtual void HeadDefaults(Item item) { }
        public virtual void ChestDefaults(Item item) { }
        public virtual void LegsDefaults(Item item) { }

        public virtual void HeadEquips(Item item, Player player) { }
        public virtual void ChestEquips(Item item, Player player) { }
        public virtual void LegsEquips(Item item, Player player) { }

        public virtual void SetBonusEffect(Player player) { }
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;

        public override bool AppliesToEntity(Item item, bool lateInstantiation) => HeadsToApplyTo.Contains(item.type) || ChestsToApplyTo.Contains(item.type) || LegsToApplyTo.Contains(item.type);

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            foreach (var item in HeadsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                if (SetBonusTiedTo == ArmorID.Head)
                    ItemSets.DontUseVanillaSetBonus[item] = true;
            }
            foreach (var item in ChestsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                if (SetBonusTiedTo == ArmorID.Chest)
                    ItemSets.DontUseVanillaSetBonus[item] = true;
            }
            foreach (var item in LegsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                if (SetBonusTiedTo == ArmorID.Legs)
                    ItemSets.DontUseVanillaSetBonus[item] = true;
            }
        }

        public override void SetDefaults(Item item)
        {
            if (HeadsToApplyTo.Contains(item.type))
                HeadDefaults(item);
            if (ChestsToApplyTo.Contains(item.type))
                ChestDefaults(item);
            if (LegsToApplyTo.Contains(item.type))
                LegsDefaults(item);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (HeadsToApplyTo.Contains(item.type))
                HeadEquips(item, player);
            if (ChestsToApplyTo.Contains(item.type))
                ChestEquips(item, player);
            if (LegsToApplyTo.Contains(item.type))
                LegsEquips(item, player);
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if ((HeadsToApplyTo.Count == 0 || HeadsToApplyTo.Contains(head.type)) && (ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) && (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)))
                return SetID + "Set";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == SetID + "Set")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonus");
                SetBonusEffect(player);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (HeadsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.HelmetTooltip");
            else if (ChestsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.ChestplateTooltip");
            else if (LegsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.LeggingsTooltip");
        }

    }
}
