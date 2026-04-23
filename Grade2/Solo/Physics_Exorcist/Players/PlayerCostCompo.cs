using _01Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _01Scripts.Players
{
    public class PlayerCostCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image[] costImg;
        private Player _player;
        private int _cost;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            UpdateCost(4);
        }

        public void PlusCost(int cost)
        {
            if(_cost >= 10) return;
            _cost += cost;
            UpdateCost(_cost);
        }
        
        public bool TrySpendCost(int cost)
        {
            if (cost <= _cost) return true;
            return false;
        }
        
        public void SpendCost(int cost)
        {
            _cost -= cost;
            UpdateCost(_cost);
        }
        
        private void UpdateCost(int currentCost)
        {
            costText.text = currentCost.ToString();
            _cost = currentCost;
            for (int i = 0; i < _cost; i++)
            {
                costImg[i].color = Color.cyan;
            }
            for (int i = _cost; i < costImg.Length; i++)
            {
                costImg[i].color = Color.gray;
            }
        }
    }
}