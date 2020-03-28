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

            controller = FoodOverhaul.PLAYER.GetComponent<CharacterController>(); // get player

            // get fsm variables
            PlayMakerFSM _Crouch = FoodOverhaul.PLAYER.GetComponents<PlayMakerFSM>().Where(x => x.FsmName == "Crouch").FirstOrDefault() as PlayMakerFSM;
            PlayerInWater = _Crouch.FsmVariables.FindFsmBool("PlayerInWater");
            CrouchPosition = _Crouch.FsmVariables.FindFsmFloat("Position");
            ShowerObject = GameObject.Find("YARD").transform.Find("Building/BATHROOM/Shower/Shower");
            ShowerDistance = ShowerObject.GetComponents<PlayMakerFSM>().Where(x => x.FsmName == "Distance").FirstOrDefault().FsmVariables.GetFsmFloat("Distance") as FsmFloat;

            StartCoroutine(UpdateCalories()); // start calories update loop
        }

        IEnumerator UpdateCalories()
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

        public void AddNutritionValue(Nutrition.NutritionType nutritionType, float value)
        {
            Nutrition n = nutritions.Where(x => x.nutritionType == nutritionType).FirstOrDefault() as Nutrition;
            if(n != null)
                n.AddAmount(value);
        }

        void UpdateCurrentMovingMode()
        {
            // get player moving speed
            if (controller)
                PlayerWalkingSpeed = controller.velocity.magnitude;

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
        }

        private void OnGUI()
        {
            string currentVehicleName = "None";
            if (currentVehicle != null)
                currentVehicleName = currentVehicle.name;

            GUILayout.BeginArea(new Rect(30, 30, 500, 500));
            GUILayout.Label("Calories: " + Calorie.amount);
            GUILayout.Label("Proteins: " + ProteinValue);
            GUILayout.Label("PlayerWalkingSpeed: " + PlayerWalkingSpeed);
            GUILayout.Label("CurrentVehicle: " + currentVehicleName);
            GUILayout.Label("PlayerInWater: " + PlayerInWater.Value.ToString());
            GUILayout.Label("PlayerCrouchPos: " + CrouchPosition.Value.ToString());
            GUILayout.Label("MovingMode: " + movingMode.ToString());
            GUILayout.EndArea();
        }
    }
}