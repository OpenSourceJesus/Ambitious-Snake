using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using BeautifyEffect;
using uCP;
using Extensions;

public class ExtraSettings : SingletonMonoBehaviour<ExtraSettings>, ISavableAndLoadable
{
	public Slider contrastSlider;
	static float contrastDefault = 1.02f;
	[SaveAndLoadValue]
	public static float contrast = contrastDefault;
	public ColorPicker tintColorPicker;
	static Color tintDefault = new Color(0f, 0f, 0f, 0f);
	[SaveAndLoadValue]
	public static Color tint = tintDefault;
	public Slider daltonizeSlider;
	static float daltonizeDefault = 0f;
	[SaveAndLoadValue]
	public static float daltonize = daltonizeDefault;
	public Slider ditherSlider;
	static float ditherDefault = .02f;
	[SaveAndLoadValue]
	public static float dither = ditherDefault;
	public Slider bloomSlider;
	static float bloomDefault = 0f;
	[SaveAndLoadValue]
	public static float bloom = bloomDefault;
	public Slider sharpenSlider;
	static float sharpenDefault = 3f;
	[SaveAndLoadValue]
	public static float sharpen = sharpenDefault;
	[SaveAndLoadValue]
	public static bool unlocked;
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
		base.Awake ();
		
		ApplyContrast();
		ApplyTintColor();
		ApplyDaltonize();
		ApplyDither();
		ApplyBloomIntensity();
		ApplySharpen();
	}

	public virtual void Init ()
	{
		InitUI ();
		ApplyAll ();
	}

	public virtual void InitUI ()
	{
		contrastSlider.value = contrast;
		tintColorPicker.color = tint;
		tintColorPicker.UpdateUI ();
		// tintColorPicker.transform.GetChild(1).GetChild(1).GetComponent<GetColorInRect>().UpdateUI (tintColorPicker.hsv);
		daltonizeSlider.value = daltonize;
		ditherSlider.value = dither;
		bloomSlider.value = bloom;
		sharpenSlider.value = sharpen;
	}
	
	public void ResetAll ()
	{
		SetContrast (contrastDefault);
		SetTintColor (tintDefault);
		SetDaltonize (daltonizeDefault);
		SetDither (ditherDefault);
		SetBloomIntensity (bloomDefault);
		SetSharpen (sharpenDefault);
		contrastSlider.value = contrastDefault;
		tintColorPicker.color = tintDefault;
		tintColorPicker.UpdateUI ();
		// tintColorPicker.transform.GetChild(1).GetChild(1).GetComponent<GetColorInRect>().UpdateUI (tintColorPicker.hsv);
		daltonizeSlider.value = daltonizeDefault;
		ditherSlider.value = ditherDefault;
		bloomSlider.value = bloomDefault;
		sharpenSlider.value = sharpenDefault;
	}
	
	public static void ApplyAll ()
	{
		ApplyContrast ();
		ApplyTintColor ();
		ApplyDither ();
		ApplyDaltonize ();
		ApplyBloomIntensity ();
		ApplySharpen ();
	}
	
	static void ApplyContrast ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.contrast = contrast;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	static void ApplyTintColor ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.tintColor = tint;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	static void ApplyBloomIntensity ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.bloomIntensity = bloom;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	static void ApplySharpen ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.sharpen = sharpen;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	static void ApplyDaltonize ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.daltonize = daltonize;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	static void ApplyDither ()
	{
		if (Beautify.instance != null)
		{
			Beautify.instance.dither = dither;
			Beautify.instance.UpdateMaterialProperties();
		}
	}
	
	public void SetContrast (float contrast)
	{
		ExtraSettings.contrast = contrast;
		ApplyContrast ();
	}
	
	public void SetTintColor (Color tint)
	{
		ExtraSettings.tint = tint;
		ApplyTintColor ();
	}
	
	public void SetBloomIntensity (float bloom)
	{
		ExtraSettings.bloom = bloom;
		ApplyBloomIntensity ();
	}
	
	public void SetSharpen (float sharpen)
	{
		ExtraSettings.sharpen = sharpen;
		ApplySharpen ();
	}
	
	public void SetDaltonize (float daltonize)
	{
		ExtraSettings.daltonize = daltonize;
		ApplyDaltonize ();
	}
	
	public void SetDither (float dither)
	{
		ExtraSettings.dither = dither;
		ApplyDither ();
	}
}