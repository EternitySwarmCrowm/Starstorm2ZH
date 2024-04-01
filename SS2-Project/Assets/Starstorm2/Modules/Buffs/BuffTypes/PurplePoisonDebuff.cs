﻿using R2API;
using RoR2;
namespace SS2.Buffs
{
    public sealed class PurplePoisonDebuff : BuffBase
    {
        // TODO
        public override BuffDef BuffDef { get; } = SS2Assets.LoadAsset<BuffDef>("bdPurplePoison", SS2Bundle.Indev);
        public static DotController.DotIndex index;

        public override void Initialize()
        {
            index = DotAPI.RegisterDotDef(0.25f, 0.18f, DamageColorIndex.DeathMark, BuffDef);
        }

        public sealed class Behavior : BaseBuffBodyBehavior
        {
            [BuffDefAssociation]
            private static BuffDef GetBuffDef() => SS2Content.Buffs.bdPurplePoison;
        }
    }
}
