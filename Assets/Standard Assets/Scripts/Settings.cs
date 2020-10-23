using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BeautifyEffect;
using Extensions;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class Settings : SingletonMonoBehaviour<Settings>, ISavableAndLoadable
	{
		public Dropdown resolutionsDropdown;
		static string[] resolutionsTexts = new string[0];
		public static Vector2Int[] resolutions = new Vector2Int[0];
		public static int bestResolutionIndex;
		[SaveAndLoadValue]
		public static int resolutionIndex;
		public Slider volumeSlider;
		static float volumeDefault = 1f;
		[SaveAndLoadValue]
		public static float volume = volumeDefault;
		public Toggle muteToggle;
		static bool muteDefault = false;
		[SaveAndLoadValue]
		public static bool mute = muteDefault;
		public Toggle fullscreenToggle;
		static bool fullscreenDefault = true;
		[SaveAndLoadValue]
		public static bool fullscreen = fullscreenDefault;
		public Toggle vSyncToggle;
		static bool vSyncDefault = false;
		[SaveAndLoadValue]
		public static bool vSync = vSyncDefault;
		public Dropdown antiAliasDropdown;
		static int[] antiAliasOptions = new int[] {0, 2, 4, 8};
		static int antiAliasDefault = 0;
		[SaveAndLoadValue]
		public static int antiAliasIndex = antiAliasDefault;
		public Slider brightnessSlider;
		static float brightnessDefault = 1.05f;
		[SaveAndLoadValue]
		public static float brightness = brightnessDefault;
		public Toggle backgroundRipplesToggle;
		static bool backgroundRipplesDefault = true;
		[SaveAndLoadValue]
		public static bool backgroundRipples = backgroundRipplesDefault;
		public Toggle frictionSparksToggle;
		static bool frictionSparksDefault = true;
		[SaveAndLoadValue]
		public static bool frictionSparks = frictionSparksDefault;
		public Button extraSettingsButton;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		[SerializeField]
		int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				GameManager.SetUniqueId (this);
				return;
			}
