using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using AmbitiousSnake;

public class UIControlManager : SingletonMonoBehaviour<UIControlManager>
{
	public _Selectable currentSelected;
	public ComplexTimer colorMultiplier;
	public static List<_Selectable> selectables = new List<_Selectable>();
	Vector2 inputDirection;
	Vector2 previousInputDirection;
	public Timer repeatTimer;
	public float angleEffectiveness;
	public float distanceEffectiveness;
	_Selectable selectable;
	bool inControlMode;
	bool controllingWithJoystick;
	Vector2 previousMousePosition;
	public float autoScrollAmount;

	public override void Awake ()
	{
		base.Awake ();
		repeatTimer.onFinished += delegate { _HandleChangeSelected (); ControlSelected (); };
	}

	public virtual void OnDestroy ()
	{
		repeatTimer.onFinished -= delegate { _HandleChangeSelected (); ControlSelected (); };
	}

	public virtual void Update ()
	{
		if (currentSelected != null)
		{
			if (!selectables.Contains(currentSelected) || !currentSelected.selectable.IsInteractable())
			{
				ColorSelected (currentSelected, 1);
				HandleChangeSelected (false);
			}
			ColorSelected (currentSelected, colorMultiplier.GetValue());
			HandleMouseInput ();
			HandleMovementInput ();
			HandleSubmitSelected ();
		}
		else
			HandleChangeSelected (false);
	}

	public virtual void AddSelectable (_Selectable selectable)
	{
		selectables.Add(selectable);
	}

	public virtual void RemoveSelectable (_Selectable selectable)
	{
		selectables.Remove(selectable);
	}

	public virtual bool IsMousedOverSelectable (_Selectable selectable)
	{
		return IsMousedOverRectTransform(selectable.rectTrs, selectable.canvas, selectable.canvasRectTrs);
	}

