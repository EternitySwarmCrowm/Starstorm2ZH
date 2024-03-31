﻿using RoR2;
using UnityEngine;
using Moonstorm;
using System.Collections.Generic;

using Moonstorm;
namespace SS2.Equipments
{
    [DisabledContent]
    public class AffixPurple : EliteEquipmentBase
    {
        public override EquipmentDef EquipmentDef { get; } = SS2Assets.LoadAsset<EquipmentDef>("ElitePurpleEquipment", SS2Bundle.Indev);

        public override List<MSEliteDef> EliteDefs { get; } = new List<MSEliteDef>
        {
            SS2Assets.LoadAsset<MSEliteDef>("edPurple", SS2Bundle.Indev),
            SS2Assets.LoadAsset<MSEliteDef>("edPurpleHonor", SS2Bundle.Indev)
        };

        public override bool FireAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}
