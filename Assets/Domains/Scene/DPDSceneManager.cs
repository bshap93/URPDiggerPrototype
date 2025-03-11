using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using UnityEngine;

namespace Domains.Scene
{

    public class DpdSceneManager : MonoBehaviour
    {
        [Header("Character")]

        [SerializeField]
        CharacterActor characterActor = null;


        [Header("Scene references")]

        [SerializeField]
        CharacterReferenceObject[] references = null;

        [Header("UI")]


        [SerializeField]
        bool hideAndConfineCursor = true;

        [Header("Graphics")]

        [SerializeField]
        GameObject graphicsObject = null;

        [Header("Camera")]
        [SerializeField]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Camera3D camera = null;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        [UnityEngine.Serialization.FormerlySerializedAs("frameRateText")]
        [SerializeField]


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        Renderer[] graphicsRenderers = null;
        Renderer[] capsuleRenderers = null;

        NormalMovement normalMovement = null;

        float GetRefreshRateValue()
        {
#if UNITY_2022_2_OR_NEWER
            return (float)Screen.currentResolution.refreshRateRatio.value;
#else
            return Screen.currentResolution.refreshRate;
#endif
        }

        void Awake()
        {
            if (characterActor != null)
                normalMovement = characterActor.GetComponentInChildren<NormalMovement>();

            // Set the looking direction mode
            if (normalMovement != null && camera != null)
            {
                if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                    normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                else
                    normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.Movement;
            }


            if (graphicsObject != null)
                graphicsRenderers = graphicsObject.GetComponentsInChildren<Renderer>(true);

            Cursor.visible = !hideAndConfineCursor;
            Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;


        }

        void Update()
        {

            for (int index = 0; index < references.Length; index++)
            {
                if (references[index] == null)
                    break;

                if (Input.GetKeyDown(KeyCode.Alpha1 + index) || Input.GetKeyDown(KeyCode.Keypad1 + index))
                {
                    GoTo(references[index]);
                    break;
                }
            }




            if (Input.GetKeyDown(KeyCode.V))
            {
                // If the Camera3D is present, change between First person and Third person mode.
                if (camera != null)
                {
                    camera.ToggleCameraMode();

                    if (normalMovement != null)
                    {
                        if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                            normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                        else
                            normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.Movement;

                    }
                }
            }

        }



        void HandleVisualObjects(bool showCapsule)
        {
            if (capsuleRenderers != null)
                for (int i = 0; i < capsuleRenderers.Length; i++)
                    capsuleRenderers[i].enabled = showCapsule;

            if (graphicsRenderers != null)
                for (int i = 0; i < graphicsRenderers.Length; i++)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)graphicsRenderers[i];
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
