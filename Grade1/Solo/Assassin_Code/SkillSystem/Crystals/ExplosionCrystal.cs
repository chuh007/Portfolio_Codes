using System;
using DG.Tweening;
using UnityEngine;

namespace Code.SkillSystem.Crystals
{
    public class ExplosionCrystal : Crystal
    {
        public Action OnTimeOut;
        
        public override void TriggerCrystal()
        {
            transform.DOScale(Vector3.one * 2.5f, 0.03f);
            Explosion();
        }
    }
}