using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Extensions;
using CoroutineWithData = ThreadingUtilities.CoroutineWithData;

namespace AmbitiousSnake
{
	[ExecuteAlways]
	public class LevelEditor : SingletonMonoBehaviour<LevelEditor>
	{
		public CollisionMatrix collisionMatrix;
		int brushSize = 1;
		BuildMethod buildMethod = BuildMethod.Replace;
		int currentActionIndex = 0;
		PartOfLevelEditor whatToBuild;
		PartOfLevelEditor currentPart;
		List<PartOfLevelEditor> whatToErase = new List<PartOfLevelEditor>();
		List<PartOfLevelEditor> whatToSelect = new List<PartOfLevelEditor>();
		List<PartOfLevelEditor> whatToMove = new List<PartOfLevelEditor>();
		Vector2 movement;
		List<PartOfLevelEditor> selected = new List<PartOfLevelEditor>();
		Vector2 worldMousePos;
		Vector2 previousWorldMousePos;
		Vector2 selectStartWorldMousePos;
		Image selection;
		public Color selectionColor;
		SelectMethod selectMethod = SelectMethod.Replace;
		public List<GameObject> extraOptionsLists;
		public Canvas worldCanvas;
		Vector2 mapSize;
		public ClampedFloat zoomAmount;
		public float divideFromAlphaOnDeselect;
		List<EditorAction> actions = new List<EditorAction>();
		float worldCanvasScale;
		Vector2 selectionVector;
		string mapName;
		string mapUsername;
		string mapPassword;
		bool publish;
		string mapData;
		EditorActionType currentActionType;
		public GameObject moveOverlapDialog;
		public _Slider rotationSlider;
		Hotkey[] hotkeys;
		List<PartOfLevelEditor> moveErrorOriginalObjects = new List<PartOfLevelEditor>();
		List<PartOfLevelEditor> moveErrorMovedObjects = new List<PartOfLevelEditor>();
		List<Vector2> movedPartsStartPos = new List<Vector2>();
		bool error;
		EditorAction currentAction = new EditorAction();
		bool mouseOnUI;
		bool mousePressStartedOnUI;
		public Button undoButton;
		public Button redoButton;
		bool madeAction;
		bool canAct;
		bool mousePressed;
		string parTime;
		const string successIndicator = "﬩";
		public const string endOfInfoIndicator = "(END)";
		RectTransform selectionTrs;
		[SerializeField]
		RectTransform worldCanvasTrs;
		PartOfLevelEditor[] uniqueParts;
		PartOfLevelEditor[] requiredParts;
		public Toggle publishToggle;
		public Text parText;
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (worldCanvasTrs == null)
					worldCanvasTrs = worldCanvas.GetComponent<RectTransform>();
				return;
			}
#endif
			base.Awake ();
			worldCanvasScale = worldCanvasTrs.lossyScale.x;
			mapSize = worldCanvasTrs.sizeDelta * worldCanvasScale;
			Rect uvRect = worldCanvas.GetComponentInChildren<RawImage>().uvRect;
			uvRect.size = worldCanvas.GetComponent<RectTransform>().sizeDelta;
			worldCanvas.GetComponentInChildren<RawImage>().uvRect = uvRect;
			whatToErase.AddRange(GameManager.Instance.levelEditorPrefabs);
			foreach (GameObject extraOptionsList in extraOptionsLists)
				extraOptionsList.SetActive(false);
			moveOverlapDialog.SetActive(false);
			// hotkeys = FindObjectsOfType<Hotkey>();
			SetUndoAndRedoButtonInteractable ();
			List<PartOfLevelEditor> uniqueParts = new List<PartOfLevelEditor>();
			List<PartOfLevelEditor> requiredParts = new List<PartOfLevelEditor>();
			foreach (PartOfLevelEditor part in GameManager.Instance.levelEditorPrefabs)
			{
				if (part.onlyOneExists)
					uniqueParts.Add(part);
				else if (part.required)
					requiredParts.Add(part);
			}
			this.uniqueParts = uniqueParts.ToArray();
			this.requiredParts = requiredParts.ToArray();
		}
		
		public virtual IEnumerator SaveRoutine ()
		{
			WWWForm form = NetworkManager.defaultDatabaseAccessForm;
			form.AddField("mapName", mapName);
			form.AddField("mapUsername", mapUsername);
			form.AddField("mapPassword", mapPassword);
			mapData = "";
			foreach (PartOfLevelEditor part in PartOfLevelEditor.instances)
				mapData += part.ToString();
			form.AddField("mapData", mapData);
			form.AddField("parTime", parTime);
			CoroutineWithData cd = new CoroutineWithData(this, NetworkManager.Instance.PostFormToResourceRoutine("SaveMap", NetworkManager.defaultDatabaseAccessForm));
			string result = "";
			Exception exception;
			while (string.IsNullOrEmpty(result))
			{
				yield return cd.coroutine;
				exception = cd.result as Exception;
				if (exception != null)
				{
					NetworkManager.Instance.notificationText.text = exception.Message;
					StartCoroutine(NetworkManager.Instance.notificationTextObject.DisplayRoutine ());
					yield break;
				}
				else
					result = cd.result as string;
			}
			if (result.Contains("" + successIndicator))
				Messager.instance.Message(result.Split(new string[] {successIndicator}, StringSplitOptions.None)[1]);
			Debug.Log(result);
		}
		
		public virtual IEnumerator LoadRoutine ()
		{
			WWWForm form = NetworkManager.defaultDatabaseAccessForm;
			form.AddField("mapName", mapName);
			form.AddField("mapUsername", mapUsername);
			form.AddField("mapPassword", mapPassword);
			CoroutineWithData cd = new CoroutineWithData(this, NetworkManager.Instance.PostFormToResourceRoutine("LoadMap", NetworkManager.defaultDatabaseAccessForm));
			string result = "";
			Exception exception;
			while (string.IsNullOrEmpty(result))
			{
				yield return cd.coroutine;
				exception = cd.result as Exception;
				if (exception != null)
				{
					NetworkManager.Instance.notificationText.text = exception.Message;
					StartCoroutine(NetworkManager.Instance.notificationTextObject.DisplayRoutine ());
					yield break;
				}
				else
					result = cd.result as string;
			}
			Debug.Log(result);
			if (!result.Contains("" + successIndicator))
			{
				Messager.instance.Message("The level couldn't be loaded");
				yield break;
			}
			string textBeforeData = "Map data: ";
			string textBeforePar = "Map par: ";
			string textBeforePublished = "Map published: ";
			mapData = result.Substring(result.IndexOf(textBeforeData) + textBeforeData.Length);
			mapData = mapData.Substring(0, result.IndexOf(textBeforePar));
			parTime = result.Substring(result.IndexOf(textBeforePar) + textBeforePar.Length);
			parTime = parTime.Substring(0, result.IndexOf(textBeforePublished));
			parText.text = parTime;
			string publishStr = result.Substring(result.IndexOf(textBeforePublished) + textBeforePublished.Length);
			publishStr = publishStr.Substring(0, result.IndexOf(endOfInfoIndicator));
			publish = (bool) Enum.ToObject(typeof(bool), byte.Parse(publishStr));
			publishToggle.isOn = publish;
			ClearMap ();
			PartOfLevelEditor.CreateObjects(mapData);
			SetUndoAndRedoButtonInteractable ();
			Messager.instance.Message(result.Split(new string[] {successIndicator}, StringSplitOptions.None)[1]);
			Debug.Log(result);
		}
		
		public virtual IEnumerator DeleteRoutine ()
		{
			WWWForm form = NetworkManager.defaultDatabaseAccessForm;
			form.AddField("mapName", mapName);
			form.AddField("mapUsername", mapUsername);
			form.AddField("mapPassword", mapPassword);
			CoroutineWithData cd = new CoroutineWithData(this, NetworkManager.Instance.PostFormToResourceRoutine("DeleteMap", NetworkManager.defaultDatabaseAccessForm));
			string result = "";
			Exception exception;
			while (string.IsNullOrEmpty(result))
			{
				yield return cd.coroutine;
				exception = cd.result as Exception;
				if (exception != null)
				{
					NetworkManager.Instance.notificationText.text = exception.Message;
					StartCoroutine(NetworkManager.Instance.notificationTextObject.DisplayRoutine ());
					yield break;
				}
				else
					result = cd.result as string;
			}
			SetUndoAndRedoButtonInteractable ();
			if (result.Contains("" + successIndicator))
				Messager.instance.Message(result.Split(new string[] {successIndicator}, StringSplitOptions.None)[1]);
			Debug.Log(result);
		}
		
		public virtual IEnumerator PublishRoutine ()
		{
			WWWForm form = NetworkManager.defaultDatabaseAccessForm;
			form.AddField("mapName", mapName);
			form.AddField("mapUsername", mapUsername);
			form.AddField("mapPassword", mapPassword);
			form.AddField("publish", publish.GetHashCode());
			CoroutineWithData cd = new CoroutineWithData(this, NetworkManager.Instance.PostFormToResourceRoutine("PublishMap", NetworkManager.defaultDatabaseAccessForm));
			string result = "";
			Exception exception;
			while (string.IsNullOrEmpty(result))
			{
				yield return cd.coroutine;
				exception = cd.result as Exception;
				if (exception != null)
				{
					NetworkManager.Instance.notificationText.text = exception.Message;
					StartCoroutine(NetworkManager.Instance.notificationTextObject.DisplayRoutine ());
					yield break;
				}
				else
					result = cd.result as string;
			}
			SetUndoAndRedoButtonInteractable ();
			if (result.Contains("" + successIndicator))
				Messager.instance.Message(result.Split(new string[] {successIndicator}, StringSplitOptions.None)[1]);
			Debug.Log(result);
		}
		
		public virtual void Update ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			bool mouseDown = Input.GetMouseButtonDown(0);
			bool mouseUp = Input.GetMouseButtonUp(0);
			if (currentActionType == EditorActionType.Pan || currentActionType == EditorActionType.Zoom)
				worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			else
				worldMousePos = EditorCamera.Instance.cam.ScreenToWorldPoint(Input.mousePosition);
			Physics2D.queriesHitTriggers = true;
			if (mouseDown)
				OnPointerDown ();
			if (mouseUp)
				OnPointerUp ();
			if ((mousePressed && mousePressStartedOnUI != mouseOnUI) || (mousePressed && mouseOnUI))
				canAct = false;
			else
				canAct = true;
			if (mouseDown)
				ActStart ();
			if (mousePressed)
				ActContinue ();
			if (mouseUp)
				ActEnd ();
			Physics2D.queriesHitTriggers = false;
			previousWorldMousePos = worldMousePos;
			rotationSlider.snapValues = new float[0];
			foreach (PartOfLevelEditor part in selected)
			{
				foreach (int rota in part.allowedRotations)
				{
					if (!rotationSlider.snapValues.Contains(rota))
						rotationSlider.snapValues = rotationSlider.snapValues.Add(rota);
				}
			}
		}
		
		public virtual void ActStart ()
		{
			madeAction = false;
			if (!canAct)
				return;
			currentAction = new EditorAction();
			whatToMove.Clear();
			switch (currentActionType)
			{
				case EditorActionType.Move:
					MoveStart ();
					break;
				case EditorActionType.Select:
					SelectStart ();
					break;
			}
		}
		
		public virtual void ActContinue ()
		{
			if (!canAct)
				return;
			switch (currentActionType)
			{
				case EditorActionType.Build:
					BuildContinue ();
					break;
				case EditorActionType.Erase:
					EraseContinue ();
					break;
				case EditorActionType.Move:
					MoveContinue ();
					break;
				case EditorActionType.Select:
					SelectContinue ();
					break;
				case EditorActionType.Pan:
					PanContinue ();
					break;
				case EditorActionType.Zoom:
					ZoomContinue ();
					break;
			}
		}
		
		public virtual void ActEnd ()
		{
			switch (currentActionType)
			{
				case EditorActionType.Move:
					MoveEnd ();
					break;
				case EditorActionType.Select:
					SelectEnd ();
					break;
				case EditorActionType.Pan:
					PanEnd ();
					break;
				case EditorActionType.Zoom:
					ZoomEnd ();
					break;
			}
			if (error || !madeAction)
				return;
			EndAction ();
		}
		
		public virtual void OnPointerDown ()
		{
			mousePressed = true;
			mousePressStartedOnUI = mouseOnUI;
		}
		
		public virtual void OnPointerUp ()
		{
			mousePressed = false;
		}
		
		public virtual void OnPointerEnterUI ()
		{
			mouseOnUI = true;
		}
		
		public virtual void OnPointerExitUI ()
		{
			mouseOnUI = false;
		}
		
		public virtual void EndAction ()
		{
			int actionCount = actions.Count;
			for (int i = currentActionIndex; i < actionCount; i ++)
				actions.RemoveAt(actions.Count - 1);
			actions.Add(currentAction);
			currentActionIndex ++;
			currentAction = new EditorAction();
			madeAction = false;
			SetUndoAndRedoButtonInteractable ();
		}
		
		public virtual void ZoomContinue ()
		{
			Vector2 mouseMovement = worldMousePos - previousWorldMousePos;
			zoomAmount.SetValue(zoomAmount.GetValue() + mouseMovement.y);
			EditorCamera.Instance.GetComponent<Camera>().orthographicSize = zoomAmount.GetValue();
		}
		
		public virtual void ZoomEnd ()
		{
			Camera.main.orthographicSize = EditorCamera.Instance.cam.orthographicSize;
		}
		
		public virtual void BuildContinue ()
		{
			if (whatToBuild == null)
				return;
			bool overlap = false;
			foreach (Vector2 buildPos in GetBrushSubPositions())
			{
				if (VectorExtensions.IsInsideBounds(buildPos, VectorExtensions.SetZ(mapSize / 2, 1), true))
				{
					overlap = false;
					Physics2D.queriesHitTriggers = true;
					foreach (Collider2D hit in Physics2D.OverlapPointAll(buildPos, LayerMask.GetMask(LayerMask.LayerToName(whatToBuild.gameObject.layer))))
					{
						if (hit.name == whatToBuild.name)
						{
							overlap = true;
							break;
						}
					}
					if (!overlap)
						BuildAt (buildPos, whatToBuild);
				}
			}
		}
		
		public virtual Vector2 GetBrushPosition ()
		{
			if (brushSize % 2 == 1)
				return VectorSnappedToGridSquares(worldMousePos);
			else
				return VectorSnappedToGridIntersections(worldMousePos);
		}
		
		public virtual Vector2[] GetBrushSubPositions ()
		{
			if (brushSize == 1)
				return new Vector2[] { GetBrushPosition() };
			List<Vector2> output = new List<Vector2>();
			Vector2 brushPos = GetBrushPosition();
			for (int x = -brushSize / 2; x <= brushSize / 2; x ++)
			{
				for (int y = -brushSize / 2; y <= brushSize / 2; y ++)
				{
					Vector2 relativePos = new Vector2(x, y) * worldCanvasScale;
					if (brushSize % 2 == 0)
						relativePos = Vector2.ClampMagnitude(relativePos, relativePos.magnitude - .5f);
					output.Add(VectorSnappedToGridSquares(brushPos + relativePos));
				}
			}
			return output.ToArray();
		}
		
		public virtual void EraseContinue ()
		{
			foreach (Vector2 erasePos in GetBrushSubPositions())
				EraseAt (erasePos, whatToErase);
		}
		
		public virtual void MoveStart ()
		{
			whatToMove.Clear();
			whatToMove.AddRange(selected);
			movedPartsStartPos.Clear();
			foreach (PartOfLevelEditor part in whatToMove)
			{
				movedPartsStartPos.Add(part.trs.position);
				currentAction.destroyedData += part.ToString();
			}
		}
		
		public virtual void MoveContinue ()
		{
			foreach (PartOfLevelEditor movePart in whatToMove)
			{
				Vector2 move = (Vector3) (VectorSnappedToGridSquares(worldMousePos) - VectorSnappedToGridSquares(previousWorldMousePos));
				movement += move;
				movePart.GetComponent<Transform>().position += (Vector3) move;
			}
		}
		
		public virtual void MoveEnd ()
		{
			bool overlap = false;
			moveErrorOriginalObjects.Clear();
			moveErrorMovedObjects.Clear();
			foreach (PartOfLevelEditor part in whatToMove)
			{
				Collider2D[] _hits = Physics2D.OverlapPointAll(part.trs.position, collisionMatrix.GetLayerCollisionMask(part.gameObject.layer));
				List<Collider2D> hits = new List<Collider2D>();
				hits.AddRange(_hits);
				if (hits.Contains(part.GetComponent<Collider2D>()))
					hits.Remove(part.GetComponent<Collider2D>());
				if (hits.Count > 0)
				{
					overlap = true;
					foreach (Collider2D hit in hits)
						moveErrorOriginalObjects.Add(hit.GetComponent<PartOfLevelEditor>());
					moveErrorMovedObjects.Add(part);
				}
			}
			if (overlap)
			{
				moveOverlapDialog.SetActive(true);
				SetError (true);
			}
			else
			{
				foreach (PartOfLevelEditor part in whatToMove)
					currentAction.whereWereMyActions.Add(part.trs.position);
				madeAction = true;
			}
		}
		
		public virtual void CancelMovement ()
		{
			for (int i = 0; i < whatToMove.Count; i ++)
				whatToMove[i].trs.position = (Vector3) movedPartsStartPos[i];
			OnMoveOverlapResolved ();
		}
		
		public virtual void KeepOriginalObjects ()
		{
			for (int i = 0; i < moveErrorMovedObjects.Count; i ++)
				Destroy(moveErrorMovedObjects[i].gameObject);
			madeAction = true;
			OnMoveOverlapResolved ();
		}
		
		public virtual void KeepMovedObjects ()
		{
			for (int i = 0; i < moveErrorOriginalObjects.Count; i ++)
				Destroy(moveErrorOriginalObjects[i].gameObject);
			madeAction = true;
			OnMoveOverlapResolved ();
		}
		
		public virtual void OnMoveOverlapResolved ()
		{
			error = false;
			EndAction ();
			moveOverlapDialog.SetActive(false);
		}
		
		public virtual void ClearMap ()
		{
			foreach (PartOfLevelEditor partOfLevelEditor in PartOfLevelEditor.instances)
				Destroy(partOfLevelEditor.gameObject);
			actions.Clear();
		}
		
		public virtual void SetAction (string actionName)
		{
			currentActionType = (EditorActionType) Enum.Parse(typeof(EditorActionType), actionName);
		}
		
		public virtual void SetAction (int actionIndex)
		{
			currentActionType = (EditorActionType) Enum.ToObject(typeof(EditorActionType), actionIndex);
		}
		
		public virtual void SetSelectMethod (string selectMethodName)
		{
			selectMethod = (SelectMethod) Enum.Parse(typeof(SelectMethod), selectMethodName);
		}
		
		public virtual void SetSelectMethod (int selectMethodIndex)
		{
			selectMethod = (SelectMethod) Enum.ToObject(typeof(SelectMethod), selectMethodIndex);
		}
		
		public virtual void SelectStart ()
		{
			selectStartWorldMousePos = worldMousePos;
			selection = new GameObject().AddComponent<Image>();
			selectionTrs = selection.GetComponent<RectTransform>();
			selectionTrs.pivot = Vector2.zero;
			selection.color = selectionColor;
			selectionTrs.position = VectorSnappedToGridIntersections(selectStartWorldMousePos);
			selectionTrs.sizeDelta = VectorExtensions.Divide(Vector2.one, worldCanvas.transform.lossyScale);
			selectionTrs.SetParent(worldCanvas.transform);
		}
		
		public virtual void SelectContinue ()
		{
			if (selection != null)
			{
				selectionVector = worldMousePos - selectStartWorldMousePos;
				selection.transform.localScale = VectorSnappedToGridIntersections(worldMousePos) - VectorSnappedToGridIntersections(selectStartWorldMousePos);
				if (Mathf.Abs(selection.transform.localScale.x) < worldCanvasScale)
					selection.transform.localScale = new Vector3(worldCanvasScale * Mathf.Sign(selectionVector.x), selection.transform.localScale.y);
				if (Mathf.Abs(selection.transform.localScale.y) < worldCanvasScale)
					selection.transform.localScale = new Vector3(selection.transform.localScale.x, worldCanvasScale * Mathf.Sign(selectionVector.y));
			}
		}
		
		public virtual void SelectEnd ()
		{
			Vector3[] selectionCorners = new Vector3[4];
			selection.GetComponent<RectTransform>().GetWorldCorners(selectionCorners);
			Vector2 selectStart = (Vector2) selectionCorners[3];
			Vector2 selectEnd = (Vector2) selectionCorners[1];
			selectStart = Vector2.MoveTowards(selectStart, selectEnd, 1);
			selectEnd = Vector2.MoveTowards(selectEnd, selectStart, 1);
			List<PartOfLevelEditor> previousSelected = new List<PartOfLevelEditor>();
			previousSelected.AddRange(selected);
			switch (selectMethod)
			{
				case SelectMethod.Replace:
					while (selected.Count > 0)
						ForceDeselectObject (selected[0]);
					foreach (Collider2D hit in Physics2D.OverlapAreaAll(selectStart, selectEnd))
						TrySelectObject (hit.GetComponent<PartOfLevelEditor>());
					break;
				case SelectMethod.Add:
					foreach (Collider2D hit in Physics2D.OverlapAreaAll(selectStart, selectEnd))
						TrySelectObject (hit.GetComponent<PartOfLevelEditor>());
					break;
				case SelectMethod.Subtract:
					foreach (Collider2D hit in Physics2D.OverlapAreaAll(selectStart, selectEnd))
						TrySelectObject (hit.GetComponent<PartOfLevelEditor>());
					break;
				case SelectMethod.Toggle:
					foreach (Collider2D hit in Physics2D.OverlapAreaAll(selectStart, selectEnd))
					{
						if (selected.Contains(hit.GetComponent<PartOfLevelEditor>()))
							TryDeselectObject (hit.GetComponent<PartOfLevelEditor>());
						else
							TrySelectObject (hit.GetComponent<PartOfLevelEditor>());
					}
					break;
			}
			if (selection != null)
				Destroy(selection.gameObject);
			List<PartOfLevelEditor> previousAndCurrentSelected = new List<PartOfLevelEditor>();
			previousAndCurrentSelected.AddRange(previousSelected);
			previousAndCurrentSelected.AddRange(selected);
			foreach (PartOfLevelEditor part in previousAndCurrentSelected)
			{
				if (previousSelected.Contains(part) != selected.Contains(part))
				{
					currentAction.whereWereMyActions.Add(part.trs.position);
					madeAction = true;
				}
			}
			foreach (PartOfLevelEditor part in whatToSelect)
				currentAction.whatToActUpon.Add(GetPartPrefab(part));
		}
		
		public virtual void PanContinue ()
		{
			Vector3 newCameraPos = EditorCamera.Instance.transform.position;
			newCameraPos -= (Vector3) (VectorSnappedToGridSquares(worldMousePos) - VectorSnappedToGridSquares(previousWorldMousePos));
			newCameraPos = newCameraPos.ClampComponents(-mapSize / 2, mapSize / 2);
			newCameraPos.z = EditorCamera.Instance.transform.position.z;
			EditorCamera.Instance.transform.position = newCameraPos;
		}
		
		public virtual void PanEnd ()
		{
			Camera.main.transform.position = EditorCamera.Instance.transform.position;
		}
		
		public virtual Vector2 VectorSnappedToGridSquares (Vector2 v)
		{
			return v.Snap((Vector2) worldCanvas.transform.lossyScale);
		}
		
		public virtual Vector2 VectorSnappedToGridIntersections (Vector2 v)
		{
			Vector2 halfGridSquareSize = worldCanvas.transform.lossyScale / 2;
			Vector2 snappedToGridSquares = VectorSnappedToGridSquares(v);
			Vector2[] squareCorners = new Vector2[4];
			squareCorners[0] = snappedToGridSquares + halfGridSquareSize;
			squareCorners[1] = snappedToGridSquares - halfGridSquareSize;
			squareCorners[2] = snappedToGridSquares + new Vector2(-halfGridSquareSize.x, halfGridSquareSize.y);
			squareCorners[3] = snappedToGridSquares + new Vector2(halfGridSquareSize.x, -halfGridSquareSize.y);
			Vector2 smallestVector = squareCorners[0] - snappedToGridSquares;
			for (int i = 1; i < squareCorners.Length; i ++)
			{
				Vector2 toSquareCorner = squareCorners[i] - snappedToGridSquares;
				if (toSquareCorner.magnitude < smallestVector.magnitude)
					smallestVector = toSquareCorner;
			}
			return smallestVector + snappedToGridSquares;
		}
		
		public virtual void SetWhatToBuild (PartOfLevelEditor newGo)
		{
			whatToBuild = newGo;
		}
		
		public virtual void ToggleExtraOptions (GameObject extraOptions)
		{
			bool isActive = extraOptions.activeSelf;
			foreach (GameObject extraOptionsList in extraOptionsLists)
			{
				if (extraOptionsList.name == extraOptions.name)
					extraOptionsList.SetActive(false);
			}
			extraOptions.SetActive(!isActive);
		}
		
		public virtual void SelectObject (PartOfLevelEditor selectPart, bool forceImperative)
		{
			if (!forceImperative)
			{
				bool partFound = false;
				foreach (PartOfLevelEditor part in whatToSelect)
				{
					if (part.name == selectPart.name)
					{
						partFound = true;
						break;
					}
				}
				if (!partFound)
					return;
			}
			if (!selected.Contains(selectPart))
				selected.Add(selectPart);
			SpriteRenderer renderer = selectPart.GetComponent<SpriteRenderer>();
			if (renderer != null)
				renderer.color = ColorExtensions.MultiplyAlpha(renderer.color, divideFromAlphaOnDeselect);
		}
		
		public virtual void TrySelectObject (PartOfLevelEditor selectPart)
		{
			SelectObject(selectPart, false);
		}
		
		public virtual void ForceSelectObject (PartOfLevelEditor selectPart)
		{
			SelectObject(selectPart, true);
		}
		
		public virtual void TryDeselectObject (PartOfLevelEditor selectPart)
		{
			DeselectObject(selectPart, false);
		}
		
		public virtual void ForceDeselectObject (PartOfLevelEditor selectPart)
		{
			DeselectObject(selectPart, true);
		}
		
		public virtual void DeselectObject (PartOfLevelEditor selectPart, bool forceImperative)
		{
			if (!forceImperative)
			{
				bool partFound = false;
				foreach (PartOfLevelEditor part in whatToSelect)
				{
					if (part.name == selectPart.name)
					{
						partFound = true;
						break;
					}
				}
				if (!partFound)
					return;
			}
			if (selected.Contains(selectPart))
				selected.Remove(selectPart);
			SpriteRenderer renderer = selectPart.GetComponent<SpriteRenderer>();
			if (renderer != null)
				renderer.color = ColorExtensions.DivideAlpha(renderer.color, divideFromAlphaOnDeselect);
		}
		
		public virtual void BuildAt (Vector2 loc, PartOfLevelEditor part)
		{
			foreach (PartOfLevelEditor uniquePart in uniqueParts)
			{
				if (GameObject.Find(uniquePart.name) != null)
				{
					Messager.instance.Message("At most one of that object can exist");
					return;
				}
			}
			Collider2D[] overlaps = Physics2D.OverlapPointAll(loc, collisionMatrix.GetLayerCollisionMask(whatToBuild.gameObject.layer));
			if (overlaps.Length > 0)
			{
				switch (buildMethod)
				{
					case BuildMethod.Replace:
						foreach (Collider2D overlap in overlaps)
						{
							PartOfLevelEditor overlapPart = overlap.GetComponent<PartOfLevelEditor>();
							currentAction.destroyedData += overlapPart.ToString();
							currentAction.whatToActUpon.Add(GetPartPrefab(overlapPart));
							Destroy(overlap.gameObject);
						}
						break;
					case BuildMethod.Add:
						break;
					case BuildMethod.Cancel:
						return;
				}
			}
			PartOfLevelEditor build = (PartOfLevelEditor) Instantiate(part, loc, part.trs.rotation);
			build.name = build.name.Replace("(Clone)", "");
			Destroy(build.GetComponent<SnapPosition>());
			ForceDeselectObject (build);
			currentAction.whereWereMyActions.Add(loc);
			currentAction.whatToBuild = part;
			madeAction = true;
		}
		
		public virtual string EraseAt (Vector2 loc, PartOfLevelEditor[] eraseParts)
		{
			string output = "";
			currentAction.whatToActUpon.AddRange(eraseParts);
			foreach (PartOfLevelEditor erasePart in eraseParts)
			{
				foreach (Collider2D hit in Physics2D.OverlapPointAll(loc, LayerMask.GetMask(LayerMask.LayerToName(erasePart.gameObject.layer))))
				{
					if (hit.name == erasePart.name)
					{
						PartOfLevelEditor hitPart = hit.GetComponent<PartOfLevelEditor>();
						currentAction.destroyedData += hitPart.ToString();
						output += hitPart.ToString();
						currentAction.whereWereMyActions.Add(loc);
						if (selected.Contains(hitPart))
							selected.Remove(hitPart);
						madeAction = true;
						DestroyImmediate(hit.gameObject);
					}
				}
			}
			return output;
		}
		
		public virtual string EraseAt (Vector2 loc, List<PartOfLevelEditor> eraseParts)
		{
			string output = "";
			currentAction.whatToActUpon.AddRange(eraseParts);
			foreach (PartOfLevelEditor erasePart in eraseParts)
			{
				foreach (Collider2D hit in Physics2D.OverlapPointAll(loc, LayerMask.GetMask(LayerMask.LayerToName(erasePart.gameObject.layer))))
				{
					if (hit.name == erasePart.name)
					{
						PartOfLevelEditor hitPart = hit.GetComponent<PartOfLevelEditor>();
						currentAction.destroyedData += hitPart.ToString();
						output += hitPart.ToString();
						currentAction.whereWereMyActions.Add(loc);
						if (selected.Contains(hitPart))
							selected.Remove(hitPart);
						madeAction = true;
						DestroyImmediate(hit.gameObject);
					}
				}
			}
			return output;
		}
		
		public virtual void Undo ()
		{
			currentAction = new EditorAction();
			whatToMove.Clear();
			if (currentActionIndex > 0)
			{
				currentActionIndex --;
				actions[currentActionIndex].Undo ();
			}
			SetUndoAndRedoButtonInteractable ();
		}
		
		public virtual void SetUndoAndRedoButtonInteractable ()
		{
			undoButton.interactable = currentActionIndex > 0;
			redoButton.interactable = currentActionIndex < actions.Count;
		}
		
		public virtual void Redo ()
		{
			currentAction = new EditorAction();
			whatToMove.Clear();
			if (currentActionIndex < actions.Count)
			{
				actions[currentActionIndex].Redo ();
				currentActionIndex ++;
			}
			SetUndoAndRedoButtonInteractable ();
		}
		
		public virtual void SetHotkeysEnabled (bool enabled)
		{
			foreach (Hotkey hotkey in hotkeys)
				hotkey.enabled = enabled;
		}
		
		public virtual void SetBuildMethod (string buildMethodName)
		{
			buildMethod = (BuildMethod) Enum.Parse(typeof(BuildMethod), buildMethodName);
		}
		
		public virtual void SetBuildMethod (int buildMethodIndex)
		{
			buildMethod = (BuildMethod) Enum.ToObject(typeof(BuildMethod), buildMethodIndex);
		}
		
		public virtual void SetBrushSize (float brushSize)
		{
			brushSize = (int) brushSize;
		}
		
		public virtual void SetCurrentPart (PartOfLevelEditor part)
		{
			currentPart = part;
		}
		
		public virtual void SetWhatToErase (bool erase)
		{
			if (erase && !whatToErase.Contains(currentPart))
				whatToErase.Add(currentPart);
			else if (!erase && whatToErase.Contains(currentPart))
				whatToErase.Remove(currentPart);
		}
		
		public virtual void SetWhatToSelect (bool select)
		{
			if (select && !whatToSelect.Contains(currentPart))
				whatToSelect.Add(currentPart);
			else if (!select && whatToSelect.Contains(currentPart))
				whatToSelect.Remove(currentPart);
		}
		
		public virtual void RotateSelectionObjects (float newRota)
		{
			foreach (PartOfLevelEditor part in selected)
			{
				int closestRota = part.allowedRotations[0];
				for (int i = 1; i < part.allowedRotations.Length; i ++)
				{
					if (Mathf.DeltaAngle(closestRota, newRota) < Mathf.DeltaAngle(part.allowedRotations[i], newRota))
						closestRota = part.allowedRotations[i];
				}
				part.trs.eulerAngles = Vector3.forward * closestRota;
			}
		}
		
		public virtual void EndChangeProperties ()
		{
			foreach (PartOfLevelEditor part in selected)
			{
				currentAction.whatToActUpon.Add(GetPartPrefab(part));
				currentAction.whereWereMyActions.Add(part.trs.position);
			}
			EndAction ();
		}
		
		public virtual void StartChangeProperties ()
		{
			foreach (PartOfLevelEditor part in selected)
				currentAction.destroyedData += part.ToString();
		}
		
		public virtual void ScaleSelectionObjectsX (float newSizeX)
		{
			foreach (PartOfLevelEditor part in selected)
			{
				if (part.resizable)
				{
					int previousSizeX = (int) part.trs.localScale.x;
					part.trs.localScale = new Vector3(newSizeX * worldCanvasScale, part.trs.localScale.y);
					bool isOnIntersection = previousSizeX / 25 % 2 == 0;
					bool shouldBeOnIntersection = newSizeX % 2 == 0;
					if (isOnIntersection != shouldBeOnIntersection)
					{
						if (shouldBeOnIntersection)
							part.trs.position -= Vector3.right * worldCanvasScale / 2 * Mathf.Sign(newSizeX - previousSizeX);
						else
							part.trs.position += Vector3.right * worldCanvasScale / 2 * Mathf.Sign(newSizeX - previousSizeX);
					}
					madeAction = true;
				}
			}
		}
		
		public virtual void ScaleSelectionObjectsY (float newSizeY)
		{
			foreach (PartOfLevelEditor part in selected)
			{
				if (part.resizable)
				{
					int previousSizeY = (int) part.trs.localScale.y;
					part.trs.localScale = new Vector3(part.trs.localScale.x, newSizeY * worldCanvasScale);
					bool isOnIntersection = previousSizeY / 25 % 2 == 0;
					bool shouldBeOnIntersection = newSizeY % 2 == 0;
					if (isOnIntersection != shouldBeOnIntersection)
					{
						if (shouldBeOnIntersection)
							part.trs.position -= Vector3.up * worldCanvasScale / 2 * Mathf.Sign(newSizeY - previousSizeY);
						else
							part.trs.position += Vector3.up * worldCanvasScale / 2 * Mathf.Sign(newSizeY - previousSizeY);
					}
					madeAction = true;
				}
			}
		}
		
		public virtual void SetMapName (string mapName)
		{
			this.mapName = mapName;
		}
		
		public virtual void SetMapUsername (string mapUsername)
		{
			this.mapUsername = mapUsername;
		}
		
		public virtual void SetMapPassword (string mapPassword)
		{
			this.mapPassword = mapPassword;
		}
		
		public virtual void SetParTime (string parTime)
		{
			this.parTime = parTime;
		}
		
		public virtual void SetPublish (bool publish)
		{
			foreach (PartOfLevelEditor requiredPart in requiredParts)
			{
				if (GameObject.Find(requiredPart.name) == null)
				{
					Messager.instance.Message("Levels must have a snake and an end");
					return;
				}
			}
			this.publish = publish;
			StartCoroutine(PublishRoutine ());
		}
		
		public virtual void Save ()
		{
			StartCoroutine(SaveRoutine ());
		}
		
		public virtual void Load ()
		{
			StartCoroutine(LoadRoutine ());
		}
		
		public virtual void Delete ()
		{
			StartCoroutine(DeleteRoutine ());
		}
		
		public virtual void SetError (bool error)
		{
			this.error = error;
		}
		
		public static PartOfLevelEditor GetPartPrefab (PartOfLevelEditor partInstance)
		{
			foreach (PartOfLevelEditor partPrefab in GameManager.Instance.levelEditorPrefabs)
			{
				if (partPrefab.name == partInstance.name)
					return partPrefab;
			}
			return null;
		}
		
		public enum EditorActionType
		{
			Build = 0,
			Erase = 1,
			Move = 2,
			Select = 3,
			Pan = 4,
			Zoom = 5,
			Properties = 6
		}
		
		public enum SelectMethod
		{
			Replace = 0,
			Add = 1,
			Subtract = 2,
			Toggle = 3
		}
		
		public enum BuildMethod
		{
			Cancel = 0,
			Replace = 1,
			Add = 2
		}
		
		[Serializable]
		public class EditorAction
		{
			public EditorActionType type;
			public PartOfLevelEditor whatToBuild;
			public List<PartOfLevelEditor> whatToActUpon = new List<PartOfLevelEditor>();
			public string destroyedData;
			public List<Vector2> whereWereMyActions = new List<Vector2>();
			
			public EditorAction ()
			{
				if (LevelEditor.Instance != null)
				{
					if (LevelEditor.Instance.whatToBuild != null)
						whatToBuild = LevelEditor.Instance.whatToBuild;
					type = LevelEditor.Instance.currentActionType;
				}
			}
			
			public virtual void Undo ()
			{
				LevelEditor.Instance.enabled = false;
				switch (type)
				{
					case EditorActionType.Build:
						List<PartOfLevelEditor> whatToErase = new List<PartOfLevelEditor>();
						whatToErase.AddRange(whatToActUpon);
						whatToErase.Add(whatToBuild);
						foreach (Vector2 loc in whereWereMyActions)
							LevelEditor.Instance.EraseAt(loc, whatToErase.ToArray());
						PartOfLevelEditor.CreateObjects(destroyedData);
						break;
					case EditorActionType.Erase:
						PartOfLevelEditor.CreateObjects(destroyedData);
						break;
					case EditorActionType.Select:
						LayerMask selectMask = LayerMask.GetMask();
						foreach (PartOfLevelEditor part in whatToActUpon)
							selectMask = LayerMaskExtensions.AddToMask(selectMask, part.gameObject.layer);
						foreach (Vector2 loc in whereWereMyActions)
						{
							foreach (Collider2D hit in Physics2D.OverlapPointAll(loc, selectMask))
							{
								PartOfLevelEditor hitPart = hit.GetComponent<PartOfLevelEditor>();
								if (LevelEditor.Instance.selected.Contains(hitPart))
									LevelEditor.Instance.ForceDeselectObject(hitPart);
								else
									LevelEditor.Instance.ForceSelectObject(hitPart);
							}
						}
						break;
					case EditorActionType.Move:
						foreach (Vector2 loc in whereWereMyActions)
							LevelEditor.Instance.EraseAt(loc, GameManager.Instance.levelEditorPrefabs);
						PartOfLevelEditor[] createdParts = PartOfLevelEditor.CreateObjects(LevelEditor.Instance.actions[LevelEditor.Instance.currentActionIndex].destroyedData);
						whereWereMyActions.Clear();
						whatToActUpon.Clear();
						foreach (PartOfLevelEditor createdPart in createdParts)
						{
							LevelEditor.Instance.ForceSelectObject(createdPart);
							whereWereMyActions.Add(createdPart.trs.position);
							whatToActUpon.Add(LevelEditor.GetPartPrefab(createdPart));
						}
						break;
					case EditorActionType.Properties:
						string eraseData = "";
						foreach (Vector2 loc in whereWereMyActions)
							eraseData += LevelEditor.Instance.EraseAt(loc, GameManager.Instance.levelEditorPrefabs);
						createdParts = PartOfLevelEditor.CreateObjects(LevelEditor.Instance.actions[LevelEditor.Instance.currentActionIndex].destroyedData);
						whereWereMyActions.Clear();
						whatToActUpon.Clear();
						foreach (PartOfLevelEditor createdPart in createdParts)
						{
							LevelEditor.Instance.ForceSelectObject(createdPart);
							whereWereMyActions.Add(createdPart.trs.position);
							whatToActUpon.Add(LevelEditor.GetPartPrefab(createdPart));
						}
						destroyedData = eraseData;
						break;
				}
				LevelEditor.Instance.enabled = true;
			}
			
			public virtual void Redo ()
			{
				LevelEditor.Instance.enabled = false;
				switch (type)
				{
					case EditorActionType.Build:
						foreach (Vector2 loc in whereWereMyActions)
							LevelEditor.Instance.BuildAt(loc, whatToBuild);
						break;
					case EditorActionType.Erase:
						foreach (Vector2 loc in whereWereMyActions)
							LevelEditor.Instance.EraseAt(loc, whatToActUpon);
						break;
					case EditorActionType.Select:
						LayerMask selectMask = LayerMask.GetMask();
						foreach (PartOfLevelEditor part in whatToActUpon)
							selectMask = LayerMaskExtensions.AddToMask(selectMask, part.gameObject.layer);
						foreach (Vector2 loc in whereWereMyActions)
						{
							foreach (Collider2D hit in Physics2D.OverlapPointAll(loc, selectMask))
							{
								PartOfLevelEditor hitPart = hit.GetComponent<PartOfLevelEditor>();
								if (LevelEditor.Instance.selected.Contains(hitPart))
									LevelEditor.Instance.ForceDeselectObject(hitPart);
								else
									LevelEditor.Instance.ForceSelectObject(hitPart);
							}
						}
						break;
					case EditorActionType.Move:
						foreach (Vector2 loc in whereWereMyActions)
							LevelEditor.Instance.EraseAt(loc, GameManager.Instance.levelEditorPrefabs);
						PartOfLevelEditor[] createdParts = PartOfLevelEditor.CreateObjects(LevelEditor.Instance.actions[LevelEditor.Instance.currentActionIndex].destroyedData);
						whereWereMyActions.Clear();
						whatToActUpon.Clear();
						foreach (PartOfLevelEditor createdPart in createdParts)
						{
							LevelEditor.Instance.ForceSelectObject(createdPart);
							whereWereMyActions.Add(createdPart.trs.position);
							whatToActUpon.Add(LevelEditor.GetPartPrefab(createdPart));
						}
						break;
					case EditorActionType.Properties:
						string eraseData = "";
						foreach (Vector2 loc in whereWereMyActions)
							eraseData += LevelEditor.Instance.EraseAt(loc, GameManager.Instance.levelEditorPrefabs);
						createdParts = PartOfLevelEditor.CreateObjects(LevelEditor.Instance.actions[LevelEditor.Instance.currentActionIndex].destroyedData);
						whereWereMyActions.Clear();
						whatToActUpon.Clear();
						foreach (PartOfLevelEditor createdPart in createdParts)
						{
							LevelEditor.Instance.ForceSelectObject(createdPart);
							whereWereMyActions.Add(createdPart.trs.position);
							whatToActUpon.Add(LevelEditor.GetPartPrefab(createdPart));
						}
						destroyedData = eraseData;
						break;
				}
				LevelEditor.Instance.enabled = true;
			}
		}
	}
}