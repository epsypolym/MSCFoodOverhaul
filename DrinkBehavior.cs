using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections;

namespace FoodOverhaul
{
    public class DrinkBehavior : MonoBehaviour
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

        public float Volume;
        public Vector3 inhandpos;
        public Quaternion inhandrot;
        public AudioClip useSound;
        public bool throwable;

        InteractionRaycast foodInteraction;
        Rigidbody rigidbody;
        Collider selfCollider;
        PlayerFoodSystems pfs;
        FsmBool GUIuse;
        AudioSource audio;

        bool mouseOver = false;

        // Use this for initialization
        void Start()
        {
            foodInteraction = FoodOverhaul.foodInteraction;
            GUIuse = FoodOverhaul.GUIuse;
            rigidbody = GetComponent<Rigidbody>();
            selfCollider = GetComponent<Collider>();
            pfs = FoodOverhaul.PLAYER.GetComponent<PlayerFoodSystems>();
            audio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            // show mouse over icon, if this object is raycasted.
            if (foodInteraction.GetHit(selfCollider) && !foodInteraction.isDrinking)
            {
                mouseOver = true;
                GUIuse.Value = true;

                if (Input.GetKeyDown(KeyCode.F)) Drink(); // press use to Drink this.
            }
            // hide mouse over icon, if this object is not raycasted.
            else if (mouseOver)
            {
                mouseOver = false;
                GUIuse.Value = false;
            }
        }

        void Drink()
        {
            pfs.AddNutritionValues(this);
            pfs.ThirstVolume += Volume;
            audio.Play();
            StartCoroutine(DrinkEvent());                   
        }

        IEnumerator DrinkEvent()
        {
            foodInteraction.isDrinking = true;
            foodInteraction.hackyworkaround.SetActive(true);
            foodInteraction.handObject.SetActive(true);
            transform.SetParent(foodInteraction.handObject.transform);
            transform.localPosition = inhandpos;
            transform.localRotation = inhandrot;
            gameObject.layer = 20;
            rigidbody.isKinematic = true;

            // wait player to finish drinking
            yield return new WaitForSeconds(6.5f);

            foodInteraction.hackyworkaround.SetActive(false);
            foodInteraction.handObject.SetActive(false);
            foodInteraction.isDrinking = false;

            // throw or destroy this gameObject
            if (throwable)
                ThrowEvent();
            else
                GameObject.Destroy(gameObject);
            
        }

        void ThrowEvent()
        {   
            transform.SetParent(null);
            LoadAssets.MakeGameObjectPickable(gameObject);
            rigidbody.isKinematic = false;
            transform.localPosition = Camera.main.transform.position + new Vector3(0,-0.1f,0);
            MasterAudio.PlaySound3DAndForget("Burb", transform, false, 1f, null, 0f, "burb01");
            rigidbody.AddForce(Camera.main.transform.forward * 150);      
            gameObject.name = "empty(Clone)";
            enabled = false;
        }
    }
}