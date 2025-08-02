using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPlayFab.ProgressionModels;
using Il2CppSystem.Threading.Tasks;
using Il2CppTMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RumbleHud
{
    class Resources
    {
        private static AssetBundle bundle;

        private static TMP_FontAsset tmpFont;
        private static Texture2D backgroundTexture = null;
        private static Texture2D healthPipsTexture = null;
        private static Texture2D hostIconTexture = null;
        private static Dictionary<ShiftStones, Texture2D> shiftStoneTextures = new Dictionary<ShiftStones, Texture2D>();
        private static Dictionary<ShiftStones, string> shiftStoneResourceNames = new Dictionary<ShiftStones, string>()
        {
            {ShiftStones.Empty, "ss_empty"},
            {ShiftStones.Adamant, "ss_adamant"},
            {ShiftStones.Charge, "ss_charge"},
            {ShiftStones.Flow, "ss_flow"},
            {ShiftStones.Guard, "ss_guard"},
            {ShiftStones.Stubborn, "ss_stubborn"},
            {ShiftStones.Surge, "ss_surge"},
            {ShiftStones.Vigor, "ss_vigor"},
            {ShiftStones.Volatile, "ss_volatile"}
        };

        private static bool initialized = false;
        public static bool Initialized { get { return initialized; } }

        public static TMP_FontAsset TmpFont { get { return tmpFont; } }
        public static Texture2D BackgroundTexture { get {  return backgroundTexture; } }
        public static Texture2D HealthPipsTexture { get { return healthPipsTexture; } }
        public static Texture2D HostIconTexture {  get { return hostIconTexture; } }

        public static readonly Color HealthLow = new Color(151f / 255, 74f / 255, 69f / 255);
        public static readonly Color HealthMedium = new Color(139f / 255, 132f / 255, 66f / 255);
        public static readonly Color HealthHigh = new Color(96f / 255, 142f / 255, 83f / 255);
        public static readonly Color HealthFull = new Color(124f / 255, 150f / 255, 171f / 255);

        private static void LoadShiftStoneTexture(ShiftStones shiftStone)
        {
            var resourceName = shiftStoneResourceNames[shiftStone];
            var shiftStoneTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>(resourceName));
            shiftStoneTexture.name = $"RumbleHud_{shiftStone}";
            shiftStoneTextures[shiftStone] = shiftStoneTexture;
            GameObject.DontDestroyOnLoad(shiftStoneTexture);
        }

        public static void LoadResources(bool reload = false)
        {
            if (initialized && !reload) return;

            //bundle = Il2CppAssetBundleManager.LoadFromFile(@"UserData/rumblehud");
            bundle = LoadBundleFromFile(@"UserData/rumblehud");
            tmpFont = GameObject.Instantiate(bundle.LoadAsset<TMP_FontAsset>("TMP_GoodDogPlain"));
            backgroundTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("PlayerBackground"));
            backgroundTexture.name = "RumbleHud_BackgroundTexture";
            healthPipsTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("HealthPip"));
            healthPipsTexture.name = "RumbleHud_HealthPipTexture";
            hostIconTexture = GameObject.Instantiate(bundle.LoadAsset<Texture2D>("HostIcon"));
            hostIconTexture.name = "RumbleHud_HostIconTexture";

            LoadShiftStoneTexture(ShiftStones.Empty);
            LoadShiftStoneTexture(ShiftStones.Adamant);
            LoadShiftStoneTexture(ShiftStones.Charge);
            LoadShiftStoneTexture(ShiftStones.Flow);
            LoadShiftStoneTexture(ShiftStones.Guard);
            LoadShiftStoneTexture(ShiftStones.Stubborn);
            LoadShiftStoneTexture(ShiftStones.Surge);
            LoadShiftStoneTexture(ShiftStones.Vigor);
            LoadShiftStoneTexture(ShiftStones.Volatile);

            GameObject.DontDestroyOnLoad(backgroundTexture);
            GameObject.DontDestroyOnLoad(healthPipsTexture);
            GameObject.DontDestroyOnLoad(hostIconTexture);

            initialized = true;
        }

        public static Texture2D GetShiftStoneTexture(ShiftStones shiftStone)
        {
            if (shiftStoneTextures[shiftStone] == null)
            {
                LoadShiftStoneTexture(shiftStone);
            }

            return shiftStoneTextures[shiftStone];
        }

        private static AssetBundle LoadBundleFromFile(string path)
        {
            System.IO.Stream stream = StreamFromFile(path);
            Il2CppSystem.IO.Stream il2CppStream = ConvertToIl2CppStream(stream);
            AssetBundle bundle = AssetBundle.LoadFromStream(il2CppStream);

            stream.Close();
            il2CppStream.Close();
            
            return bundle;
        }

        private static Il2CppSystem.IO.Stream ConvertToIl2CppStream(System.IO.Stream stream)
        {
            Il2CppSystem.IO.MemoryStream Il2CppStream = new Il2CppSystem.IO.MemoryStream();

            const int bufferSize = 4096;
            byte[] managedBuffer = new byte[bufferSize];
            Il2CppStructArray<byte> Il2CppBuffer = new(managedBuffer);

            int bytesRead;
            while ((bytesRead = stream.Read(managedBuffer, 0, managedBuffer.Length)) > 0)
            {
                Il2CppBuffer = managedBuffer;
                Il2CppStream.Write(Il2CppBuffer, 0, bytesRead);
            }
            Il2CppStream.Flush();
            return Il2CppStream;
        }

        private static System.IO.MemoryStream StreamFromFile(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            return new MemoryStream(fileBytes);
        }
    }
}
