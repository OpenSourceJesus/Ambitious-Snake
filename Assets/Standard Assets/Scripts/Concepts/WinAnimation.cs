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
			GameManager.GetSingleton<LevelTimer>().timer.Stop ();
			GameManager.GetSingleton<Snake>().gameObject.SetActive(false);
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
				GameManager.GetSingleton<GameManager>().LoadLevel ("Level Select");
			}
		}
	}
}