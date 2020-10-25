using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class AutoClickButton : MonoBehaviour
{
	public Button button;
	public bool onEnable;
	public bool onAwake;
	public bool onStart;
	public bool onUpdate;
	public bool onDisable;
	public bool onDestroy;
	public bool onLevelLoaded;
	public bool onLevelUnloaded;
	public bool onTriggerEnter2D;
	
    public virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (button == null)
				button = GetComponent<Button>();
			return;
		}
#endif
		if (onEnable)
        	button.onClick.Invoke();
	}
	
	public virtual void Awake ()
	{
		if (onAwake)
			button.onClick.Invoke();
		if (onLevelLoaded)
			SceneManager.sceneLoaded += LevelLoaded;
		if (onLevelUnloaded)
			SceneManager.sceneUnloaded += LevelUnloaded;
	}
	
	public virtual void Start ()
	{
		if (onStart)
			button.onClick.Invoke();
	}
	
	public virtual void Update ()
	{
		if (onUpdate)
			button.onClick.Invoke();
	}
	
	public virtual void OnDisable ()
	{
		if (onDisable)
			button.onClick.Invoke();
	}
	
	public virtual void OnDestroy ()
	{
		if (onDestroy)
			button.onClick.Invoke();
	}
	
	public virtual void OnTriggerEnter2D (Collider2D other)
	{
		if (onTriggerEnter2D)
			button.onClick.Invoke();
	}
	
	public virtual void LevelLoaded (Scene scene, LoadSceneMode loadMode)
	{
		if (this != null)
			button.onClick.Invoke();
	}
	
	public virtual void LevelUnloaded (Scene scene)
	{
		button.onClick.Invoke();
	}
	
	public virtual void Trigger ()
	{
		button.onClick.Invoke();
	}
}
