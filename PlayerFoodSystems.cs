using MSCLoader;
using UnityEngine;

namespace FoodOverhaul
{
    public class PlayerFoodSystems : MonoBehaviour
    {
        public float CalorieValue = 3000;
        public float ProteinValue = 91;
        public float GlucoseValue;
        public float CalciumValue;
        public float ZincValue;
        public float ChromiumValue;
        public float IronValue;
        public float VitaminBValue;
        public float VitaminCValue;
        public float VitaminDValue;

        public float ThirstVolume;

        private float PlayerWalkingSpeed = 0;
        private float TotalDecreaseRate = 1;

        private CharacterController controller;

        void Start()
        {
            controller = FoodOverhaul.PLAYER.GetComponent<CharacterController>();
        }

        private void Update()
        {
            UpdateCalories();
        }

        void UpdateCalories()
        {
            if (controller)
                PlayerWalkingSpeed = controller.velocity.magnitude;

            CalorieValue -= (PlayerWalkingSpeed < 1 ? 1 : PlayerWalkingSpeed) * TotalDecreaseRate * Time.deltaTime;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 500, 500));
            GUILayout.Label("Calories: " + CalorieValue);
            GUILayout.Label("Proteins: " + ProteinValue);
            GUILayout.Label("PlayerWalkingSpeed: " + PlayerWalkingSpeed);
            GUILayout.EndArea();
        }

    }
}