#endif
			base.Awake ();
		}

		public virtual void Init ()
		{
			SetResolutions ();
			bestResolutionIndex = GetBestResolutionIndex();
			InitUI ();
			ApplyAll ();
		}

		public virtual void InitUI ()
		{
			extraSettingsButton.interactable = ExtraSettings.unlocked;
			resolutionsDropdown.AddOptions(resolutionsTexts.ToList());
			volumeSlider.value = volume;
			muteToggle.isOn = mute;
			fullscreenToggle.isOn = fullscreen;
			resolutionsDropdown.value = resolutionIndex;
			vSyncToggle.isOn = vSync;
			antiAliasDropdown.value = antiAliasIndex;
			brightnessSlider.value = brightness;
			backgroundRipplesToggle.isOn = backgroundRipples;
			frictionSparksToggle.isOn = frictionSparks;
		}
		
		public virtual void ResetAll ()
		{
			SetVolume (volumeDefault);
			SetMute (muteDefault);
			SetFullscreen (fullscreenDefault);
			SetVSync (vSyncDefault);
			SetAntiAliasIndex (antiAliasDefault);
			SetBrightness (brightnessDefault);
			SetBackgroundRipples (backgroundRipplesDefault);
			SetResolution (bestResolutionIndex);
			SetFrictionSparks (frictionSparksDefault);
			volumeSlider.value = volumeDefault;
			muteToggle.isOn = muteDefault;
			fullscreenToggle.isOn = fullscreenDefault;
			fullscreenToggle.isOn = fullscreenDefault;
			resolutionsDropdown.value = bestResolutionIndex;
			vSyncToggle.isOn = vSyncDefault;
			antiAliasDropdown.value = antiAliasDefault;
			brightnessSlider.value = brightnessDefault;
			backgroundRipplesToggle.isOn = backgroundRipplesDefault;
			frictionSparksToggle.isOn = frictionSparksDefault;
		}
		
		public static void ApplyAll ()
		{
			ApplyVolume ();
			ApplyMute ();
			ApplyFullscreen ();
			ApplyResolution ();
			ApplyVSync ();
			ApplyAntiAliasIndex ();
			ApplyBrightness ();
			ApplyBackgroundRipples ();
			ApplyFrictionSparks ();
		}
		
		public static void ApplyBackgroundRipples ()
		{
			if (VectorGrid.instance != null)
				VectorGrid.instance.enabled = backgroundRipples;
		}
		
		public static void ApplyFrictionSparks ()
		{
			if (GameManager.GetSingleton<Snake>() != null)
				GameManager.GetSingleton<Snake>().sparkCreator.enabled = frictionSparks;
		}
		
		public static void ApplyVolume ()
		{
			AudioListener.volume = volume;
		}
		
		public static void ApplyMute ()
		{
			AudioListener.pause = mute;
		}
		
		public static void ApplyBrightness ()
		{
			if (Beautify.instance != null)
			{
				Beautify.instance.brightness = brightness;
				Beautify.instance.UpdateMaterialProperties();
			}
		}
		
		public static void ApplyVSync ()
		{
			QualitySettings.vSyncCount = vSync.GetHashCode();
		}
		
		public static void ApplyAntiAliasIndex ()
		{
			QualitySettings.antiAliasing = antiAliasOptions[antiAliasIndex];
		}
		
		public static void ApplyResolution ()
		{
			Screen.SetResolution((int) resolutions[resolutionIndex].x, (int) resolutions[resolutionIndex].y, Screen.fullScreen);
		}
		
		public static void ApplyFullscreen ()
		{
			Screen.fullScreen = fullscreen;
		}
		
		public void SetBackgroundRipples (bool backgroundRipples)
		{
			Settings.backgroundRipples = backgroundRipples;
			ApplyBackgroundRipples ();
		}
		
		public void SetFrictionSparks (bool frictionSparks)
		{
			Settings.frictionSparks = frictionSparks;
			ApplyFrictionSparks ();
		}
		
		public void SetVolume (float volume)
		{
			Settings.volume = volume;
			ApplyVolume ();
		}
		
		public void SetMute (bool mute)
		{
			Settings.mute = mute;
			ApplyMute ();
		}
		
		public void SetResolution (int resolutionIndex)
		{
			Settings.resolutionIndex = resolutionIndex;
			ApplyResolution ();
		}
		
		public void SetFullscreen (bool fullscreen)
		{
			Settings.fullscreen = fullscreen;
			ApplyFullscreen ();
		}
		
		public void SetVSync (bool vSync)
		{
			Settings.vSync = vSync;
			ApplyVSync ();
		}
		
		public void SetAntiAliasIndex (int antiAliasIndex)
		{
			Settings.antiAliasIndex = antiAliasIndex;
			ApplyAntiAliasIndex ();
		}
		
		public void SetBrightness (float brightness)
		{
			Settings.brightness = brightness;
			ApplyBrightness ();
		}
		
		public static void SetResolutions ()
		{
			resolutions = new Vector2Int[0];
			resolutionsTexts = new string[0];
			string option;
			foreach (Resolution resolution in Screen.resolutions)
			{
				Debug.Log(resolution);
				option = resolution.width + " X " + resolution.height;
				if (!resolutionsTexts.Contains(option))
				{
					resolutions = resolutions.Add(new Vector2Int(resolution.width, resolution.height));
					resolutionsTexts = resolutionsTexts.Add(option);
				}
			}
		}
		
		public static int GetBestResolutionIndex ()
		{
			int closestResolutionIndex = 0;
			float actualAspectRatio = (float) Screen.width / Screen.height;
			float closestAspectRatio = resolutions[closestResolutionIndex].x / resolutions[closestResolutionIndex].y;
			float closestAspectRatioDifference = Mathf.Abs(closestAspectRatio - actualAspectRatio);
			float closestAspectRatioPercentDifference = closestAspectRatioDifference / actualAspectRatio * 100;
			float aspectRatio;
			float aspectRatioDifference;
			float aspectRatioPercentDifference;
			for (int i = 1; i < resolutions.Length; i ++)
			{
				aspectRatio = resolutions[i].x / resolutions[i].y;
				aspectRatioDifference = Mathf.Abs(aspectRatio - actualAspectRatio);
				aspectRatioPercentDifference = aspectRatioDifference / actualAspectRatio * 100;
				if (aspectRatioPercentDifference <= closestAspectRatioPercentDifference)
				{
					closestResolutionIndex = i;
					closestAspectRatio = aspectRatio;
					closestAspectRatioDifference = aspectRatioDifference;
					closestAspectRatioPercentDifference = aspectRatioPercentDifference;
				}
			}
			return closestResolutionIndex;
		}
	}
}