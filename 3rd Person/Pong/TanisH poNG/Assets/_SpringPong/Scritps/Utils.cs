/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/



using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using DG.Tweening;

namespace AppAdvisory.SpringPong
{
	/// <summary>
	/// Utility class.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Compare colors.
		/// </summary>
		public static bool HaveSameColor(this DotBase dBase, DotBase other)
		{
			return dBase.color == other.color;
		}
		/// <summary>
		/// Compare colors.
		/// </summary>
		public static bool HaveSameColor(this Transform dBase, Transform other)
		{
			return dBase.GetComponent<DotBase>().color == other.GetComponent<DotBase>().color;
		}
		/// <summary>
		/// Compare colors.
		/// </summary>
		public static bool HaveSameColor(this DotColor c, Transform other)
		{
			return c == other.GetComponent<DotBase>().color;
		}
		/// <summary>
		/// Set the best score.
		/// </summary>
		public static bool SetBest(int lastScore)
		{
			int best = GetBest();

			if(lastScore > best)
			{
				PlayerPrefs.SetInt("BEST_SCORE",lastScore);
				PlayerPrefs.Save();
				return true;
			}

			return false;
		}
		/// <summary>
		/// Get the best score.
		/// </summary>
		public static int GetBest()
		{
			return PlayerPrefs.GetInt("BEST_SCORE",0);
		}
		/// <summary>
		/// Set the last score.
		/// </summary>
		public static void SetLast(int lastScore)
		{
			PlayerPrefs.SetInt("LAST_SCORE",lastScore);
			PlayerPrefs.Save();
		}
		/// <summary>
		/// Get the last score.
		/// </summary>
		public static int GetLast()
		{
			return PlayerPrefs.GetInt("LAST_SCORE", 0);
		}
		/// <summary>
		/// Set alpha of Spriterenderer element.
		/// </summary>
		public static void SetAlpha(this SpriteRenderer sr, float a)
		{
			Color c = sr.color;
			c.a = a;
			sr.color = c;
		}
		/// <summary>
		/// Set alpha of UI Image element.
		/// </summary>
		public static void SetAlpha(this Image im, float a)
		{
			Color c = im.color;
			c.a = a;
			im.color = c;
		}
		/// <summary>
		/// Set alpha of UI Text element.
		/// </summary>
		public static void SetAlpha(this Text t, float a)
		{
			Color c = t.color;
			c.a = a;
			t.color = c;
		}

		/// <summary>
		/// Clean the memory and reload the scene
		/// </summary>
		public static void ReloadLevel()
		{
			CleanMemory();

			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif

			CleanMemory();
		}
		/// <summary>
		/// Clean the memory
		/// </summary>
		public static void CleanMemory()
		{
			DOTween.KillAll();
			GC.Collect();
			Application.targetFrameRate = 60;
		}
	}
}