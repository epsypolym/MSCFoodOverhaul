using MSCLoader;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FoodOverhaul
{
    public class PlayerFoodSystems : MonoBehaviour
    {
        public enum MovingMode { Still, Walking, Sitting, InCar, CarryingShopBag, Sauna, Shower };
        public MovingMode movingMode = MovingMode.Still;

        public Nutrition Calorie = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Calorie,
            amount = 3000,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 3000
        };

        private List<Nutrition> nutritions = new List<Nutrition>();

        // -> these will be updated to use Nutrition -class
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
            // add nutritions to list
            nutritions.Add(Calorie);
        
            controller = FoodOverhaul.PLAYER.GetComponent<CharacterController>(); // get player
            StartCoroutine(UpdateCalories()); // start calories update loop
        }

        IEnumerator UpdateCalories()
        {
            bool finished = false;
            while(!finished)
            {
                UpdateCalorieValue();
                new WaitForSeconds(1); // update frequency in seconds
                yield return null;
            }
        }

        void UpdateCalorieValue()
        {
            float totalUsedCalories = UnityEngine.Random.Range(132, 147); // standing

            // get player moving speed
            if (controller)
                PlayerWalkingSpeed = controller.velocity.magnitude;

            // get player current action (wip)
            if (PlayMakerGlobals.Instance.Variables.GetFsmBool("PlayerComputer").Value)
                movingMode = MovingMode.Sitting;
            else if (PlayMakerGlobals.Instance.Variables.GetFsmBool("PlayerCarControl").Value)
                movingMode = MovingMode.InCar;
            else
                movingMode = MovingMode.Sitting;

            // calculate total used calories (per second)
            totalUsedCalories = GetTotalUsedCalories(movingMode, Calorie);
            // set calories absorption rate
            Calorie.absorptionRate = ((PlayerWalkingSpeed < 1 ? 1 : PlayerWalkingSpeed) * TotalDecreaseRate) * Time.deltaTime;
            // calculate calories absorption
            Calorie.CalculateAbsorption();
        }

        float GetTotalUsedCalories(MovingMode movingMode, Nutrition calorie)
        {
            float _ogValue = calorie.amount;
            switch(movingMode)
            {
                case MovingMode.Sitting:
                    _ogValue = UnityEngine.Random.Range(68, 74);
                    break;
                case MovingMode.InCar:
                    _ogValue = UnityEngine.Random.Range(114, 121);
                    break;
                default:
                    _ogValue = calorie.amount;
                    break;
            }
            return _ogValue * calorie.amountMultiplier;
        }

        public void AddNutritionValue(Nutrition.NutritionType nutritionType, float value)
        {
            Nutrition n = nutritions.Where(x => x.nutritionType == nutritionType).FirstOrDefault() as Nutrition;
            if(n != null)
                n.AddAmount(value);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 500, 500));
            GUILayout.Label("Calories: " + Calorie.amount);
            GUILayout.Label("Proteins: " + ProteinValue);
            GUILayout.Label("PlayerWalkingSpeed: " + PlayerWalkingSpeed);
            GUILayout.EndArea();
        }
    }
}