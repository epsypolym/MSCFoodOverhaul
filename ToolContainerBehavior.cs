using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace FoodOverhaul
{
    public class ToolContainerBehavior : MonoBehaviour
    {
        public GameObject item;
        public int amount;
        public int stepCount;
        public float toolID;
        public GameObject lid;
        bool mouseOver = false;

        InteractionRaycast foodInteraction;
        Collider selfCollider;
        FsmBool GUIuse;
        FsmFloat gameToolID;

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
                if (foodInteraction.GetHit(selfCollider) & gameToolID.Value > 0f & toolID == gameToolID.Value)
                {
                    mouseOver = GUIuse.Value = true;
                    if (Input.GetAxis("Mouse ScrollWheel") < 0f) openStep();
                }
                else if (mouseOver)
                {
                    mouseOver = GUIuse.Value = false;
                }
            }
            else if (foodInteraction.GetHit(selfCollider))
            {
                mouseOver = GUIuse.Value = true;
                if (Input.GetKeyDown(KeyCode.F)) openEvent();
            }
            else if (mouseOver)
            {
                mouseOver = GUIuse.Value = false;
            }
        }

        void openStep()
        {
            stepCount -= 1;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 20, 0));
        }

        void openEvent()
        {
            GameObject.Destroy(lid);
            gameObject.GetComponent<PourableBehavior>().enabled = true;
            gameObject.GetComponent<ToolContainerBehavior>().enabled = false;
        }
    }
}