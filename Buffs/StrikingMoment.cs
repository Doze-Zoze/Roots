using Roots.Config;
using RootsBeta.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Buffs
{
    public class StrikingMoment : GlobalBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => RootsModConfig.Instance.RemoveClasses;

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type != BuffID.ParryDamageBuff) return;
            tip = Language.GetTextValue("Mods.RootsBeta.Buffs.StrikingMoment.Tooltip");
        }

        public override void Update(int type, Player player, ref int buffIndex)
        {
            player.Roots().PhysicalModifyHitNPCFuncs.Add((plr,npc,modifiers) =>
            {
                if (!plr.HasBuff(BuffID.ParryDamageBuff))
                    return modifiers;
                modifiers.SourceDamage *= 5;
                plr.ClearBuff(BuffID.ParryDamageBuff);
                return modifiers;
            });
        }
    }
}
