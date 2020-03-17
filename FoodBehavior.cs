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
        GameObject self;
        PlayerFoodSystems pfd;
        FsmBool GUIuse;

        bool mouseOver = false;
        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            selfCollider = GetComponent<Collider>();
            self = transform.gameObject;
            pfd = foodInteraction.playerObject.GetComponent<PlayerFoodSystems>();
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
            pfd.CalorieValue += CalorieValue;
            pfd.ProteinValue += ProteinValue;
            pfd.GlucoseValue += GlucoseValue;
            pfd.CalciumValue += CalciumValue;
            pfd.ZincValue += ZincValue;
            pfd.ChromiumValue += ChromiumValue;
            pfd.IronValue += IronValue;
            pfd.VitaminBValue += VitaminBValue;
            pfd.VitaminCValue += VitaminCValue;
            pfd.VitaminDValue += CalorieValue;
            GameObject.Destroy(self);
        }

    }
}