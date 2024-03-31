﻿using R2API;
using RoR2;
using RoR2.Items;

using Moonstorm;
namespace SS2.Items
{
    public sealed class NemesisBossHelper : ItemBase
    {
        public override ItemDef ItemDef { get; } = SS2Assets.LoadAsset<ItemDef>("NemBossHelper", SS2Bundle.Items);

        public sealed class Behavior : BaseItemBodyBehavior, IBodyStatArgModifier
        {
            [ItemDefAssociation]
            private static ItemDef GetItemDef() => SS2Content.Items.NemBossHelper;
            public void ModifyStatArguments(RecalculateStatsAPI.StatHookEventArgs args)
            {
                args.healthMultAdd += 12;
                args.damageMultAdd += 0.75f;
                body.regen = 0;                     
            }                                       
        }                                           
    }  
}
