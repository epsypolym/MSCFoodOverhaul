using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace FoodOverhaul
{
    public class FoodOverhaul : Mod
    {
        public override string ID => "FoodOverhaul"; //Your mod ID (unique)
        public override string Name => "FoodOverhaul"; //You mod name
        public override string Author => "epsypolym"; //Your Username
        public override string Version => "1.0"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;
        public static InteractionRaycast foodInteraction;
        public static FsmBool GUIuse;
        public static GameObject PLAYER;
        public static int gameToolID;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            PLAYER = GameObject.Find("PLAYER");
            foodInteraction = PLAYER.AddComponent<InteractionRaycast>();
            PLAYER.AddComponent<PlayerFoodSystems>();
            GUIuse = PlayMakerGlobals.Instance.Variables.GetFsmBool("GUIuse");

            //delet zone over
            AssetBundle ab = LoadAssets.LoadBundle(this, "foodoverhaul.unity3d");
            GameObject peas = ab.LoadAsset("canned pea soup.prefab") as GameObject;
            GameObject fishsticc = ab.LoadAsset("fish sticks.prefab") as GameObject;
            GameObject colabruh = ab.LoadAsset("cola.prefab") as GameObject;
            GameObject sodabruh = ab.LoadAsset("soda.prefab") as GameObject;
            ab.Unload(false);
            
            GameObject peasbruh = GameObject.Instantiate(peas);
            LoadAssets.MakeGameObjectPickable(peasbruh);
            GameObject fishsticks = GameObject.Instantiate(fishsticc);
            LoadAssets.MakeGameObjectPickable(fishsticks);
            GameObject cola = GameObject.Instantiate(colabruh);
            LoadAssets.MakeGameObjectPickable(cola);
            GameObject soda = GameObject.Instantiate(sodabruh);
            LoadAssets.MakeGameObjectPickable(soda);
        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        //public override void OnGUI()
        //{
            // Draw unity OnGUI() here
        //}
    }
}
