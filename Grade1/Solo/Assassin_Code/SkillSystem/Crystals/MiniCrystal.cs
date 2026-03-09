using System;
using Code.Entities;
using DG.Tweening;
using UnityEngine;

namespace Code.SkillSystem.Crystals
{
    public class MiniCrystal : Crystal
    {
        [SerializeField] private float moveSpeed = 20f;
        private bool _isLaunched = false;
        private MultiCrystalController _multiCrystalController;
        
        public override void SetUp(float damageStat, CrystalSkill skill, CrystalController controller, Entity owner)
        {
            base.SetUp(damageStat, skill, controller, owner);
            _multiCrystalController = controller as MultiCrystalController;
            StartPulseMove();
        }

        private void StartPulseMove()
        {
            Vector3 position = transform.localPosition;
            transform.DOLocalMoveY(position.y + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        private void Update()
        {
            if (_isLaunched == false) return; //발사하지 않은 녀석들은 update 필요 없어.
            if (_canExplode == false) return; //이미 폭발함

            if (_multiCrystalController.ActiveTarget == null)
            {
                Explosion();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position,
                                    _multiCrystalController.ActiveTarget.position,
                                    moveSpeed * Time.deltaTime);
            
            if(Vector2.Distance(transform.position, _multiCrystalController.ActiveTarget.position) < 1f)
                Explosion();

        }

        public override void TriggerCrystal()
        {
            if (_multiCrystalController.ActiveTarget == null)
            {
                Explosion();
                return;
            }

            transform.DOKill(); //펄스 무빙 하던 트윈 제거
            transform.DOLocalMoveY(2f, 0.3f).OnComplete(() =>
            {
                _isLaunched = true;
                transform.SetParent(null);
            });

        }
    }
}