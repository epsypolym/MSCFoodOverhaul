using MSCLoader;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;

namespace FoodOverhaul
{
    public class PlayerFoodSystems : MonoBehaviour
    {
        public enum MovingMode { Still, Walking, Sitting, Driving, CarryingShopBag, Sauna, Shower, Swimming };
        public MovingMode movingMode = MovingMode.Still;

        #region Nutritions
        public Nutrition Calorie = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Calorie,
            amount = 3000,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 3000
        };

        public Nutrition Protein = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Protein,
            amount = 91,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 91
        };

        public Nutrition Sugar = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Sugar,
            amount = 120,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 120
        };

        public Nutrition Carbohydrate = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Carbohydrate,
            amount = 400,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 400
        };

        public Nutrition Sodium = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Sodium,
            amount = 5,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 5
        };

        public Nutrition Calcium = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Calcium,
            amount = 0.8f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.8f
        };

        public Nutrition Zinc = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Zinc,
            amount = 0.25f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.25f
        };

        public Nutrition Chromium = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Chromium,
            amount = 0.40f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.40f
        };

        public Nutrition Iron = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.Iron,
            amount = 0.09f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.09f
        };

        public Nutrition VitaminB = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.VitaminB,
            amount = 0.02f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.02f
        };

        public Nutrition VitaminC = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.VitaminC,
            amount = 0.75f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.75f
        };

        public Nutrition VitaminD = new Nutrition()
        {
            nutritionType = Nutrition.NutritionType.VitaminD,
            amount = 0.10f,
            amountMultiplier = 1,
            absorptionRate = 0,
            absorptionRateMultiplier = 1,
            amountPerDay = 0.10f
        };

        private List<Nutrition> nutritions = new List<Nutrition>();
        #endregion

        public float ThirstVolume;

        private float PlayerWalkingSpeed = 0;
        private float TotalDecreaseRate = 1;

        private CharacterController controller;

        private FsmBool PlayerInWater; // if Value is true, then swimming
        private FsmFloat CrouchPosition; // if Value is < 0.3f, then sitting
        private FsmFloat ShowerDistance; // player distance from shower
        private Transform ShowerObject; // if gameObject is active and ShowerDistance is < 0.2f, then in shower

        private AxisCarController currentVehicle = null;
        private Drivetrain currentDrivetrain = null;

        void Start()
        {
            // add nutritions to list
            nutritions.Add(Calorie);
            nutritions.Add(Protein);
            nutritions.Add(Sugar);
            nutritions.Add(Carbohydrate);
            nutritions.Add(Sodium);
            nutritions.Add(Calcium);
            nutritions.Add(Zinc);
            nutritions.Add(Chromium);
            nutritions.Add(Iron);
            nutritions.Add(VitaminB);
            nutritions.Add(VitaminC);
            nutritions.Add(VitaminD);

            controller = FoodOverhaul.PLAYER.GetComponent<CharacterController>(); // get player

            // get fsm variables
            PlayMakerFSM _Crouch = FoodOverhaul.PLAYER.GetComponents<PlayMakerFSM>().Where(x => x.FsmName == "Crouch").FirstOrDefault() as PlayMakerFSM;
            PlayerInWater = _Crouch.FsmVariables.FindFsmBool("PlayerInWater");
            CrouchPosition = _Crouch.FsmVariables.FindFsmFloat("Position");
            ShowerObject = GameObject.Find("YARD").transform.Find("Building/BATHROOM/Shower/Shower");
            ShowerDistance = ShowerObject.GetComponents<PlayMakerFSM>().Where(x => x.FsmName == "Distance").FirstOrDefault().FsmVariables.GetFsmFloat("Distance") as FsmFloat;

            StartCoroutine(UpdateNutritions()); // start calories update loop
        }

        IEnumerator UpdateNutritions()
        {
            bool finished = false;
            while(!finished)
            {
                // get current vehicle (if any)
                try
                {
                    currentVehicle = controller.transform.root.GetComponent<AxisCarController>();
                    currentDrivetrain = currentVehicle.GetComponent<Drivetrain>();
                } catch
                {
                    currentVehicle = null;
                    currentDrivetrain = null;
                }
               
                UpdateCurrentMovingMode(); // update current moving mode
                UpdateCalorieValues(); // update current calories

                new WaitForSeconds(1); // update frequency in seconds
                yield return null;
            }
        }

        void UpdateCalorieValues()
        {
            float totalUsedCalories = UnityEngine.Random.Range(132, 147); // standing

            // calculate total used calories (per second)
            totalUsedCalories = GetTotalUsedCalories(movingMode, Calorie);
            // set calories absorption rate
            Calorie.absorptionRate = ((PlayerWalkingSpeed < 1 ? 1 : PlayerWalkingSpeed) * TotalDecreaseRate) * Time.deltaTime;
            // calculate calories absorption
            Calorie.CalculateAbsorption();
        }

        /// <summary>
        /// Returns total used calories per second depending on Player actions.
        /// </summary>
        /// <param name="movingMode"></param>
        /// <param name="calorie"></param>
        /// <returns></returns>
        float GetTotalUsedCalories(MovingMode movingMode, Nutrition calorie)
        {
            float _ogValue = calorie.amount;
            switch(movingMode)
            {
                case MovingMode.Sitting:
                    _ogValue = UnityEngine.Random.Range(68, 74);
                    break;
                case MovingMode.Driving:
                    _ogValue = UnityEngine.Random.Range(114, 121);
                    break;
                case MovingMode.Swimming:
                    _ogValue = UnityEngine.Random.Range(391, 406);
                    break;
                case MovingMode.Walking:
                    _ogValue = UnityEngine.Random.Range(176, 184);
                    break;
                case MovingMode.Still:
                    _ogValue = UnityEngine.Random.Range(92, 110);
                    break;
                case MovingMode.Shower:
                    _ogValue = UnityEngine.Random.Range(143, 153);
                    break;
                default:
                    _ogValue = calorie.amount;
                    break;
            }
            return _ogValue * calorie.amountMultiplier;
        }

        /// <summary>
        /// Add single Nutrition value to PlayerFoodSystem
        /// </summary>
        /// <param name="nutritionType"></param>
        /// <param name="value"></param>
        public void AddNutritionValue(Nutrition.NutritionType nutritionType, float value)
        {
            Nutrition n = nutritions.Where(x => x.nutritionType == nutritionType).FirstOrDefault() as Nutrition;
            if(n != null)
                n.AddAmount(value);
        }

        /// <summary>
        /// Add all Nutrition values from food to PlayerFood System
        /// </summary>
        /// <param name="foodBehavior"></param>
        public void AddNutritionValues(FoodBehavior foodBehavior)
        {
            AddNutritionValue(Nutrition.NutritionType.Calorie, foodBehavior.CalorieValue);
            AddNutritionValue(Nutrition.NutritionType.Protein, foodBehavior.ProteinValue);
            AddNutritionValue(Nutrition.NutritionType.Sugar, foodBehavior.SugarValue);
            AddNutritionValue(Nutrition.NutritionType.Carbohydrate, foodBehavior.CarbohydrateValue);
            AddNutritionValue(Nutrition.NutritionType.Sodium, foodBehavior.SodiumValue);
            AddNutritionValue(Nutrition.NutritionType.Calcium, foodBehavior.CalciumValue);
            AddNutritionValue(Nutrition.NutritionType.Zinc, foodBehavior.ZincValue);
            AddNutritionValue(Nutrition.NutritionType.Chromium, foodBehavior.ChromiumValue);
            AddNutritionValue(Nutrition.NutritionType.Iron, foodBehavior.IronValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminB, foodBehavior.VitaminBValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminC, foodBehavior.VitaminCValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminD, foodBehavior.VitaminDValue);
        }

        /// <summary>
        /// Add all Nutrition values from drink to PlayerFood System
        /// </summary>
        /// <param name="drinkBehavior"></param>
        public void AddNutritionValues(DrinkBehavior drinkBehavior)
        {
            AddNutritionValue(Nutrition.NutritionType.Calorie, drinkBehavior.CalorieValue);
            AddNutritionValue(Nutrition.NutritionType.Protein, drinkBehavior.ProteinValue);
            AddNutritionValue(Nutrition.NutritionType.Sugar, drinkBehavior.SugarValue);
            AddNutritionValue(Nutrition.NutritionType.Carbohydrate, drinkBehavior.CarbohydrateValue);
            AddNutritionValue(Nutrition.NutritionType.Sodium, drinkBehavior.SodiumValue);
            AddNutritionValue(Nutrition.NutritionType.Calcium, drinkBehavior.CalciumValue);
            AddNutritionValue(Nutrition.NutritionType.Zinc, drinkBehavior.ZincValue);
            AddNutritionValue(Nutrition.NutritionType.Chromium, drinkBehavior.ChromiumValue);
            AddNutritionValue(Nutrition.NutritionType.Iron, drinkBehavior.IronValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminB, drinkBehavior.VitaminBValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminC, drinkBehavior.VitaminCValue);
            AddNutritionValue(Nutrition.NutritionType.VitaminD, drinkBehavior.VitaminDValue);
        }

        void UpdateCurrentMovingMode()
        {
            // Player is Swimming
            if (PlayerInWater.Value)
                movingMode = MovingMode.Swimming;
            else
            {
                // Player is Sitting
                if (CrouchPosition.Value <= 0.3f && currentVehicle == null && currentDrivetrain != null)
                {
                    movingMode = MovingMode.Sitting;
                }
                // Player is Driving / Sitting in vehicle
                else if (currentVehicle != null && currentDrivetrain != null)
                {
                    // Player is Driving
                    if (currentDrivetrain.velo > 1)
                        movingMode = MovingMode.Driving;
                    // Player is Sitting in vehicle
                    else
                        movingMode = MovingMode.Sitting;
                }
                else
                {
                    if (PlayerWalkingSpeed < 1)
                    {
                        // Player is Sitting
                        if (CrouchPosition.Value <= 0.3f && PlayerWalkingSpeed == 0)
                            movingMode = MovingMode.Sitting;
                        // Player is Standing
                        else
                            movingMode = MovingMode.Still;
                    }
                    // Player is Walking
                    else
                        movingMode = MovingMode.Walking;
                }
            }

            // Player is in Shower
            if (ShowerDistance.Value < 0.2f && ShowerObject.gameObject.activeSelf)
                movingMode = MovingMode.Shower;

            // get player moving speed
            // set walking speed to zero if we are driving vehicle or sitting
            if (controller)
                PlayerWalkingSpeed = (movingMode == MovingMode.Driving || movingMode == MovingMode.Sitting) ? 0 : controller.velocity.magnitude;
        }

        private void OnGUI()
        {
            string currentVehicleName = "None";
            if (currentVehicle != null)
                currentVehicleName = currentVehicle.name;

            GUILayout.BeginArea(new Rect(30, 30, 500, 500));
            GUILayout.Label("PlayerWalkingSpeed: " + PlayerWalkingSpeed);
            GUILayout.Label("CurrentVehicle: " + currentVehicleName);
            GUILayout.Label("PlayerInWater: " + PlayerInWater.Value.ToString());
            GUILayout.Label("PlayerCrouchPos: " + CrouchPosition.Value.ToString());
            GUILayout.Label("MovingMode: " + movingMode.ToString());

            foreach(Nutrition n in nutritions)
                GUILayout.Label(n.nutritionType.ToString() + ": amount: " + n.amount + ", absorptionRate: " + n.absorptionRate);

            GUILayout.EndArea();
        }
    }
}