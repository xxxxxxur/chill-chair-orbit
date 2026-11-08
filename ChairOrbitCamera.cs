using UnityEngine;
using UnityEngine.Rendering;

namespace ChillChairOrbit
{
    internal sealed class ChairOrbitCamera : MonoBehaviour
    {
        private static readonly Vector3 Pivot = new Vector3(2.73f, 0.69f, 1.684f);
        private const float DegreesPerPixel = 0.16f;
        private const float MaxYaw = 35f;
        private const float MaxPitch = 18f;

        private Camera roomCamera;
        private Vector2 previousMouse;
        private bool dragging;
        private float yaw;
        private float pitch;
        private bool applied;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool hasGamePose;
        private Vector3 previousGamePosition;
        private Quaternion previousGameRotation;

        private void Awake() => roomCamera = GetComponent<Camera>();

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += BeginCamera;
            RenderPipelineManager.endCameraRendering += EndCamera;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCamera;
            RenderPipelineManager.endCameraRendering -= EndCamera;
            RestorePose();
            hasGamePose = false;
            ResetOrbit();
        }

        private void Update()
        {
            bool held = MiddleMouse.TryRead(out Vector2 mouse);
            ReadDrag(mouse, held);
        }

        private void ReadDrag(Vector2 mouse, bool held)
        {
            if (held && dragging)
            {
                Vector2 delta = mouse - previousMouse;
                yaw = Mathf.Clamp(yaw + delta.x * DegreesPerPixel, -MaxYaw, MaxYaw);
                pitch = Mathf.Clamp(pitch - delta.y * DegreesPerPixel, -MaxPitch, MaxPitch);
            }

            dragging = held;
            previousMouse = mouse;
        }

        private void BeginCamera(ScriptableRenderContext context, Camera camera)
        {
            if (camera != roomCamera || applied)
                return;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            if (hasGamePose && ((originalPosition - previousGamePosition).sqrMagnitude > 0.04f ||
                Quaternion.Angle(originalRotation, previousGameRotation) > 12f))
                ResetOrbit();

            previousGamePosition = originalPosition;
            previousGameRotation = originalRotation;
            hasGamePose = true;
            if (yaw == 0f && pitch == 0f)
                return;

            Vector3 offset = originalPosition - Pivot;
            Quaternion horizontal = Quaternion.AngleAxis(yaw, Vector3.up);
            Vector3 pitchAxis = Vector3.Cross(Vector3.up, horizontal * offset).normalized;
            Quaternion orbit = Quaternion.AngleAxis(pitch, pitchAxis) * horizontal;
            transform.SetPositionAndRotation(Pivot + orbit * offset, orbit * originalRotation);
            applied = true;
        }

        private void EndCamera(ScriptableRenderContext context, Camera camera)
        {
            if (camera == roomCamera)
                RestorePose();
        }

        private void ResetOrbit()
        {
            dragging = false;
            yaw = pitch = 0f;
        }

        private void RestorePose()
        {
            if (!applied)
                return;

            transform.SetPositionAndRotation(originalPosition, originalRotation);
            applied = false;
        }
    }
}
