using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace FoodOverhaul
{
    public class UseContainerBehavior : MonoBehaviour
    {
        public GameObject item;
        public int amount;
        InteractionRaycast foodInteraction;
        Collider selfCollider;
        FsmBool GUIuse;

        bool mouseOver = false;

        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            selfCollider = GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (foodInteraction.GetHit(selfCollider))
            {
                mouseOver = true;
                GUIuse.Value = true;

                if (Input.GetKeyDown(KeyCode.F)) TakeOutItem();
            }
            else if (mouseOver)
            {
                mouseOver = false;
                GUIuse.Value = false;
            }
        }

        void TakeOutItem()
        {
            if (amount > 0)
            {
                GameObject instantiatedObject = GameObject.Instantiate(item) as GameObject;
                instantiatedObject.transform.position = gameObject.transform.position;
                LoadAssets.MakeGameObjectPickable(instantiatedObject);
                amount -= 1;
            }
            else
            {
                gameObject.name = "empty(Clone)";
                enabled = false;
            }
        }
    }
}