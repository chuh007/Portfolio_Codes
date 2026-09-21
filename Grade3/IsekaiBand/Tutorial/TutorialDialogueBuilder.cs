using _Code.LCH._02.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialDialogueBuilder
    {

        private static readonly Color InkColor = new(0.22f, 0.12f, 0.065f, 1f);

        public static TutorialDialogueElements Build(
            Transform parent, TMP_FontAsset fontAsset, TMP_FontAsset buttonFontAsset,
            Sprite settingsFrameSprite, Sprite guidePortrait, string guideName)
        {
            var view = new TutorialDialogueElements();
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer < 0)
                uiLayer = parent.gameObject.layer;

            view.CanvasRoot = new GameObject(
                "TutorialGuideCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            view.CanvasRoot.layer = uiLayer;
            view.CanvasRoot.transform.SetParent(parent, false);

            Canvas canvas = view.CanvasRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 220;

            CanvasScaler scaler = view.CanvasRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            view.Blocker = RuntimeUGuiFactory.CreateImageObject(
                "InputBlocker",
                view.CanvasRoot.transform,
                new Color(0f, 0f, 0f, 0.46f),
                uiLayer);
            RuntimeUGuiFactory.StretchToParent((RectTransform)view.Blocker.transform);

            GameObject fullBody = RuntimeUGuiFactory.CreateImageObject(
                "DialogueFullBody",
                view.CanvasRoot.transform,
                Color.white,
                uiLayer);
            view.FullBodyRect = (RectTransform)fullBody.transform;
            view.FullBodyImage = fullBody.GetComponent<Image>();
            view.FullBodyImage.preserveAspect = true;
            view.FullBodyImage.raycastTarget = false;

            GameObject panel = RuntimeUGuiFactory.CreateImageObject(
                "DialoguePanel",
                view.CanvasRoot.transform,
                Color.white,
                uiLayer);
            view.PanelRect = (RectTransform)panel.transform;
            view.PanelGroup = panel.AddComponent<CanvasGroup>();
            Image panelImage = panel.GetComponent<Image>();
            RuntimeUGuiFactory.ApplySprite(
                panelImage,
                settingsFrameSprite,
                Image.Type.Sliced,
                pixelsPerUnitMultiplier: 5f);
            panelImage.raycastTarget = false;

            GameObject portrait = RuntimeUGuiFactory.CreateImageObject(
                "GuidePortrait",
                panel.transform,
                Color.white,
                uiLayer);
            view.PortraitRect = (RectTransform)portrait.transform;
            view.PortraitImage = portrait.GetComponent<Image>();
            view.PortraitImage.sprite = guidePortrait;
            view.PortraitImage.preserveAspect = true;
            view.PortraitImage.raycastTarget = false;

            view.NameLabel = RuntimeUGuiFactory.CreateLabel(
                "GuideName",
                panel.transform,
                guideName,
                fontAsset,
                25f,
                FontStyles.Bold,
                TextAlignmentOptions.Left,
                uiLayer);
            view.NameLabel.color = InkColor;

            view.MessageLabel = RuntimeUGuiFactory.CreateLabel(
                "Message",
                panel.transform,
                string.Empty,
                fontAsset,
                25f,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft,
                uiLayer);
            view.MessageLabel.color = InkColor;

            view.ContinueButton = RuntimeUGuiFactory.CreateButton(
                "ContinueButton",
                panel.transform,
                "다음",
                buttonFontAsset != null ? buttonFontAsset : fontAsset,
                settingsFrameSprite,
                Color.white,
                InkColor,
                21f,
                uiLayer);
            view.ContinueLabel = view.ContinueButton.GetComponentInChildren<TextMeshProUGUI>();


            return view;
        }
    }
}
