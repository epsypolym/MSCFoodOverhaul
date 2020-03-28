using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace FoodOverhaul
{
    public class FoodBehavior : MonoBehaviour
    {
        public float CalorieValue;
        public float ProteinValue;
        public float SugarValue;
        public float CarbohydrateValue;
        public float SodiumValue;
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
            pfs.AddNutritionValues(this);
            GameObject.Destroy(gameObject);
        }
    }
}