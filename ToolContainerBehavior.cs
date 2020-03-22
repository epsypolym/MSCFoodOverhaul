using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace FoodOverhaul
{
    public class ToolContainerBehavior : MonoBehaviour
    {
        public GameObject item;
        public int amount;
        public int stepCount;
        public int toolID;
        public GameObject lid;

        InteractionRaycast foodInteraction;
        Collider selfCollider;
        FsmBool GUIuse;
        int gameToolID;

        bool mouseOver = false;
        bool toolMode = false;

        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            selfCollider = GetComponent<Collider>();
            gameToolID = FoodOverhaul.gameToolID;
        }

        // Update is called once per frame
        void Update()
        {
            if (stepCount > 0)
            {
                if (foodInteraction.GetHit(selfCollider) & toolMode & (toolID == gameToolID))
                {
                    mouseOver = true;
                    GUIuse.Value = true;

                    if (Input.GetKeyDown(KeyCode.F)) openStep();
                }
                else if (mouseOver)
                {
                    mouseOver = false;
                    GUIuse.Value = false;
                }
            }
            else
            {
                openEvent();
            }
        }

        void openStep()
        {
            stepCount -= 1;
            transform.rotation.SetEulerAngles(transform.rotation.eulerAngles + new Vector3(0, 20, 0));
        }

        void openEvent()
        {
            GameObject.Destroy(lid);
            gameObject.GetComponent<PourableBehavior>().enabled = true;
            gameObject.GetComponent<ToolContainerBehavior>().enabled = false;
        }
    }
}