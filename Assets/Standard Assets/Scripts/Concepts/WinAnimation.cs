using UnityEngine;
using System.Collections.Generic;
using Extensions;
using AmbitiousSnake;

namespace AmbitiousSnake
{
	public class WinAnimation : NotPartOfLevelEditor, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public static WinAnimation instance;
		public List<Animation> anims = new List<Animation>();
		Animation anim;
		bool animStarted;
		
		public override void Awake ()
		{
			base.Awake ();
			instance = this;
		}
		
		void OnEnable ()
		{
			LevelTimer.Instance.timer.Stop ();
			Snake.instance.gameObject.SetActive(false);
			for (int i = 0; i < anims.Count; i ++)
	        {
	            anim = anims[i];
				if (!SaveAndLoadManager.GetValue<bool>(anim.name + " Unlocked", false))
	            {
		            Destroy(anim.gameObject);
	                anims.RemoveAt(i);
	                i --;
	            }
	        }
			if (anims.Count > 0)
			{
				anim = anims[Random.Range(0, anims.Count)];
				anim.Play();
				animStarted = true;
			}
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
		
		public virtual void DoUpdate ()
		{
			if ((animStarted && !anim.isPlaying) || anims.Count == 0)
			{
				enabled = false;
				GameManager.Instance.LoadLevel ("Level Select");
			}
		}
	}
}