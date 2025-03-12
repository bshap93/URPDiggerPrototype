using Domains.Input;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scene.Scripts
{
    public class DpdSceneManager : MonoBehaviour
    {
        [Header("Character")] [SerializeField] CharacterActor characterActor;


        [Header("Scene references")] [SerializeField]
        CharacterReferenceObject[] references;


        [SerializeField] bool hideAndConfineCursor = true;

        [Header("Graphics")] [SerializeField] GameObject graphicsObject;

        [Header("Camera")] [SerializeField]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Camera3D camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        [FormerlySerializedAs("frameRateText")] [SerializeField]

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Renderer[] graphicsRenderers;
        readonly Renderer[] capsuleRenderers = null;

        [Header("UI")] bool isPaused;

        NormalMovement normalMovement;

        void Awake()
        {
            if (characterActor != null)
                normalMovement = characterActor.GetComponentInChildren<NormalMovement>();

            // Set the looking direction mode
            if (normalMovement != null && camera != null)
            {
                if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                    normalMovement.lookingDirectionParameters.lookingDirectionMode =
                        LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                else
                    normalMovement.lookingDirectionParameters.lookingDirectionMode =
                        LookingDirectionParameters.LookingDirectionMode.Movement;
            }


            if (graphicsObject != null)
                graphicsRenderers = graphicsObject.GetComponentsInChildren<Renderer>(true);

            Cursor.visible = !hideAndConfineCursor;
            Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

        void Update()
        {
            var pressedKey = CustomInputBindings.GetPressedNumberKey();

            if (CustomInputBindings.IsCancelPressed()) TogglePause();


            if (pressedKey != -1 && pressedKey < references.Length && references[pressedKey] != null)
                GoTo(references[pressedKey]);


            if (CustomInputBindings.IsChangePerspectivePressed())
                // If the Camera3D is present, change between First person and Third person mode.
                if (camera != null)
                {
                    camera.ToggleCameraMode();

                    if (normalMovement != null)
                    {
                        if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                            normalMovement.lookingDirectionParameters.lookingDirectionMode =
                                LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                        else
                            normalMovement.lookingDirectionParameters.lookingDirectionMode =
                                LookingDirectionParameters.LookingDirectionMode.Movement;
                    }
                }
        }

        void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        }

        float GetRefreshRateValue()
        {
#if UNITY_2022_2_OR_NEWER
            return (float)Screen.currentResolution.refreshRateRatio.value;
#else
            return Screen.currentResolution.refreshRate;
#endif
        }


        void HandleVisualObjects(bool showCapsule)
        {
            if (capsuleRenderers != null)
                for (var i = 0; i < capsuleRenderers.Length; i++)
                    capsuleRenderers[i].enabled = showCapsule;

            if (graphicsRenderers != null)
                for (var i = 0; i < graphicsRenderers.Length; i++)
                {
                    var skinnedMeshRenderer = (SkinnedMeshRenderer)graphicsRenderers[i];
                    if (skinnedMeshRenderer != null)
                        skinnedMeshRenderer.forceRenderingOff = showCapsule;
                    else
                        graphicsRenderers[i].enabled = !showCapsule;
                }
        }

        void GoTo(CharacterReferenceObject reference)
        {
            if (reference == null)
                return;

            if (characterActor == null)
                return;

            characterActor.constraintUpDirection = reference.referenceTransform.up;
            characterActor.Teleport(reference.referenceTransform);

            characterActor.upDirectionReference = reference.verticalAlignmentReference;
            characterActor.upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;
        }
    }
}
