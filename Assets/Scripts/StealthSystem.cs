using UnityEngine;
	
namespace ExploreTogether {
    public class StealthSystem : MonoBehaviour
	{
        private VisionSystem visionSystem; // Reference to the VisionSystem component
        public float stealthChance = 0.0f; // The chance of being seen by the VisionSystem (0.0 - 1.0)
    
        void Start(){
            visionSystem = GetComponent<VisionSystem>();
        }
    
        private void Update()
        {
            CalculateStealthChance();
        }
    
        private void CalculateStealthChance()
        {
            // Calculate the number of raycasts that can see the player
            int visibleRaycastCount = visionSystem.playerDetected ? 1 : 0;
    
            // Calculate the total number of raycasts
            int totalRaycastCount = visionSystem.raycastWidthCount * visionSystem.raycastHeightCount;
    
            // Calculate the stealth chance based on the ratio of visible raycasts to total raycasts
            stealthChance = (float)visibleRaycastCount / totalRaycastCount;
        }
    }
}