	public virtual bool IsMousedOverRectTransform (RectTransform rectTrs, Canvas canvas, RectTransform canvasRectTrs)
	{
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetWorldRect().ToNormalizedPosition(Input.mousePosition));
		else
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetWorldRect().ToNormalizedPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
	}

	public virtual void HandleMouseInput ()
	{
		bool justCanceledControlMode = false;
		if ((Input.GetMouseButtonDown(0) || (Vector2) Input.mousePosition != previousMousePosition) && controllingWithJoystick)
		{
			controllingWithJoystick = false;
		}
		if (Input.GetMouseButtonUp(0) && !controllingWithJoystick)
		{
			inControlMode = false;
			justCanceledControlMode = true;
		}
		if (!controllingWithJoystick)
		{
			foreach (_Selectable selectable in selectables)
			{
				if (IsMousedOverSelectable(selectable))
				{
					if (justCanceledControlMode || currentSelected != selectable)
					{
						ChangeSelected (selectable);
						return;
					}
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			_Slider slider = currentSelected.GetComponent<_Slider>();
			if (slider != null)
			{
				Vector2 closestPointToMouseCanvasNormalized = new Vector2();
				if (currentSelected.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					if (selectable != null)
						closestPointToMouseCanvasNormalized = slider.slidingArea.GetRectInCanvasNormalized(selectable.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetWorldRect().ToNormalizedPosition(Input.mousePosition));
				}
				else
				{
					if (selectable != null)
						closestPointToMouseCanvasNormalized = slider.slidingArea.GetRectInCanvasNormalized(selectable.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetWorldRect().ToNormalizedPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
				}
				float normalizedValue = slider.slidingArea.GetRectInCanvasNormalized(slider.canvasRectTrs).ToNormalizedPosition(closestPointToMouseCanvasNormalized).x;
				slider.slider.value = Mathf.Lerp(slider.slider.minValue, slider.slider.maxValue, normalizedValue);
				if (slider.snapValues.Length > 0)
					slider.slider.value = MathfExtensions.GetClosestNumber(slider.slider.value, slider.snapValues);
			}
		}
		previousMousePosition = Input.mousePosition;
	}

	public virtual void HandleMovementInput ()
	{
		inputDirection = InputManager.UIMovementInput;
		// if (inputDirection.magnitude > InputManager.Settings.joystickDeadzone)
		// {
			controllingWithJoystick = true;
			// if (previousInputDirection.magnitude <= InputManager.Settings.joystickDeadzone)
			{
				HandleChangeSelected (true);
				ControlSelected ();
				repeatTimer.timeRemaining = repeatTimer.duration;
				repeatTimer.Start ();
			}
		// }
		// else
			repeatTimer.Stop ();
		previousInputDirection = inputDirection;
	}

	public virtual void _HandleChangeSelected ()
	{
		HandleChangeSelected (true);
	}

	public virtual void HandleChangeSelected (bool useInputDirection = true)
	{
		if (selectables.Count == 0 || inControlMode)
			return;
		List<_Selectable> otherSelectables = new List<_Selectable>();
		otherSelectables.AddRange(selectables);
		otherSelectables.Remove(currentSelected);
		float selectableAttractiveness;
		_Selectable nextSelected = otherSelectables[0];
		float highestSelectableAttractiveness = GetAttractivenessOfSelectable(nextSelected, useInputDirection);
		_Selectable selectable;
		for (int i = 1; i < otherSelectables.Count; i ++)
		{
			selectable = otherSelectables[i];
			selectableAttractiveness = GetAttractivenessOfSelectable(selectable, useInputDirection);
			if (selectableAttractiveness > highestSelectableAttractiveness)
			{
				highestSelectableAttractiveness = selectableAttractiveness;
				nextSelected = selectable;
			}
		}
		ChangeSelected (nextSelected);
	}

	public virtual void ChangeSelected (_Selectable selectable)
	{
		if (inControlMode)
			return;
		if (currentSelected != null)
			ColorSelected (currentSelected, 1);
		currentSelected = selectable;
		currentSelected.selectable.Select();
		colorMultiplier.JumpToStart ();
		if (currentSelected.scrollbarThatMovesMe != null)
		{
			if (!RectExtensions.IsEncapsulating(currentSelected.container.GetWorldRect(), currentSelected.rectTrs.GetWorldRect(), true))
			{
				currentSelected.scrollbarThatMovesMe.value = 0;
				Canvas.ForceUpdateCanvases();
				while (!RectExtensions.IsEncapsulating(currentSelected.container.GetWorldRect(), currentSelected.rectTrs.GetWorldRect(), true))
				{
					currentSelected.scrollbarThatMovesMe.value += autoScrollAmount;
					Canvas.ForceUpdateCanvases();
				}
			}
		}
	}

	public virtual void HandleSubmitSelected ()
	{
		bool submitButtonPressed = InputManager.SubmitInput;
		if (submitButtonPressed)
			controllingWithJoystick = true;
		if (submitButtonPressed)// || (IsMousedOverSelectable(currentSelected) && Input.GetMouseButtonDown(0)))
		{
			Button button = currentSelected.GetComponent<Button>();
			if (button != null)
				button.onClick.Invoke();
			else
			{
				Toggle toggle = currentSelected.GetComponent<Toggle>();
				if (toggle != null)
					toggle.isOn = !toggle.isOn;
				else
				{
					_Slider slider = currentSelected.GetComponent<_Slider>();
					if (slider != null)
						inControlMode = !inControlMode;
				}
			}
		}
	}

	public virtual void ControlSelected ()
	{
		if (!inControlMode)
			return;
		_Slider slider = currentSelected.GetComponent<_Slider>();
		if (slider != null)
		{
			slider.indexOfCurrentSnapValue = Mathf.Clamp(slider.indexOfCurrentSnapValue + MathfExtensions.Sign(inputDirection.x), 0, slider.snapValues.Length - 1);
			slider.slider.value = slider.snapValues[slider.indexOfCurrentSnapValue];
		}
	}

	public virtual float GetAttractivenessOfSelectable (_Selectable selectable, bool useInputDirection = true)
	{
		if (!selectables.Contains(selectable) || !selectable.selectable.IsInteractable())
			return -Mathf.Infinity;
		float attractiveness = selectable.priority;
		if (useInputDirection)
		{
			Vector2 directionToSelectable = GetDirectionToSelectable(selectable);
			attractiveness += (Vector2.Dot(inputDirection.normalized, directionToSelectable.normalized) * angleEffectiveness) - (directionToSelectable.magnitude * distanceEffectiveness);
		}
		return attractiveness;
	}

	public virtual Vector2 GetDirectionToSelectable (_Selectable selectable)
	{
		return selectable.rectTrs.GetCenterInCanvasNormalized(selectable.canvasRectTrs) - currentSelected.rectTrs.GetCenterInCanvasNormalized(currentSelected.canvasRectTrs);
	}

	public virtual void ColorSelected (_Selectable selectable, float colorMultiplier)
	{
		ColorBlock colors = selectable.selectable.colors;
		colors.colorMultiplier = colorMultiplier;
		selectable.selectable.colors = colors;
	}
}