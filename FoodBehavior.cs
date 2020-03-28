using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace FoodOverhaul
{
    public class FoodBehavior : MonoBehaviour
    {
        public float CalorieValue;
        public float ProteinValue;
        public float GlucoseValue;
        public float CalciumValue;
        public float ZincValue;
        public float ChromiumValue;
        public float IronValue;
        public float VitaminBValue;
        public float VitaminCValue;
        public float VitaminDValue;

        InteractionRaycast foodInteraction;
        Collider selfCollider;
        PlayerFoodSystems pfs;
        FsmBool GUIuse;

        bool mouseOver = false;

        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            selfCollider = GetComponent<Collider>();
            pfs = FoodOverhaul.PLAYER.GetComponent<PlayerFoodSystems>();
        }

        // Update is called once per frame
        void Update()
        {
            if(foodInteraction.GetHit(selfCollider))
            {
                mouseOver = true;
                GUIuse.Value = true;

                if (Input.GetKeyDown(KeyCode.F)) Consume();
            }
            else if (mouseOver)
            {
                mouseOver = false;
                GUIuse.Value = false;
            }
        }

        void Consume()
        {
            //very epic beautiful code <3 thanks ajongl
            if (CalorieValue != 0)
                pfs.AddNutritionValue(Nutrition.NutritionType.Calorie, CalorieValue);

            //pfs.CalorieValue += CalorieValue;
            //pfs.ProteinValue += ProteinValue;
            //pfs.GlucoseValue += GlucoseValue;
            //pfs.CalciumValue += CalciumValue;
            //pfs.ZincValue += ZincValue;
            //pfs.ChromiumValue += ChromiumValue;
            //pfs.IronValue += IronValue;
            //pfs.VitaminBValue += VitaminBValue;
            //pfs.VitaminCValue += VitaminCValue;
            //pfs.VitaminDValue += CalorieValue;
            GameObject.Destroy(gameObject);
        }
    }
}