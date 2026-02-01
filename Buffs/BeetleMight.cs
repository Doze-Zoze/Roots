using Microsoft.Xna.Framework;
using RootsBeta.Players;
using RootsBeta.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class BeetleMight : GlobalBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type >= BuffID.BeetleMight1 && type <= BuffID.BeetleMight3)
            {

                player.GetDamage<GenericDamageClass>() += 0.1f * player.beetleOrbs;
                player.GetAttackSpeed<GenericDamageClass>() += 0.1f * player.beetleOrbs;
                //cancel the vanilla buff so it doesn't double stack for other mods lol
                player.GetDamage<MeleeDamageClass>() -= 0.1f * player.beetleOrbs;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f * player.beetleOrbs;
            }
        }
    }
}
