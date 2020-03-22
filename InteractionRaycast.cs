using MSCLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoodOverhaul
{
    public class InteractionRaycast : MonoBehaviour
    {
        public RaycastHit hitInfo;
        public GameObject bottleHolding;
        public GameObject handObject;
        public GameObject hackyworkaround;
        public bool isDrinking;

        public bool hasHit = false;
        public float rayDistance = 1f;
        public int layerMask;
        
        // Use this for initialization
        void Start()
        {
            isDrinking = false;
            layerMask = LayerMask.GetMask("Parts", "Wheel");
            hitInfo = new RaycastHit();
            bottleHolding = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/HandBottles/hand_rigged");
            handObject = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand");
            hackyworkaround = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/HandBottles");
        }
      
        void FixedUpdate()
        {
            if (Camera.main != null) hasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, rayDistance, layerMask);           
        }

        public bool GetHit(Collider collider) => hasHit && hitInfo.collider == collider;
        public bool GetHitAny(Collider[] colliders) => hasHit && colliders.Any(collider => collider == hitInfo.collider);
        public bool GetHitAny(List<Collider> colliders) => hasHit && colliders.Any(collider => collider == hitInfo.collider);
    }
}