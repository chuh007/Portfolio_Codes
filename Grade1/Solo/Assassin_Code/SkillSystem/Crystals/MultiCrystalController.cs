using System;
using System.Collections;
using System.Collections.Generic;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem.Crystals
{
    public class MultiCrystalController : CrystalController
    {
        [SerializeField] private GameObject crystalPrefab;
        public List<Crystal> crystalList;

        [SerializeField] private float timerMultiplier = 10f;
        [SerializeField] private float positionOffset = 1.2f;
        [SerializeField] private float findEnemyRadius = 15f;
        [SerializeField] private int amountOfCrystals = 3;

        //이부분은 나중에 SO등으로 개선할 수 있음.
        [SerializeField] private Vector2[] offsets =
        {
            new Vector2(0.3f, 0.3f), new Vector2(-0.3f, 0.3f), new Vector2(0.3f, -0.3f),
            new Vector2(-0.3f, -0.3f), Vector2.zero
        };

        public Transform ActiveTarget { get; private set; } = null;
        public bool ReadyToLaunch { get; private set; }
        private float _timer;

        public override void CreateCrystal()
        {
            StartCoroutine(MakeMultipleCryStal());
        }

        private IEnumerator MakeMultipleCryStal()
        {
            //플레이어의 뒤쪽에 offset만큼 떨어뜨려서 생성한다.
            Vector3 position = _owner.transform.position + _owner.transform.right * -positionOffset;

            WaitForSeconds wait = new WaitForSeconds(0.1f);
            for (int i = 0; i < amountOfCrystals; i++)
            {
                Crystal crystal = Instantiate(crystalPrefab, position + (Vector3)offsets[i], Quaternion.identity)
                                    .GetComponent<Crystal>();
                crystal.transform.localScale = Vector3.one * 0.5f; //절반크기로 축소
                
                crystal.transform.SetParent(_owner.transform); //부모를 지정해서 따라다니게 
                crystal.SetUp(_damageStatValue, skill, this, _owner);
                crystalList.Add(crystal);

                yield return wait;
            }

            ReadyToLaunch = true;
            _timer = skill.timeOut * timerMultiplier; //뒤를 따라다니는 애들은 라이프타임이 더 길어야 해서
            SetCrystalStatus(true); //크리스탈 준비 완료
        }

        private void Update()
        {
            if (crystalList.Count > 0 && ReadyToLaunch)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    crystalList.ForEach(crystal => crystal.TriggerCrystal());
                    crystalList.Clear();
                    SetCrystalStatus(false);
                }
            }
        }

        public override void UseCrystal()
        {
            if (!ReadyToLaunch) return;

            //스킬의 사거리 내의 적을 찾는다.
            Transform target = skill.FindClosestTarget(_owner.transform, findEnemyRadius);
            if (target != null)
            {
                ActiveTarget = target;
                crystalList[^1].TriggerCrystal();
                crystalList.RemoveAt(crystalList.Count - 1);

                if (crystalList.Count <= 0)
                {
                    SetCrystalStatus(false);
                    ReadyToLaunch = false;
                }
            }
        }
    }
}