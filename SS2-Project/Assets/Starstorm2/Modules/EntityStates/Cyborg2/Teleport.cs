﻿using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using SS2.Components;
using System.Collections.Generic;
namespace EntityStates.Cyborg2
{
    public class Teleport : BaseSkillState
    {
        public static float exitHopVelocity = 12f;
        public static float baseDuration = 0.33f;
        private float duration;
        private CameraMover camera;
        private TeleporterProjectile.ProjectileTeleporterOwnership teleporterOwnership;
        private Vector3 teleportTarget;

        private static float exitVelocityCoefficient = 1.25f;
        private static float damageRadius = 10f;
        private static float damageCoefficient = 3f;
        private Vector3 storedVelocity;
        private bool didTeleport;
        private Vector3 lastPositionBeforeTeleport;
        public override void OnEnter()
        {
            base.OnEnter();

            this.teleporterOwnership = base.GetComponent<TeleporterProjectile.ProjectileTeleporterOwnership>();
            if(!this.teleporterOwnership)
            {
                Debug.LogError("Cyborg2 teleport failed! No teleporter component. (howw?????)");
                this.outer.SetNextStateToMain();
                return;
            }

            this.teleportTarget = this.teleporterOwnership.teleporter.GetSafeTeleportPosition();

            this.duration = baseDuration; //movespeed? attackspeed?

            this.camera = base.gameObject.AddComponent<CameraMover>();
            this.camera.lerpTime = this.duration;
            this.camera.position = this.teleportTarget;

            this.storedVelocity = base.characterMotor.velocity;

            if (NetworkServer.active)
            {
                
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        private void UpdateTarget()
        {
            if(this.teleporterOwnership && this.teleporterOwnership.teleporter)
            {
                this.teleportTarget = this.teleporterOwnership.teleporter.GetSafeTeleportPosition();
                if(this.camera)
                    this.camera.position = this.teleportTarget;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.UpdateTarget();

            base.characterMotor.velocity = Vector3.zero;

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                //for some reason its 0,0,0 on the same frame the teleporter disappears
                if(this.teleportTarget != Vector3.zero)
                {
                    didTeleport = true;
                    lastPositionBeforeTeleport = base.characterBody.corePosition;
                    TeleportHelper.TeleportBody(base.characterBody, this.teleportTarget);
                    base.characterDirection.forward = base.GetAimRay().direction;


                    base.SmallHop(base.characterMotor, Teleport.exitHopVelocity);
                }             
                this.outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();           
            if (didTeleport)
            {
                base.characterMotor.ApplyForce(storedVelocity * exitVelocityCoefficient);
            }
            if(this.teleporterOwnership)
            {
                this.teleporterOwnership.DoTeleport();
                if (NetworkServer.active)
                {
                    TeleporterOrb teleporterOrb = new TeleporterOrb();
                    teleporterOrb.origin = lastPositionBeforeTeleport;
                    teleporterOrb.totalDuration = this.duration;
                    teleporterOrb.attacker = base.gameObject;
                    teleporterOrb.inflictor = base.gameObject;
                    teleporterOrb.damageValue = characterBody.damage * damageCoefficient;
                    teleporterOrb.isCrit = base.RollCrit();
                    teleporterOrb.targetObjects = teleporterOwnership.teleporter.GetTargets();
                    RoR2.Orbs.OrbManager.instance.AddOrb(teleporterOrb);
                }
            }
            if (this.camera)
            {
                Destroy(this.camera);
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        //move camera to position. keep player camera control
        public class CameraMover : MonoBehaviour, ICameraStateProvider
        {
            public static float endLerpTime = 0.2f;
            public float lerpTime;
            public Vector3 position;

            private void OnEnable()
            {
                foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                {
                    if (cameraRigController.target == base.gameObject)
                    {
                        cameraRigController.SetOverrideCam(this, this.lerpTime);
                    }
                    else if (cameraRigController.IsOverrideCam(this))
                    {
                        cameraRigController.SetOverrideCam(null, 0.05f);
                    }
                }
            }
            private void OnDisable()
            {
                foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                {
                    if (cameraRigController.target == base.gameObject)
                    {
                        cameraRigController.SetOverrideCam(null, endLerpTime);
                    }
                    else if (cameraRigController.IsOverrideCam(this))
                    {
                        cameraRigController.SetOverrideCam(null, 0.05f);
                    }
                }
            }
            public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
            {
                Vector3 position = this.position;
                Vector3 cameraLocalPos = cameraRigController.targetParams.currentCameraParamsData.idealLocalCameraPos.value;
                cameraLocalPos.y += cameraRigController.targetParams.currentCameraParamsData.pivotVerticalOffset.value + 0.9f;
                cameraLocalPos += (cameraRigController.targetParams.cameraPivotTransform ? cameraRigController.targetParams.cameraPivotTransform.localPosition : Vector3.zero);
                position += cameraState.rotation * cameraLocalPos;
                cameraState.position = position;
            }

            public bool IsHudAllowed(CameraRigController cameraRigController)
            {
                return true;
            }

            public bool IsUserControlAllowed(CameraRigController cameraRigController)
            {
                return true;
            }

            public bool IsUserLookAllowed(CameraRigController cameraRigController)
            {
                return true;
            }
        }
    }
}
