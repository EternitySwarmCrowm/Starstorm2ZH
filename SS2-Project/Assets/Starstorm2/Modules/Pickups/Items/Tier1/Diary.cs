﻿using RoR2;
using System.Collections.Generic;
using UnityEngine;
namespace SS2.Items
{
    public sealed class Diary : ItemBase
    {
        private const string token = "SS2_ITEM_DIARY_DESC";
        public override ItemDef ItemDef { get; } = SS2Assets.LoadAsset<ItemDef>("Diary", SS2Bundle.Items);

        [RooConfigurableField(SS2Config.ID_ITEM, ConfigDesc = "Number of levels gained when Empty Diary is consumed.")]
        [TokenModifier(token, StatTypes.Default, 0)]
        public static int extraLevels = 3;
        public sealed class Behavior : BaseItemMasterBehavior
        {
            //plays the diary sfx only if players can see the experience bar being affected
            public static void PlayDiarySFXLocal(GameObject holderBodyobject)
            {
                if (!holderBodyobject)
                {
                    return;
                }
                List<UICamera> uICameras = UICamera.instancesList;
                for(int i = 0; i < uICameras.Count; i++)
                {
                    UICamera uICamera = uICameras[i];
                    CameraRigController cameraRigController = uICamera.cameraRigController;
                    if(cameraRigController && cameraRigController.viewer && cameraRigController.viewer.hasAuthority && cameraRigController.target == holderBodyobject)
                    {
                        Util.PlaySound("DiaryLevelUp", holderBodyobject);
                        return;
                    }
                }
            }
            [ItemDefAssociation(useOnClient = false, useOnServer = true)]
            private static ItemDef GetItemDef() => SS2Content.Items.Diary;
            private void Awake()
            {
                Stage.onServerStageBegin += PrepareAddLevel;
            }
            private void OnDestroy()
            {
                Stage.onServerStageBegin -= PrepareAddLevel;
            }
            private void PrepareAddLevel(Stage stage)
            {
                if (stage.sceneDef)
                    base.GetComponent<CharacterMaster>().onBodyStart += AddLevel;
            }

            private void AddLevel(CharacterBody body)
            {
                ItemIndex diary = SS2Content.Items.Diary.itemIndex;
                ItemIndex consumed = SS2Content.Items.DiaryConsumed.itemIndex;
                if (body.inventory.GetItemCount(diary) > 0)
                {
                    // this uses xp orbs. wouldnt mind setting the level manually if we use our own vfx
                    ulong requiredExperience = TeamManager.GetExperienceForLevel(TeamManager.instance.GetTeamLevel(body.teamComponent.teamIndex) + (uint)extraLevels);
                    ulong experience = requiredExperience - TeamManager.instance.GetTeamCurrentLevelExperience(body.teamComponent.teamIndex);
                    ExperienceManager.instance.AwardExperience(body.transform.position, body, experience);

                    body.inventory.RemoveItem(diary);
                    body.inventory.GiveItem(consumed);
                    CharacterMasterNotificationQueue.SendTransformNotification(body.master, diary, consumed, CharacterMasterNotificationQueue.TransformationType.Default);
                    PlayDiarySFXLocal(body.gameObject);   // vfx would be nice             
                }

                body.master.onBodyStart -= AddLevel;
            }

        }
    }
}
