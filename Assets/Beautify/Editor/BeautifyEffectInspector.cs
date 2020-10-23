using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BeautifyEffect {
				[CustomEditor (typeof(Beautify))]
				public class BeautifyEffectInspector : Editor {

								Beautify _effect;
								Texture2D _headerTexture;
								static GUIStyle titleLabelStyle;
								static Color titleColor;

								void OnEnable () {
												titleColor = EditorGUIUtility.isProSkin ? new Color (0.52f, 0.66f, 0.9f) : new Color (0.12f, 0.16f, 0.4f);
												_headerTexture = Resources.Load<Texture2D> ("beautifyHeader");
												_effect = (Beautify)target;
								}

								public override void OnInspectorGUI () {
												if (_effect == null)
																return;
												_effect.isDirty = false;

												EditorGUILayout.Separator ();
												GUI.skin.label.alignment = TextAnchor.MiddleCenter;  
												GUILayout.Label (_headerTexture, GUILayout.ExpandWidth (true));
												GUI.skin.label.alignment = TextAnchor.MiddleLeft;  
												if (!_effect.enabled) {
																EditorGUILayout.HelpBox ("Beautify disabled.", MessageType.Info);
												}
												EditorGUILayout.Separator ();

												EditorGUILayout.BeginHorizontal ();
												DrawLabel ("General Settings");
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Quality", "The mobile variant is simply less accurate but faster."), GUILayout.Width (90));
												_effect.quality = (BEAUTIFY_QUALITY)EditorGUILayout.EnumPopup (_effect.quality);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Preset", "Quick configurations."), GUILayout.Width (90));
												_effect.preset = (BEAUTIFY_PRESET)EditorGUILayout.EnumPopup (_effect.preset);
												EditorGUILayout.EndHorizontal ();
			
												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Compare Mode", "Shows a side by side comparison."), GUILayout.Width (90));
												_effect.compareMode = EditorGUILayout.Toggle (_effect.compareMode);
												if (GUILayout.Button ("Help", GUILayout.Width (50))) {
																EditorUtility.DisplayDialog ("Help", "Beautify is a full-screen image processing effect that makes your scenes crisp, vivid and intense.\n\nMove the mouse over a setting for a short description or read the provided documentation (PDF) for details and tips.\n\nVisit kronnect.com for support and questions.\n\nPlease rate Beautify on the Asset Store! Thanks.", "Ok");
												}
												EditorGUILayout.EndHorizontal ();

												if (_effect.compareMode) {
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Angle", "Angle of the separator line."), GUILayout.Width (90));
																_effect.compareLineAngle = EditorGUILayout.Slider (_effect.compareLineAngle, -Mathf.PI, Mathf.PI);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Width", "Width of the separator line."), GUILayout.Width (90));
																_effect.compareLineWidth = EditorGUILayout.Slider (_effect.compareLineWidth, 0.0001f, 0.05f);
																EditorGUILayout.EndHorizontal ();
												}

												EditorGUILayout.Separator ();
												DrawLabel ("Image Enhancement");

									if (_effect.cameraEffect != null && !_effect.cameraEffect.allowHDR) {
																EditorGUILayout.HelpBox ("Some effects, like dither and bloom, works better with HDR enabled. Check your camera setting.", MessageType.Warning);
												}

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Sharpen", "Sharpen intensity."), GUILayout.Width (90));
												_effect.sharpen = EditorGUILayout.Slider (_effect.sharpen, 0f, 12f);
												EditorGUILayout.EndHorizontal ();

												if (_effect.cameraEffect != null && !_effect.cameraEffect.orthographic) {
																
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Min/Max Depth", "Any pixel outside this depth range won't be affected by sharpen. Reduce range to create a depth-of-field-like effect."), GUILayout.Width (120));
																float minDepth = _effect.sharpenMinDepth;
																float maxDepth = _effect.sharpenMaxDepth;
																EditorGUILayout.MinMaxSlider (ref minDepth, ref maxDepth, 0f, 1.1f);
																_effect.sharpenMinDepth = minDepth;
																_effect.sharpenMaxDepth = maxDepth;
																EditorGUILayout.EndHorizontal ();

																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Depth Threshold", "Reduces sharpen if depth difference around a pixel exceeds this value. Useful to prevent artifacts around wires or thin objects."), GUILayout.Width (120));
																_effect.sharpenDepthThreshold = EditorGUILayout.Slider (_effect.sharpenDepthThreshold, 0f, 0.05f);
																EditorGUILayout.EndHorizontal ();
												}

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("   Luminance Relax.", "Soften sharpen around a pixel with high contrast. Reduce this value to remove ghosting and protect fine drawings or wires over a flat surface."), GUILayout.Width (120));
												_effect.sharpenRelaxation = EditorGUILayout.Slider (_effect.sharpenRelaxation, 0f, 0.2f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("   Clamp", "Maximum pixel adjustment."), GUILayout.Width (120));
												_effect.sharpenClamp = EditorGUILayout.Slider (_effect.sharpenClamp, 0f, 1f);
												EditorGUILayout.EndHorizontal ();
			
												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("   Motion Sensibility", "Increase to reduce sharpen to simulate a cheap motion blur and to reduce flickering when camera rotates or moves. This slider controls the amount of camera movement/rotation that contributes to sharpen reduction. Set this to 0 to disable this feature."), GUILayout.Width (120));
												_effect.sharpenMotionSensibility = EditorGUILayout.Slider (_effect.sharpenMotionSensibility, 0f, 1f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Dither", "Simulates more colors than RGB quantization can produce. Removes banding artifacts in gradients, like skybox. This setting controls the dithering strength."), GUILayout.Width (90));
												_effect.dither = EditorGUILayout.Slider (_effect.dither, 0f, 0.2f);
												EditorGUILayout.EndHorizontal ();
	
												if (_effect.cameraEffect != null && !_effect.cameraEffect.orthographic) {
																
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Min Depth", "Will only remove bands on pixels beyond this depth. Useful if you only want to remove sky banding (set this to 0.99)"), GUILayout.Width (120));
																_effect.ditherDepth = EditorGUILayout.Slider (_effect.ditherDepth, 0f, 1f);
																EditorGUILayout.EndHorizontal ();

												}

												EditorGUILayout.Separator ();
												DrawLabel ("Color Grading");

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Vibrance", "Improves pixels color depending on their saturation."), GUILayout.Width (90));
												_effect.saturate = EditorGUILayout.Slider (_effect.saturate, -2f, 3f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Daltonize", "Similar to vibrance but mostly accentuate primary red, green and blue colors to compensate protanomaly (red deficiency), deuteranomaly (green deficiency) and tritanomaly (blue deficiency). This effect does not shift color hue hence it won't help completely red, green or blue color blindness. The effect will vary depending on each subject so this effect should be enabled on user demand."), GUILayout.Width (90));
												_effect.daltonize = EditorGUILayout.Slider (_effect.daltonize, 0f, 2f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Tint", "Blends image with an optional color. Alpha specifies intensity."), GUILayout.Width (90));
												_effect.tintColor = EditorGUILayout.ColorField (_effect.tintColor);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Contrast", "Final image contrast adjustment. Allows you to create more vivid images."), GUILayout.Width (90));
												_effect.contrast = EditorGUILayout.Slider (_effect.contrast, 0.5f, 1.5f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Brightness", "Final image brightness adjustment."), GUILayout.Width (90));
												_effect.brightness = EditorGUILayout.Slider (_effect.brightness, 0f, 2f);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.Separator ();
												DrawLabel ("Extra FX");

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Bloom", "Produces fringes of light extending from the borders of bright areas, contributing to the illusion of an extremely bright light overwhelming the camera or eye capturing the scene."), GUILayout.Width (90));
												_effect.bloom = EditorGUILayout.Toggle (_effect.bloom);
												if (_effect.bloom) {
																GUILayout.Label (new GUIContent ("Debug", "Enable to see bloom/anamorphic channel."));
																_effect.bloomDebug = EditorGUILayout.Toggle (_effect.bloomDebug, GUILayout.Width (40));
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Intensity", "Bloom multiplier."), GUILayout.Width (90));
																_effect.bloomIntensity = EditorGUILayout.Slider (_effect.bloomIntensity, 0f, 10f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Threshold", "Brightness sensibility."), GUILayout.Width (90));
																_effect.bloomThreshold = EditorGUILayout.Slider (_effect.bloomThreshold, 0f, 5f);
																if (_effect.quality == BEAUTIFY_QUALITY.BestQuality) {
																				EditorGUILayout.EndHorizontal ();
																				EditorGUILayout.BeginHorizontal ();
																				GUILayout.Label (new GUIContent ("   Reduce Flicker", "Enables an additional filter to reduce excess of flicker."), GUILayout.Width (90));
																				_effect.bloomAntiflicker = EditorGUILayout.Toggle (_effect.bloomAntiflicker);
																				EditorGUILayout.EndHorizontal ();
																				EditorGUILayout.BeginHorizontal ();
																				GUILayout.Label (new GUIContent ("   Customize", "Edit bloom style parameters."), GUILayout.Width (90));
																				_effect.bloomCustomize = EditorGUILayout.Toggle (_effect.bloomCustomize);
																				if (_effect.bloomCustomize) {
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label ("   Presets", GUILayout.Width (90));
																								if (GUILayout.Button ("Focused")) {
																												_effect.SetBloomWeights (1f, 0.9f, 0.75f, 0.6f, 0.35f, 0.1f);
																								}
																								if (GUILayout.Button ("Regular")) {
																												_effect.SetBloomWeights (0.85f, 0.95f, 1f, 0.9f, 0.77f, 0.6f);
																								}
																								if (GUILayout.Button ("Blurred")) {
																												_effect.SetBloomWeights (0.2f, 0.4f, 0.6f, 0.75f, 0.9f, 1.0f);
																								}
																								EditorGUILayout.EndHorizontal ();
						
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 1", "First layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight0 = EditorGUILayout.Slider (_effect.bloomWeight0, 0f, 1f);
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 2", "Second layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight1 = EditorGUILayout.Slider (_effect.bloomWeight1, 0f, 1f);
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 3", "Third layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight2 = EditorGUILayout.Slider (_effect.bloomWeight2, 0f, 1f);
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 4", "Fourth layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight3 = EditorGUILayout.Slider (_effect.bloomWeight3, 0f, 1f);
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 5", "Fifth layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight4 = EditorGUILayout.Slider (_effect.bloomWeight4, 0f, 1f);
																								EditorGUILayout.EndHorizontal ();
																								EditorGUILayout.BeginHorizontal ();
																								GUILayout.Label (new GUIContent ("   Weight 6", "Sixth layer bloom weight."), GUILayout.Width (90));
																								_effect.bloomWeight5 = EditorGUILayout.Slider (_effect.bloomWeight5, 0f, 1f);
																				}
																}
												}
												EditorGUILayout.EndHorizontal ();
			

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Anamorphic F.", "Also known as JJ Abrams flares, adds spectacular light streaks to your scene."), GUILayout.Width (90));
												if (_effect.quality != BEAUTIFY_QUALITY.BestQuality) {
																GUILayout.Label ("(only available for 'best quality' setting)");
												} else {
																_effect.anamorphicFlares = EditorGUILayout.Toggle (_effect.anamorphicFlares);
												}
												if (_effect.anamorphicFlares && _effect.quality == BEAUTIFY_QUALITY.BestQuality) {
																if (!_effect.bloom) {
																				GUILayout.Label (new GUIContent ("Debug", "Enable to see bloom/anamorphic flares channel."));
																				_effect.bloomDebug = EditorGUILayout.Toggle (_effect.bloomDebug, GUILayout.Width (40));
																}
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Intensity", "Flares light multiplier."), GUILayout.Width (90));
																_effect.anamorphicFlaresIntensity = EditorGUILayout.Slider (_effect.anamorphicFlaresIntensity, 0f, 10f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Threshold", "Brightness sensibility."), GUILayout.Width (90));
																_effect.anamorphicFlaresThreshold = EditorGUILayout.Slider (_effect.anamorphicFlaresThreshold, 0f, 5f);
																EditorGUILayout.EndHorizontal ();
																if (_effect.quality == BEAUTIFY_QUALITY.BestQuality) {
																				EditorGUILayout.BeginHorizontal ();
																				GUILayout.Label (new GUIContent ("   Reduce Flicker", "Enables an additional filter to reduce excess of flicker."), GUILayout.Width (90));
																				_effect.anamorphicFlaresAntiflicker = EditorGUILayout.Toggle (_effect.anamorphicFlaresAntiflicker);
																				EditorGUILayout.EndHorizontal ();
																}
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Spread", "Amplitude of the flares."), GUILayout.Width (90));
																_effect.anamorphicFlaresSpread = EditorGUILayout.Slider (_effect.anamorphicFlaresSpread, 0.1f, 2f);
																GUILayout.Label ("Vertical");
																_effect.anamorphicFlaresVertical = EditorGUILayout.Toggle (_effect.anamorphicFlaresVertical, GUILayout.Width (20));
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Tint", "Optional tint color for the anamorphic flares. Use color alpha component to blend between original color and the tint."), GUILayout.Width (90));
																_effect.anamorphicFlaresTint = EditorGUILayout.ColorField (_effect.anamorphicFlaresTint);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Lens Dirt", "Enables lens dirt effect which intensifies when looking to a light (uses the nearest light to camera). You can assign other dirt textures directly to the shader material with name 'Beautify'."), GUILayout.Width (90));
												_effect.lensDirt = EditorGUILayout.Toggle (_effect.lensDirt);
												EditorGUILayout.EndHorizontal ();
												if (_effect.lensDirt) {
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Dirt Texture", "Texture used for the lens dirt effect."), GUILayout.Width (90));
																_effect.lensDirtTexture = (Texture2D)EditorGUILayout.ObjectField (_effect.lensDirtTexture, typeof(Texture2D), false);
																if (GUILayout.Button ("?", GUILayout.Width (20))) {
																				EditorUtility.DisplayDialog ("Lens Dirt Texture", "You can find additional lens dirt textures inside \nBeautify/Resources/Textures folder.", "Ok");
																}
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Threshold", "This slider controls the visibility of lens dirt. A high value will make lens dirt only visible when looking directly towards a light source. A lower value will make lens dirt visible all time."), GUILayout.Width (90));
																_effect.lensDirtThreshold = EditorGUILayout.Slider (_effect.lensDirtThreshold, 0f, 1f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Intensity", "This slider controls the maximum brightness of lens dirt effect."), GUILayout.Width (90));
																_effect.lensDirtIntensity = EditorGUILayout.Slider (_effect.lensDirtIntensity, 0f, 1f);
																EditorGUILayout.EndHorizontal ();
												}

												if (_effect.vignetting || _effect.frame || _effect.outline || _effect.nightVision || _effect.thermalVision) {
																EditorGUILayout.HelpBox ("Customize the effects below using color picker. Alpha has special meaning depending on effect. Read the tooltip moving the mouse over the effect name.", MessageType.Info);
												}

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Vignetting", "Enables colored vignetting effect. Color alpha specifies intensity of effect."), GUILayout.Width (90));
												_effect.vignetting = EditorGUILayout.Toggle (_effect.vignetting);
			
												if (_effect.vignetting) {
																GUILayout.Label (new GUIContent ("Color", "The color for the vignetting effect. Alpha specifies intensity of effect."), GUILayout.Width (40));
																_effect.vignettingColor = EditorGUILayout.ColorField (_effect.vignettingColor);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Circular Shape", "Ignores screen aspect ratio showing a circular shape."), GUILayout.Width (90));
																_effect.vignettingCircularShape = EditorGUILayout.Toggle (_effect.vignettingCircularShape);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Frame", "Enables colored frame effect. Color alpha specifies intensity of effect."), GUILayout.Width (90));
												_effect.frame = EditorGUILayout.Toggle (_effect.frame);
			
												if (_effect.frame) {
																GUILayout.Label (new GUIContent ("Color", "The color for the frame effect. Alpha specifies intensity of effect."), GUILayout.Width (40));
																_effect.frameColor = EditorGUILayout.ColorField (_effect.frameColor);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Outline", "Enables outline (edge detection) effect. Color alpha specifies edge detection threshold."), GUILayout.Width (90));
												_effect.outline = EditorGUILayout.Toggle (_effect.outline);
			
												if (_effect.outline) {
																GUILayout.Label (new GUIContent ("Color", "The color for the outline. Alpha specifies edge detection threshold."), GUILayout.Width (40));
																_effect.outlineColor = EditorGUILayout.ColorField (_effect.outlineColor);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Sepia", "Enables sepia color effect."), GUILayout.Width (90));
												_effect.sepia = EditorGUILayout.Toggle (_effect.sepia, GUILayout.Width (40));
												if (_effect.sepia) {
																GUILayout.Label ("Intensity");
																_effect.sepiaIntensity = EditorGUILayout.Slider (_effect.sepiaIntensity, 0f, 1f);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Night Vision", "Enables night vision effect. Color alpha controls intensity. For a better result, enable Vignetting and set its color to (0,0,0,32)."), GUILayout.Width (90));
												_effect.nightVision = EditorGUILayout.Toggle (_effect.nightVision);
												if (_effect.nightVision) {
																GUILayout.Label (new GUIContent ("Color", "The color for the night vision effect. Alpha controls intensity."), GUILayout.Width (40));
																_effect.nightVisionColor = EditorGUILayout.ColorField (_effect.nightVisionColor);
												}
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Thermal Vision", "Enables thermal vision effect."), GUILayout.Width (90));
												_effect.thermalVision = EditorGUILayout.Toggle (_effect.thermalVision);
												EditorGUILayout.EndHorizontal ();

												EditorGUILayout.Separator ();
												DrawLabel ("Beta FX");

			
												EditorGUILayout.BeginHorizontal ();
												GUILayout.Label (new GUIContent ("Depth of Field", "Blurs the image based on distance to focus point."), GUILayout.Width (90));
												_effect.depthOfField = EditorGUILayout.Toggle (_effect.depthOfField);
												if (_effect.depthOfField) {
																GUILayout.Label (new GUIContent ("Debug", "Enable to see depth of field focus area."));
																_effect.depthOfFieldDebug = EditorGUILayout.Toggle (_effect.depthOfFieldDebug, GUILayout.Width (40));
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Focus Target", "Dynamically focus target."), GUILayout.Width (90));
																_effect.depthOfFieldTargetFocus = (Transform)EditorGUILayout.ObjectField (_effect.depthOfFieldTargetFocus, typeof(Transform), true);
																EditorGUILayout.EndHorizontal ();
																if (_effect.depthOfFieldTargetFocus == null) {
																				EditorGUILayout.BeginHorizontal ();
																				GUILayout.Label (new GUIContent ("   Focus Distance", "Distance to focus point."), GUILayout.Width (120));
																				_effect.depthOfFieldDistance = EditorGUILayout.Slider (_effect.depthOfFieldDistance, 1f, 100f);
																				EditorGUILayout.EndHorizontal ();
																}
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Focus Speed", "1=immediate focus on distance or target."), GUILayout.Width (_effect.depthOfFieldTargetFocus == null ? 120 : 90));
																_effect.depthOfFieldFocusSpeed = EditorGUILayout.Slider (_effect.depthOfFieldFocusSpeed, 0.001f, 1f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Focal Length", "Focal length of the virtual lens."), GUILayout.Width (_effect.depthOfFieldTargetFocus == null ? 120 : 90));
																_effect.depthOfFieldFocalLength = EditorGUILayout.Slider (_effect.depthOfFieldFocalLength, 0.005f, 0.5f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Aperture", "Diameter of the aperture (mm)."), GUILayout.Width (90));
																GUILayout.Label ("f/", GUILayout.Width (15));
																_effect.depthOfFieldAperture = EditorGUILayout.FloatField (_effect.depthOfFieldAperture, GUILayout.Width (40));
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Foreground Blur", "Enables blur in front of focus object."), GUILayout.Width (120));
																_effect.depthOfFieldForegroundBlur = EditorGUILayout.Toggle (_effect.depthOfFieldForegroundBlur, GUILayout.Width (40));
																if (_effect.depthOfFieldForegroundBlur) {
																				GUILayout.Label (new GUIContent ("Offset", "Distance from focus plane for foreground blur."), GUILayout.Width (50));
																				_effect.depthOfFieldForegroundDistance = EditorGUILayout.FloatField (_effect.depthOfFieldForegroundDistance, GUILayout.Width (50));
																}
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Bokeh Threshold", "Determines which bright spots will be augmented in defocused areas."), GUILayout.Width (120));
																_effect.depthOfFieldBokehThreshold = EditorGUILayout.Slider (_effect.depthOfFieldBokehThreshold, 0.5f, 3f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Bokeh Intensity", "Intensity multiplier for bright spots in defocused areas."), GUILayout.Width (120));
																_effect.depthOfFieldBokehIntensity = EditorGUILayout.Slider (_effect.depthOfFieldBokehIntensity, 0f, 8f);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Downsampling", "Reduces screen buffer size to improve performance."), GUILayout.Width (120));
																_effect.depthOfFieldDownsampling = EditorGUILayout.IntSlider (_effect.depthOfFieldDownsampling, 1, 5);
																EditorGUILayout.EndHorizontal ();
																EditorGUILayout.BeginHorizontal ();
																GUILayout.Label (new GUIContent ("   Sample Count", "Determines the maximum number of samples to be gathered in the effect."), GUILayout.Width (120));
																_effect.depthOfFieldMaxSamples = EditorGUILayout.IntSlider (_effect.depthOfFieldMaxSamples, 2, 16);
																GUILayout.Label ("(" + ((_effect.depthOfFieldMaxSamples - 1) * 2 + 1) + " samples)");
												}
												EditorGUILayout.EndHorizontal ();

												if (_effect.isDirty) {
																EditorUtility.SetDirty (target);
												}


								}

								void DrawLabel (string s) {
												if (titleLabelStyle == null) {
																GUIStyle skurikenModuleTitleStyle = "ShurikenModuleTitle";
																titleLabelStyle = new GUIStyle (skurikenModuleTitleStyle);
																titleLabelStyle.contentOffset = new Vector2 (5f, -2f);
																titleLabelStyle.normal.textColor = titleColor;
																titleLabelStyle.fixedHeight = 22;
																titleLabelStyle.fontStyle = FontStyle.Bold;
												}

												GUILayout.Label (s, titleLabelStyle);
								}

				}

}
