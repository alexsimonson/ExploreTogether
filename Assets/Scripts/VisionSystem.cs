using UnityEngine;

namespace ExploreTogether {
    public class VisionSystem : MonoBehaviour
    {
        public LayerMask playerLayer; // Layer mask for the player
        public LayerMask obstacleLayer; // Layer mask for obstacles
        public float fieldOfViewHorizontal = 90f; // Horizontal field of view angle in degrees
        public float fieldOfViewVertical = 60f; // Vertical field of view angle in degrees
        public float viewDistance = 1000f; // Maximum view distance
        public int raycastWidthCount = 50; // Number of raycasts to perform along the width of the field of view
        public int raycastHeightCount = 30; // Number of raycasts to perform along the height of the field of view
        public float fixedRaycastHeight = 1.0f; // Fixed height for the raycasts
        public Transform eyeballLocation; // Reference to the custom eyeball location transform
        public Material lineMaterial;

        private LineRenderer lineRenderer;
        private int currentLineIndex = 0;
        public bool playerDetected = false;

        private Transform chaseTargetTransform; // Reference to the chase target's transform

        public bool drawLines = true; // Control for drawing lines

        private void Start()
        {
            // Set the default eyeballLocation to Vector3(0.0146000003, 1.70749998, 0.153200001)
            if (eyeballLocation == null)
            {
                eyeballLocation = new GameObject("EyeballLocation").transform;
                eyeballLocation.parent = transform;
                eyeballLocation.localPosition = new Vector3(0.0146000003f, 1.70749998f, 0.153200001f);
            }

            // Initialize LineRenderer
            GameObject lineObj = new GameObject("LineRenderer");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = raycastWidthCount * raycastHeightCount * 2;
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        private void Update()
        {
            // Reset current line index
            currentLineIndex = 0;

            // Check if the player is within the field of view and visible
            if (IsPlayerVisible())
            {
                // Player detected!
                Debug.Log("Player detected!");
                // should set the state to chase
                GetComponent<GenericNPC>().SetState(GenericNPC.State.Chase);
            }
            else
            {
                // Reset the chase target's transform to null if the player is not visible
                chaseTargetTransform = null;
            }
        }

        private bool IsPlayerVisible()
        {
            float horizontalAngleStep = fieldOfViewHorizontal / (raycastWidthCount - 1);
            float verticalAngleStep = fieldOfViewVertical / (raycastHeightCount - 1);
            float halfFOVHorizontal = fieldOfViewHorizontal * 0.5f;
            float halfFOVVertical = fieldOfViewVertical * 0.5f;

            this.playerDetected = false;

            for (int i = 0; i < raycastWidthCount; i++)
            {
                float horizontalAngle = -halfFOVHorizontal + i * horizontalAngleStep;

                for (int j = 0; j < raycastHeightCount; j++)
                {
                    float verticalAngle = -halfFOVVertical + j * verticalAngleStep;

                    // Calculate the direction using Spherical Coordinates
                    float phi = Mathf.Deg2Rad * (90f - verticalAngle);
                    float theta = Mathf.Deg2Rad * (horizontalAngle + 90f);
                    Vector3 direction = new Vector3(
                        Mathf.Sin(phi) * Mathf.Cos(theta),
                        Mathf.Cos(phi),
                        Mathf.Sin(phi) * Mathf.Sin(theta)
                    );
                    direction = transform.rotation * direction;

                    // Perform a raycast from the eyeball location
                    RaycastHit hit;
                    if (Physics.Raycast(eyeballLocation.position, direction, out hit, viewDistance, playerLayer | obstacleLayer))
                    {
                        if ((obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                        {
                            // Obstacle hit, adjust the view distance
                            if (drawLines)
                            {
                                DrawLine(eyeballLocation.position, hit.point);
                            }
                        }
                        else
                        {
                            // Player is visible and not obstructed by any obstacles
                            if (drawLines)
                            {
                                DrawLine(eyeballLocation.position, eyeballLocation.position + direction * viewDistance);
                            }
                            this.playerDetected = true;
                            chaseTargetTransform = hit.collider.gameObject.transform;
                        }
                    }
                    else
                    {
                        // No hit, draw the full view distance
                        if (drawLines)
                        {
                            DrawLine(eyeballLocation.position, eyeballLocation.position + direction * viewDistance);
                        }
                    }
                }
            }

            return this.playerDetected;
        }

        private void DrawLine(Vector3 start, Vector3 end)
        {
            int startIndex = currentLineIndex * 2;

            lineRenderer.SetPosition(startIndex, start);
            lineRenderer.SetPosition(startIndex + 1, end);

            currentLineIndex++;
        }

        public Vector3 GetChaseTargetPosition()
        {
            if (chaseTargetTransform != null)
            {
                return chaseTargetTransform.position;
            }
            else
            {
                return Vector3.zero; // Return a default position if the chase target is not set
            }
        }

        public GameObject GetChaseTargetObject(){
            if(chaseTargetTransform!=null){
                return chaseTargetTransform.gameObject;
            }else{
                return null;
            }
        }
    }
}
