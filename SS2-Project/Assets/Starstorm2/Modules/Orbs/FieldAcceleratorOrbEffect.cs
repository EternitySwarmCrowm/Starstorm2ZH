﻿using RoR2;
using RoR2.Orbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SS2.Items.FieldAccelerator;

using Moonstorm;
namespace SS2.Orbs
{
    public class FieldAcceleratorOrbEffect : OrbEffect
    {
        private void Start()
        {
            base.Start();
            var teleInstance = TeleporterInteraction.instance;
            if (teleInstance != null)
            {
                var fatb = teleInstance.GetComponent<FieldAcceleratorTeleporterBehavior>();

                if (fatb != null)
                    targetTransform = fatb.displayInstance.transform;
                else
                    targetTransform = teleInstance.transform;
            }
        }
    }
}
