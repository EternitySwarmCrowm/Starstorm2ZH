﻿using RoR2;
using RoR2.Items;
using UnityEngine.Networking;
namespace SS2.Items
{
    //TODO: move to ethereal equipment class.
   /* public sealed class EtherealItemAffix : SS2Item
    {
        public override ItemDef ItemDef { get; } = SS2Assets.LoadAsset<ItemDef>("EtherealItemAffix", SS2Bundle.Equipments);

        public override void Initialize()
        {

        }
        public sealed class BodyBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation]
            private static ItemDef GetItemDef() => SS2Content.Items.EtherealItemAffix;

            public void Start()
            {
                if (NetworkServer.active)
                {
                    body.AddBuff(SS2Content.Buffs.bdEthereal);
                }
            }

            private void OnDestroy()
            {
                if (NetworkServer.active && body.enabled)
                {
                    body.RemoveBuff(SS2Content.Buffs.bdEthereal);
                }
            }
        }
    }*/
}