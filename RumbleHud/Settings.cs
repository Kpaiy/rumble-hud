using Il2CppPlayFab.ClientModels;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UIFramework;
using UnityEngine;

namespace RumbleHud
{
	public enum HostIndicatorOptions
	{
		[Display(Name = "No Host Indicator")]
		None,
		Text,
		Icon,
		[Display(Name = "Text and Icon")]
		Both
	}

	public class Settings
	{
		

		private static Settings Instance { get; set; }


		public float HudScale { get; set; }
		public HostIndicatorOptions HostIndicator { get; set; }
		public bool HideSolo { get; set; }
		public bool LockControls { get; set; } // When this is true, keyboard controls cannot change the HUD.

		public static Settings FromJson(string jsonString)
		{
			return JsonUtility.FromJson<Settings>(jsonString);
		}

		public static Settings Initialize()
		{
			var newInstance = new Settings
			{
				HudScale = 1.0f,
				HostIndicator = HostIndicatorOptions.Text,
				HideSolo = false,
				LockControls = false,
			};
			Instance = newInstance;
			return newInstance;
		}

		public static Settings FromXmlFile(string filePath)
		{
			var serializer = new XmlSerializer(typeof(Settings));
			using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

			Settings settings = (Settings)serializer.Deserialize(fileStream);

			Instance = settings;
			return settings;
		}

		public void ToXmlFile(string filePath)
		{
			using var writer = new StreamWriter(filePath);
			var serializer = new XmlSerializer(this.GetType());
			serializer.Serialize(writer, this);
			writer.Flush();
		}
	}

	internal static class Preferences
	{
		//loaded preference values
		public static bool IsVisible { get; set; }
		public static float HudScale { get; set; }
		public static HostIndicatorOptions HostIndicator { get; set; }
		public static bool HideSolo { get; set; }
		public static bool LockControls { get; set; } // When this is true, keyboard controls cannot change the HUD.


		private const string CONFIG_FILE = "config.cfg";
		private const string USER_DATA = "UserData/RumbleHudData/";
		internal static MelonPreferences_Category RumbleHudCategory;
		internal static MelonPreferences_Entry<bool> PrefIsVisible;
		internal static MelonPreferences_Entry<float> PrefHudScale;
		internal static MelonPreferences_Entry<HostIndicatorOptions> PrefHostIndicator;
		internal static MelonPreferences_Entry<bool> PrefHideSolo;
		internal static MelonPreferences_Entry<bool> PrefLockControls;

		internal static MelonPreferences_Category HiddenCategory;
		internal static MelonPreferences_Entry<bool> XmlMigrated;

		internal static void InitPrefs()
		{
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);

			RumbleHudCategory = MelonPreferences.CreateCategory("RumbleHUD", "Rumble HUD");
			RumbleHudCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			PrefIsVisible = RumbleHudCategory.CreateEntry("HudIsVisible", true, "Display Hud", "Display or hide HUD");
			PrefHudScale = RumbleHudCategory.CreateEntry("HudScale", 1.0f, "Hud Scale", "The size of the HUD. Keep it strictly positive. Control in-game using - and =.");
			PrefHostIndicator = RumbleHudCategory.CreateEntry("HostIndicator", HostIndicatorOptions.Text, "Host Indicator", "How to indicate who is host on the HUD. Cycle in-game using O.");
			PrefHideSolo = RumbleHudCategory.CreateEntry("HideSolo", false, "Hide Solo", "Whether to auto-hide the HUD when you are the only player. Cannot be set in-game.");
			PrefLockControls = RumbleHudCategory.CreateEntry("LockControls", false, "Lock Controls", "When this is true, keyboard controls are disabled, preventing accidental changes.");

			HiddenCategory = MelonPreferences.CreateCategory("HiddenCategory", "Hidden Category");
			HiddenCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			XmlMigrated = HiddenCategory.CreateEntry("XmlMigrated", false, "XML Settings Migrated", "For initial migration into MelonPreferences. Setting to false would look for the XML file and apply the settings to the preferences");

			ApplyPrefs();
		}


		internal static void CheckIfXmlMigrated()
		{
			if (XmlMigrated.Value)
				return;
			if (!File.Exists(@"UserData\RumbleHud.xml"))
				return;
			try
			{
				Melon<Core>.Logger.Msg("Not previously migrated from XML. Attempting migration");
				Settings oldSettings = Settings.FromXmlFile(@"UserData\RumbleHud.xml");

				PrefHudScale.Value = oldSettings.HudScale;
				PrefHostIndicator.Value = oldSettings.HostIndicator;
				PrefHideSolo.Value = oldSettings.HideSolo;
				PrefLockControls.Value = oldSettings.LockControls;
				ApplyPrefs();
				XmlMigrated.Value = true;
				File.Delete(@"UserData\RumbleHud.xml");
			}
			catch (Exception ex) 
			{
				Melon<Core>.Logger.Msg($"Error in XML to Melonpreferences migration attempt: {ex.Message}");
			}
		}
		internal static void ApplyPrefs()
		{
			IsVisible = PrefIsVisible.Value;
			HudScale = PrefHudScale.Value;
			HostIndicator = PrefHostIndicator.Value;
			HideSolo = PrefHideSolo.Value;
			LockControls = PrefLockControls.Value;

			Hud.SetVisible(IsVisible);
			Hud.SetScale(HudScale);
			Hud.ClearPlayerUi();
		}

	}
}
 