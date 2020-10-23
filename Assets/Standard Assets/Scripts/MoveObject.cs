using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Extensions;

namespace AmbitiousSnake
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class MoveObject : NotPartOfLevelEditor
	{
		public MoveType moveType;
		public Transform wayPointsParent;
		public List<Waypoint> wayPoints = new List<Waypoint>();
		public bool autoSetWaypoints = true;
		public float moveSpeed;
		public float rotateSpeed;
		public int currentWaypoint;
		public bool backTracking;
		LineRenderer line;
		public WaypointPath path;
		public Transform rotationViewerPrefab;
		List<Transform> rotationViewers;
		[HideInInspector]
		public bool hasTraveledFullCycle;
		ComplexTimer waitTimer;
		
		public override void Awake ()
		{
			base.Awake ();
			Start ();
			waitTimer = gameObject.AddComponent<ComplexTimer>();
			waitTimer.title = "Wait Time";
			waitTimer.changeAmountMultiplier = 1;
			waitTimer.value.max = 1;
		}
		
		void Start ()
		{
			foreach (SnapPosition snapPosition in GetComponentsInChildren<SnapPosition>())
				snapPosition.enabled = false;
			if (autoSetWaypoints)
				wayPointsParent = transform;
			if (wayPointsParent != null)
				wayPoints.AddRange(wayPointsParent.GetComponentsInChildren<Waypoint>());
			foreach (Waypoint waypoint in wayPoints)
				waypoint.transform.SetParent(null);
			if (moveSpeed != 0)
			{
				if (GetComponent<LineRenderer>() == null)
					line = gameObject.AddComponent<LineRenderer>();
				else
					line = GetComponent<LineRenderer>();
				line.positionCount = wayPoints.Count + 1;
				line.SetPosition(0, transform.position);
				if (moveType == MoveType.Loop)
				{
					int counter = 1;
					int waypointIndex = currentWaypoint;
					while (true)
					{
						line.SetPosition(counter, wayPoints[waypointIndex].transform.position);
						if (backTracking)
						{
							waypointIndex --;
							if (waypointIndex == -1)
								waypointIndex = wayPoints.Count - 1;
						}
						else
						{
							waypointIndex ++;
							if (waypointIndex == wayPoints.Count)
								waypointIndex = 0;
						}
						if (waypointIndex == currentWaypoint)
							break;
						counter ++;
					}
				}
				else
				{
					for (int i = 0; i < wayPoints.Count; i ++)
						line.SetPosition(i + 1, wayPoints[i].transform.position);
				}
				line.material = path.material;
				line.startColor = path.color;
				line.endColor = path.color;
				line.startWidth = path.width;
				line.endWidth = path.width;
				line.sortingLayerName = path.sortingLayerName;
				line.sortingOrder = Mathf.Clamp(path.sortingOrder, -32768, 32767);
			}
			if (rotateSpeed != 0)
			{
				for (int i = 0; i < wayPoints.Count; i ++)
				{
					Transform waypoint = wayPoints[i].transform;
					Transform nextWaypoint = null;
					if ((i == wayPoints.Count - 1 && !backTracking) || (i == 0 && backTracking))
					{
						switch (moveType)
						{
							case MoveType.Once:
								return;
							case MoveType.Loop:
								if (!backTracking)
									nextWaypoint = wayPoints[0].transform;
								else
									nextWaypoint = wayPoints[wayPoints.Count - 1].transform;
								break;
							case MoveType.PingPong:
								if (!backTracking)
									nextWaypoint = wayPoints[i - 1].transform;
								else
									nextWaypoint = wayPoints[i + 1].transform;
								break;
						}
					}
					else
					{
						if (!backTracking)
							nextWaypoint = wayPoints[i + 1].transform;
						else
							nextWaypoint = wayPoints[i - 1].transform;
					}
					if (waypoint.eulerAngles.z != nextWaypoint.eulerAngles.z)
					{
						Transform rotationViewer = (Transform) Instantiate(rotationViewerPrefab);
						List<Bounds> boundsInstances = new List<Bounds>();
						foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
							boundsInstances.Add(renderer.bounds);
						Bounds rotationBounds = BoundsExtensions.CombineBounds(boundsInstances.ToArray());
						rotationViewer.position = waypoint.position;
						float greatestRadius = Mathf.Max(rotationBounds.extents.x, rotationBounds.extents.y);
						foreach (Bounds bounds in boundsInstances)
						{
							Vector2 furthestPoint = bounds.min;
							float currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
							if (currentRadius > greatestRadius)
								greatestRadius = currentRadius;
							furthestPoint = bounds.max;
							currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
							if (currentRadius > greatestRadius)
								greatestRadius = currentRadius;
							furthestPoint = new Vector2(bounds.min.x, bounds.max.y);
							currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
							if (currentRadius > greatestRadius)
								greatestRadius = currentRadius;
							furthestPoint = new Vector2(bounds.max.x, bounds.min.y);
							currentRadius = Vector2.Distance(rotationBounds.center, furthestPoint);
							if (currentRadius > greatestRadius)
								greatestRadius = currentRadius;
						}
						rotationViewer.localScale = Vector2.one * ((greatestRadius + Vector2.Distance(rotationBounds.center, transform.position)) * 2);
						rotationViewer.eulerAngles = waypoint.eulerAngles;
						Scene currentScene = SceneManager.GetActiveScene();
						for (int i2 = 0; i2 < SceneManager.sceneCount; i2 ++)
						{
							SceneManager.SetActiveScene(SceneManager.GetSceneAt(i2));
							GameObject canvas = GameObject.Find("Canvas (World)");
							if (canvas != null)
							{
								rotationViewer.SetParent(canvas.transform);
								break;
							}
						}
						SceneManager.SetActiveScene(currentScene);
						rotationViewer.GetComponent<Image>().color = path.color;
						float rotaAmount = Mathf.Abs(Mathf.DeltaAngle(waypoint.eulerAngles.z, nextWaypoint.eulerAngles.z));
						if (MathfExtensions.RotationDirectionToAngle(waypoint.eulerAngles.z, nextWaypoint.eulerAngles.z) > 0 == backTracking)
							rotationViewer.GetComponent<Image>().fillClockwise = false;
						rotationViewer.GetComponent<Image>().fillAmount = 1f / (360f / rotaAmount);
					}
				}
			}
		}
		
		void FixedUpdate ()
		{
			if (GameManager.paused || GameManager.isInLevelTransition)
				return;
			if (moveSpeed != 0)
				transform.position = Vector2.Lerp(transform.position, wayPoints[currentWaypoint].transform.position, moveSpeed * (1f / Vector2.Distance(transform.position, wayPoints[currentWaypoint].transform.position)));
			if (rotateSpeed != 0)
				transform.rotation = Quaternion.Slerp(transform.rotation, wayPoints[currentWaypoint].transform.rotation, rotateSpeed * (1f / Quaternion.Angle(transform.rotation, wayPoints[currentWaypoint].transform.rotation)));
			if ((transform.position == wayPoints[currentWaypoint].transform.position || moveSpeed == 0) && (transform.up == wayPoints[currentWaypoint].transform.up || rotateSpeed == 0))
			{
				waitTimer.value.max = wayPoints[currentWaypoint].waitTime;
				waitTimer.Resume();
				if (waitTimer.IsAtEnd())
					OnReachedWaypoint ();
			}
		}
		
		public void OnReachedWaypoint ()
		{
			waitTimer.Pause();
			waitTimer.JumpToStart();
			if (backTracking)
				currentWaypoint --;
			else
				currentWaypoint ++;
			switch (moveType)
			{
				case MoveType.Once:
					if (currentWaypoint == wayPoints.Count)
					{
						hasTraveledFullCycle = true;
						currentWaypoint = wayPoints.Count - 1;
					}
					else if (currentWaypoint == -1)
					{
						hasTraveledFullCycle = true;
						currentWaypoint = 0;
					}
					return;
				case MoveType.Loop:
					if (currentWaypoint == wayPoints.Count)
						currentWaypoint = 0;
					else if (currentWaypoint == -1)
						currentWaypoint = wayPoints.Count - 1;
					return;
				case MoveType.PingPong:
					if (currentWaypoint == wayPoints.Count)
					{
						currentWaypoint -= 2;
						backTracking = !backTracking;
						hasTraveledFullCycle = true;
					}
					else if (currentWaypoint == -1)
					{
						currentWaypoint += 2;
						backTracking = !backTracking;
						hasTraveledFullCycle = true;
					}
					return;
			}
		}
	}

	public enum MoveType
	{
		Once = 0,
		Loop = 1,
		PingPong = 2
	}
}