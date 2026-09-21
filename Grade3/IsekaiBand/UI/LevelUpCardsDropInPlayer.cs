using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    public class LevelUpCardsDropInPlayer : MonoBehaviour
    {
        private const string CardClassName = "card";
        private const string InClassName = "card--in";

        [SerializeField] private UIDocument document;
        [SerializeField, Min(0)] private int delayMs = 30;

        private void Awake()
        {
            if (document == null)
                document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            ResetCards();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
                Play();
        }

        public void Play()
        {
            if (document == null)
                return;

            VisualElement root = document.rootVisualElement;
            if (root == null)
                return;

            ResetCards();
            root.schedule.Execute(AddInClass).StartingIn(delayMs);
        }

        private void ResetCards()
        {
            if (document == null)
                return;

            VisualElement root = document.rootVisualElement;
            if (root == null)
                return;

            root.Query<VisualElement>(className: CardClassName)
                .ForEach(card => card.RemoveFromClassList(InClassName));
        }

        private void AddInClass()
        {
            if (document == null)
                return;

            VisualElement root = document.rootVisualElement;
            if (root == null)
                return;

            root.Query<VisualElement>(className: CardClassName)
                .ForEach(card => card.AddToClassList(InClassName));
        }
    }
}
