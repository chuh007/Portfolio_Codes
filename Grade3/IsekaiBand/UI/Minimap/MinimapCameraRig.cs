using _Code.LCH._02.Scripts.Player;
using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal static class MinimapCameraRig
    {
        public static Transform ResolveTarget(Transform target, bool autoFindPlayer)
        {
            if (target != null || !autoFindPlayer) return target;

            Player player = Object.FindAnyObjectByType<Player>();
            if (player != null)
                return player.transform;

            return target;
        }

        public static Camera ResolveCamera(Component owner, Camera minimapCamera, bool createCameraIfMissing)
        {
            if (minimapCamera != null) return minimapCamera;

            minimapCamera = owner.GetComponent<Camera>();
            if (minimapCamera != null || !createCameraIfMissing) return minimapCamera;

            GameObject cameraObject = new GameObject("Minimap Camera");
            cameraObject.transform.SetParent(owner.transform, false);
            return cameraObject.AddComponent<Camera>();
        }

        public static void Apply(Camera minimapCamera, float viewRadius, Color backgroundColor,
            LayerMask cullingMask, RenderTexture renderTexture)
        {
            if (minimapCamera == null) return;

            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = viewRadius;
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = backgroundColor;
            minimapCamera.cullingMask = cullingMask;
            minimapCamera.targetTexture = renderTexture;
            minimapCamera.enabled = renderTexture != null;
        }

        public static void Follow(Camera minimapCamera, Transform target, float cameraZ, Vector3 cameraEulerAngles)
        {
            if (minimapCamera == null || target == null) return;

            Vector3 targetPosition = target.position;
            minimapCamera.transform.SetPositionAndRotation(
                new Vector3(targetPosition.x, targetPosition.y, cameraZ),
                Quaternion.Euler(cameraEulerAngles));
        }
    }
}
