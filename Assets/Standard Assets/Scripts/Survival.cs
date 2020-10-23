using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Extensions;

namespace AmbitiousSnake
{
	public class Survival : SingletonMonoBehaviour<Survival>
	{
		public Wave[] wavePrefabs = new Wave[0];
		float addToDifficulty = 1;
		float difficulty = 1;
		Wave[] waves = new Wave[0];
		public float addToTimeScale;
		uint currentWave = 0;
		public Text currentWaveText;
		public Text highestWaveText;
		public static bool Unlocked
		{
			get
			{
				return SaveAndLoadManager.GetValue<bool>("Survival unlocked", false);
			}
			set
			{
				SaveAndLoadManager.SetValue("Survival unlocked", value);
			}
		}
		public static uint Highscore
		{
			get
			{
				return SaveAndLoadManager.GetValue<uint>("Survival highscore", 0);
			}
			set
			{
				SaveAndLoadManager.SetValue("Survival highscore", value);
			}
		}
		public Animator timeScaleIncreaseAnim;
		public Transform trs;
		
		public override void Awake ()
		{
			base.Awake ();
			GameManager.Instance.SetPaused (false);
			Time.timeScale = 1;
			highestWaveText.text = "Highscore: " + Highscore;
		}
		
		public virtual void Update ()
		{
			for (int i = 0; i < waves.Length; i ++)
			{
				if (waves[i].IsDone())
				{
					DestroyImmediate(trs.GetChild(i).gameObject);
					waves.RemoveAt(i);
				}
			}
			if (trs.childCount == 0)
				NextWave ();
		}
		
		public virtual void NextWave ()
		{
			bool waveDifficultyExists = false;
			foreach (Wave wave in wavePrefabs)
			{
				if (wave.difficulty == (int) difficulty)
				{
					waveDifficultyExists = true;
					break;
				}
			}
			if (!waveDifficultyExists)
			{
				Time.timeScale += addToTimeScale;
				difficulty = 1;
				timeScaleIncreaseAnim.Play("Increase Time Scale");
			}
			Wave nextWave;
			do
			{
				nextWave = wavePrefabs[Random.Range(0, wavePrefabs.Length)];				
			} while (nextWave.difficulty != (int) difficulty);
			foreach (Wave requiredWave in nextWave.requiredWaves)
				Instantiate(requiredWave, transform);
			Instantiate(nextWave, transform);
			waves = new Wave[0];
			waves = waves.AddRange(GetComponentsInChildren<Wave>());
			difficulty += addToDifficulty;
			currentWave ++;
			currentWaveText.text = "Wave: " + currentWave;
			if (currentWave > Highscore)
			{
				Highscore = currentWave;
				highestWaveText.text = "Highscore: " + currentWave;
			}
		}
	}
}