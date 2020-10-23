using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace AmbitiousSnake
{
	public class WinAnimation : NotPartOfLevelEditor
	{
		public static WinAnimation instance;
		public List<Animation> anims = new List<Animation>();
		Animation anim;
		bool animStarted;
		
		public override void Awake ()
		{
			base.Awake ();
			instance = this;
		}
		
		public virtual void OnEnable ()
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
		}
		
		public virtual void Update ()
		{
			if ((animStarted && !anim.isPlaying) || anims.Count == 0)
			{
				enabled = false;
				GameManager.Instance.LoadLevel ("Level Select");
			}
		}
	}
}