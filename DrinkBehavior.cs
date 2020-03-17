using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections;

namespace FoodOverhaul
{
    public class DrinkBehavior : MonoBehaviour
    {
        public float CalorieValue;
        public float Volume;
        public Vector3 inhandpos;
        public Quaternion inhandrot;
        public AudioClip useSound;
        public bool throwable;

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
            if (foodInteraction.GetHit(selfCollider) & !foodInteraction.isDrinking)
            {
                mouseOver = true;
                GUIuse.Value = true;

                if (Input.GetKeyDown(KeyCode.F)) Drink();
            }
            else if (mouseOver)
            {
                mouseOver = false;
                GUIuse.Value = false;
            }
        }

        void Drink()
        {
            
            pfd.CalorieValue += CalorieValue;
            pfd.ThirstVolume += Volume;
            self.GetComponent<AudioSource>().Play();
            StartCoroutine(DrinkEvent());
            foodInteraction.isDrinking = true;

            
            
        }

        IEnumerator DrinkEvent()
        {
            foodInteraction.hackyworkaround.SetActive(true);
            foodInteraction.handObject.SetActive(true);
            transform.SetParent(foodInteraction.handObject.transform);
            transform.localPosition = inhandpos;
            transform.localRotation = inhandrot;
            gameObject.layer = 20;
            GetComponent<Rigidbody>().isKinematic = true;
            yield return new WaitForSeconds(6.5f);
            foodInteraction.hackyworkaround.SetActive(false);
            foodInteraction.handObject.SetActive(false);
            foodInteraction.isDrinking = false;
            if (throwable)
            {
                ThrowEvent();
            }
            else
            {
                GameObject.Destroy(self);
            }
            
        }

        void ThrowEvent()
        {   
            transform.SetParent(null);
            LoadAssets.MakeGameObjectPickable(gameObject);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            transform.localPosition = Camera.main.transform.position + new Vector3(0,-0.1f,0);
            MasterAudio.PlaySound3DAndForget("Burb", transform, false, 1f, null, 0f, "burb01");
            gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 150);
            gameObject.GetComponent<DrinkBehavior>().enabled = false;
            gameObject.name = "empty(Clone)";
        }
    }
}