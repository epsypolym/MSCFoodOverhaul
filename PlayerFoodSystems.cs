using MSCLoader;
using UnityEngine;
using System;
using System.Collections;

namespace FoodOverhaul
{
    public class PlayerFoodSystems : MonoBehaviour
    {
        public enum MovingMode { Still, Walking, Sitting, InCar, CarryingShopBag, Sauna, Shower };
        public MovingMode movingMode = MovingMode.Still;

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
        public float CalorieMultiplier = 0.01f;

        private CharacterController controller;

        void Start()
        {
            controller = FoodOverhaul.PLAYER.GetComponent<CharacterController>();
            StartCoroutine(UpdateCalories());
        }

        IEnumerator UpdateCalories()
        {
            bool finished = false;
            while(!finished)
            {
                UpdateCalorieValue();
                new WaitForSeconds(1);
                yield return null;
            }
        }

        void UpdateCalorieValue()
        {
            float totalUsedCalories = UnityEngine.Random.Range(132, 147); // standing

            if (controller)
                PlayerWalkingSpeed = controller.velocity.magnitude;

            if (PlayMakerGlobals.Instance.Variables.GetFsmBool("PlayerComputer").Value)
                movingMode = MovingMode.Sitting;
            else if (PlayMakerGlobals.Instance.Variables.GetFsmBool("PlayerCarControl").Value)
                movingMode = MovingMode.InCar;
            else
                movingMode = MovingMode.Sitting;

            totalUsedCalories = GetTotalUsedCalories(movingMode, totalUsedCalories);
            CalorieValue -= ( (PlayerWalkingSpeed < 1 ? 1 : PlayerWalkingSpeed) * TotalDecreaseRate * Time.deltaTime ) * totalUsedCalories;
        }

        float GetTotalUsedCalories(MovingMode movingMode, float originalValue)
        {
            float _ogValue = originalValue;
            switch(movingMode)
            {
                case MovingMode.Sitting:
                    _ogValue = UnityEngine.Random.Range(68, 74);
                    break;
                case MovingMode.InCar:
                    _ogValue = UnityEngine.Random.Range(114, 121);
                    break;
                default:
                    _ogValue = originalValue;
                    break;
            }
            return _ogValue * CalorieMultiplier;
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