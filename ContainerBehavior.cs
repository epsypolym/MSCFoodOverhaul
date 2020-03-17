using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace FoodOverhaul
{
    public class ContainerBehavior : MonoBehaviour
    {
        public GameObject item;
        public int amount;

        InteractionRaycast foodInteraction;
        Collider selfCollider;
        GameObject self;
        FsmBool GUIuse;

        bool mouseOver = false;

        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            selfCollider = GetComponent<Collider>();
            self = transform.gameObject;
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
                GameObject instantiatedobject = GameObject.Instantiate(item) as GameObject;
                instantiatedobject.transform.position = gameObject.transform.position;
                LoadAssets.MakeGameObjectPickable(instantiatedobject);
                amount -= 1;
            }
            else
            {
                gameObject.name = "empty(Clone)";
                gameObject.GetComponent<ContainerBehavior>().enabled = false;

            }
        }
    }
}