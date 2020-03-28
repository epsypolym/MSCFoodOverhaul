using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FoodOverhaul
{
    [System.Serializable]
    public class Nutrition
    {
        public enum NutritionType { Calorie, Protein, Glucose, Calcium, Zinc, Chromium, Iron, VitaminB, VitaminC, VitaminD, BloodSugar, Carbohydrate };
       
        public NutritionType nutritionType;
        public float amount; // in body
        public float amountMultiplier = 1;
        public float absorptionRate;
        public float absorptionRateMultiplier = 1; // affected by other nutritions and stuff
        public float amountPerDay; // starting value / how much you should absorpt per day
     
        public void CalculateAbsorption()
        {
            amount -= (absorptionRate * absorptionRateMultiplier) * Time.deltaTime;
        }

        public void AddAmount(float _amount)
        {
            amount += _amount;
        }
    }
}
