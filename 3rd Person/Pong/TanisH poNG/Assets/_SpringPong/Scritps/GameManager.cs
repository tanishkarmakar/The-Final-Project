/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif
#if VS_SHARE
using AppAdvisory.SharingSystem;
#endif
using AppAdvisory.UI;
#if APPADVISORY_LEADERBOARD
using AppAdvisory.social;
#endif



namespace AppAdvisory.SpringPong
{
	/// <summary>
	/// In Charge to all the game management (game over, point, restart etc..) and in charge to show interstitial in the game.
	/// For monetizing this game with ads, everythign is already coded for you. You just need to get VERY SIMPLE ADS here: http://u3d.as/oWD
	/// </summary>
	public class GameManager : MonoBehaviour 
	{
		#region variables
		public Text m_textInGame;
		Text textInGame
		{
			get
			{
				if(m_textInGame == null)
					m_textInGame = FindObjectOfType<AppAdvisory.UI.UIController>().scoreIngame;

				return m_textInGame;
			}
		}

		/// <summary>
		/// If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
		/// </summary>
		public string VerySimpleAdsURL = "http://u3d.as/oWD";
		/// <summary>
		/// Number of "play" to show an interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
		/// </summary>
		public int numberOfPlayToShowInterstitial = 10;
		/// <summary>
		/// Reference to the player (ie. the the centered ball).
		/// </summary>
		public Transform player;
		/// <summary>
		/// The pink color, change it to change the color.
		/// </summary>
		public Color colorPink;
		/// <summary>
		/// The blue color, change it to change the color.
		/// </summary>
		public Color colorBlue;
		/// <summary>
		/// Speed of the ball (= Player) to make a complete move. Change it to change the speed.
		/// </summary>
		public float timeToMoveFromTopToBottom = 0.6f;
		/// <summary>
		/// Speed of the big dots on the top and the bottom is equal to the timeToMoveFromTopToBottom divide by that. Change it to change the speed of the big dots move.
		/// </summary>
		float timeToMoveDotsDivisor = 20f;
		/// <summary>
		/// Reference to the big dot on the top, pink.
		/// </summary>
		public Transform dotUpPink;
		/// <summary>
		/// Reference to the big dot on the top, blue.
		/// </summary>
		public Transform dotUpBlue;
		/// <summary>
		/// Reference to the big dot on the bottom, pink.
		/// </summary>
		public Transform dotDownPink;
		/// <summary>
		/// Reference to the big dot on the bottom, blue.
		/// </summary>
		public Transform dotDownBlue;
		/// <summary>
		/// Reference to the SpriteRenderer of the big dot on the top, pink.
		/// </summary>
		SpriteRenderer _dotUpPinkSR;
		/// <summary>
		/// Reference to the SpriteRenderer of the big dot on the top, blue.
		/// </summary>
		SpriteRenderer _dotUpBlueSR;
		/// <summary>
		/// Reference to the SpriteRenderer of the big dot on the bottom, pink.
		/// </summary>
		SpriteRenderer _dotDownPinkSR;
		/// <summary>
		/// Reference to the SpriteRenderer of the big dot on the top, blue.
		/// </summary>
		SpriteRenderer _dotDownBlueSR;
		/// <summary>
		/// Reference to the SpriteRenderer of the Player.
		/// </summary>
		SpriteRenderer _playerSR;
		/// <summary>
		/// Reference to the text title.
		/// </summary>
		public Text textTitle;
		/// <summary>
		/// Reference to the text score displayed in the center of the screen during the game.
		/// </summary>
		public Text textScore;
		/// <summary>
		/// True if the game is started. Used to block some method if the game is not started.
		/// </summary>
		bool isStarted = false;
		/// <summary>
		/// True if the game is Game Over. Used to block some method after the player loses.
		/// </summary>
		bool isGameOver = false;
		/// <summary>
		/// We move the ball with a tweener, from the top to the bottom, and bottom from top. We place the big dots with the same top position and bottom position of the ball.
		/// But we don't want to have the ball go inside the big dots. So we decal the position with this value.
		/// </summary>
		float decal = 0.45f;
		/// <summary>
		/// References to all the audioclips use when the ball bounce on a big dot. Change it , add some... to customize it.
		/// </summary>
		public AudioClip[] pocs;
		/// <summary>
		/// Reference to the auidioclip play when he player loses.
		/// </summary>
		public AudioClip lose;
		/// <summary>
		/// DotColor of the the top position, blue by default at start
		/// </summary>
		DotColor currentColorUp = DotColor.Blue;
		/// <summary>
		/// DotColor of the bottom position, pink by default at start
		/// </summary>
		DotColor currentColorDown = DotColor.Pink;
		/// <summary>
		/// DotColor of Player, ie. the ball who moves.
		/// </summary>
		DotColor currentColor
		{
			get
			{
				if(player.transform.position.y > 0)
				{
					return currentColorUp;
				}
				else
				{
					return currentColorDown;
				}
			}
		}
		/// <summary>
		/// The point in the current game.
		/// </summary>
		int _point = 0;
		/// <summary>
		/// The point in the current game.
		/// </summary>
		int point
		{
			get
			{
				return _point;
			}

			set
			{
				_point = value;

				textScore.text = _point.ToString();

				if(value == 0)
				{
					textScore.SetAlpha(0);
				}
				else if(textScore.color.a == 0)
				{
					textScore.DOFade(1f, 1f);
				}

			}
		}
		#endregion

		/// <summary>
		/// Some iitializations.
		/// </summary>
		void Awake()
		{
			_dotUpPinkSR = this.dotUpPink.GetComponent<SpriteRenderer>();
			_dotUpBlueSR = this.dotUpBlue.GetComponent<SpriteRenderer>();
			_dotDownPinkSR = this.dotDownPink.GetComponent<SpriteRenderer>();
			_dotDownBlueSR = this.dotDownBlue.GetComponent<SpriteRenderer>();
			_playerSR = this.player.GetComponent<SpriteRenderer>();

			Application.targetFrameRate = 60;
			GC.Collect();

			if(Time.realtimeSinceStartup < 2)
			{
				DOTween.Init();
			}

			point = 0;



			DOAlphaDots(0);

			SetInGameElementsActive(false);

			FindObjectOfType<AppAdvisory.UI.UIController>().textLast.text = "LAST\n" + Utils.GetLast();
			FindObjectOfType<AppAdvisory.UI.UIController>().textBest.text = "BEST\n" + Utils.GetBest();

			//			StartTheUI();
		}
		//		/// <summary>
		//		/// At Start, we set the UI.
		//		/// </summary>
		//		void StartTheUI()
		//		{
		//			var UI = FindObjectOfType<AppAdvisory.UI.UIController>();
		//
		//			UI.SetBestText(Utils.GetBest());
		//			UI.SetBestText(Utils.GetLast());
		//			UI.DOAnimIN();
		//		}
		/// <summary>
		/// Activate or desactivate in game elements: the 4 big dots, and the player ball.
		/// </summary>
		void SetInGameElementsActive(bool setActive)
		{
			dotDownBlue.gameObject.SetActive(setActive);
			dotDownPink.gameObject.SetActive(setActive);
			dotUpBlue.gameObject.SetActive(setActive);
			dotUpPink.gameObject.SetActive(setActive);
			player.gameObject.SetActive(setActive);
		}
		/// <summary>
		/// Method called when we touch the screen (or click on desktop).
		/// </summary>
		void OnTouched (TouchDirection td)
		{
			if(isGameOver)
			{
				return;
			}

			if(!isGameOver)
			{
				if(Input.GetMouseButtonDown(0))
				{
					DOMoveDots();
				}
			}
		}
		/// <summary>
		/// Move the 4 big dots when the player touches the screen.
		/// </summary>
		void DOMoveDots()
		{
			dotUpBlue.DOKill(true);
			dotUpPink.DOKill(true);
			dotDownBlue.DOKill(true);
			dotDownPink.DOKill(true);

			if(dotUpBlue.position.x <= 0.01f && dotUpBlue.position.x >= -0.01f)
			{
				currentColorUp = DotColor.Pink;
				currentColorDown = DotColor.Blue;

				dotUpBlue.DOMoveX(1f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotUpPink.DOMoveX(0f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotDownBlue.DOMoveX(0f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotDownPink.DOMoveX(-1f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);
			}
			else
			{
				currentColorUp = DotColor.Blue;
				currentColorDown = DotColor.Pink;

				dotUpBlue.DOMoveX(0f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotUpPink.DOMoveX(-1f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotDownBlue.DOMoveX(1f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);

				dotDownPink.DOMoveX(0f, timeToMoveFromTopToBottom / timeToMoveDotsDivisor).SetEase(Ease.Linear);
			}
		}
		/// <summary>
		/// Fade in the alpha of the in game elements: the 4 big dots, and the player ball.
		/// </summary>
		public void DoFadeIn()
		{
			DoFadeIn(null);
		}
		/// <summary>
		/// Fade in the alpha of the in game elements: the 4 big dots, and the player ball.
		/// </summary>
		public void DoFadeIn(Action isCompleted)
		{
			SetInGameElementsActive(true);

			DOAlphaDots(0f);
			DOVirtual.Float(0f, 1f, 0.5f, DOAlphaDots)
				.OnComplete(() => {
					if(isCompleted != null)
						isCompleted();
				});
		}
		void UpdateTextInGameAlpha(float f)
		{
			Color c = textInGame.color;
			c.a = f;
			textInGame.color = c;
		}
		/// <summary>
		/// Fade out the alpha of the in game elements: the 4 big dots, and the player ball.
		/// </summary>
		public void DoFadeOut()
		{
			DoFadeOut(null);
		}
		/// <summary>
		/// Fade out the alpha of the in game elements: the 4 big dots, and the player ball.
		/// </summary>
		public void DoFadeOut(Action isCompleted)
		{
			SetInGameElementsActive(true);

			DOAlphaDots(1f);
			DOVirtual.Float(1f, 0f, 0.5f, DOAlphaDots)
				.OnComplete(() => {
					if(isCompleted != null)
						isCompleted();
				});
		}
		/// <summary>
		/// Change the alpha of the in game elements: the 4 big dots, and the player ball.
		/// </summary>
		void DOAlphaDots(float a)
		{
			UpdateTextInGameAlpha(a);

			_dotUpBlueSR.SetAlpha(a);
			_dotUpPinkSR.SetAlpha(a);
			_dotDownBlueSR.SetAlpha(a);
			_dotDownPinkSR.SetAlpha(a);
			_playerSR.SetAlpha(a);
		}
		/// <summary>
		/// Method called by the UIController (have a look to the UIController GameObject editor, in the "On UI Anim Out End".
		/// </summary>
		public void DOStart()
		{
			#if VS_SHARE
			VSSHARE.DOHideScreenshotIcon();
			#endif

			InputTouch.OnTouched += OnTouched;

			if(dotUpBlue.position.x <= 0.01f && dotUpBlue.position.x >= -0.01f)
			{
				currentColorUp = DotColor.Blue;
				currentColorDown = DotColor.Pink;
			}
			else
			{
				currentColorUp = DotColor.Pink;
				currentColorDown = DotColor.Blue;
			}

			player.GetComponent<DotBase>().SetColor(currentColorUp);

			isStarted = true;
			isGameOver = false;

			StopAllCoroutines();

			point = 0;

			player.DOMoveY(dotUpPink.position.y - decal,timeToMoveFromTopToBottom / 2f).SetEase(Ease.Linear).OnComplete(DOMovePlayer);
		}
		/// <summary>
		/// Move the player, ie. the bounce between top and bottom, and bottom and top.
		/// </summary>
		void DOMovePlayer()
		{
			float playerPosY = player.position.y;

			if(currentColor.HaveSameColor(player) == false)
			{
				DOGameOver();
				return;
			}

			point++;

			PlaySoundPoc();

			if(point == 1)
			{
				player.GetComponent<Player>().SetColor(DotColor.Pink);
			}
			else
			{
				player.GetComponent<Player>().RandomColor();
			}

			float moveToY = dotUpPink.position.y - decal;

			if(playerPosY > 0)
			{
				moveToY = dotDownPink.position.y + decal;
			}

			player.DOMoveY(moveToY, timeToMoveFromTopToBottom).SetEase(Ease.Linear).OnComplete(DOMovePlayer);
		}
		/// <summary>
		/// Method called when the player loses. We will fade out all the in game elements(the 4 big dots, and the player ball) and restart the scene.
		/// </summary>
		void DOGameOver()
		{
			InputTouch.OnTouched -= OnTouched;

			FindObjectOfType<AppAdvisory.UI.UIController>().DOTakeScreenshotWithVerySimpleShare();

			isGameOver = true;
			isStarted = false;

			PlaySoundLose();

			StopAllCoroutines();
			DOTween.KillAll();

			Utils.SetBest(point);
			Utils.SetLast(point);

			player.DOMoveY(Mathf.Sign(player.transform.position.y) * Mathf.Abs(dotUpPink.position.y - decal) * 3, timeToMoveFromTopToBottom * 2)
				.SetEase(Ease.Linear);

			textScore.DOFade(0f, timeToMoveFromTopToBottom * 1.5f);

			DOVirtual.Float(1f, 0f, timeToMoveFromTopToBottom * 1.5f, delegate(float value) {
				textScore.SetAlpha(value);
				DOAlphaDots(value);
			})
				.OnComplete(() => {

					SetInGameElementsActive(false);

					Utils.ReloadLevel();
				});

			ReportScoreToLeaderboard(point);

			FindObjectOfType<AppAdvisory.UI.UIController>().textLast.text = "LAST\n" + Utils.GetLast();
			FindObjectOfType<AppAdvisory.UI.UIController>().textBest.text = "BEST\n" + Utils.GetBest();

			ShowAds();

			#if VS_SHARE
			VSSHARE.DOTakeScreenShot();
			#endif
		}
		/// <summary>
		/// "Real" random use to select the "poc" sound to display when the player ball bounce on a big dot.
		/// </summary>
		private System.Random rand = new System.Random();
		/// <summary>
		/// Play a "poc" sound to display when the player ball bounce on a big dot.
		/// </summary>
		void PlaySoundPoc()
		{
			int i = rand.Next(0,pocs.Length);
			GetComponent<AudioSource>().PlayOneShot(pocs[i]);
		}
		/// <summary>
		/// Play the GameOver sound.
		/// </summary>
		void PlaySoundLose()
		{
			GetComponent<AudioSource>().PlayOneShot(lose);
		}
		/// <summary>
		/// If using Very Simple Leaderboard by App Advisory, report the score : http://u3d.as/qxf
		/// </summary>
		void ReportScoreToLeaderboard(int p)
		{
			#if APPADVISORY_LEADERBOARD
			LeaderboardManager.ReportScore(p);
			#else
			print("Get very simple leaderboard to use it : http://u3d.as/qxf");
			#endif
		}
		/// <summary>
		/// Show interstitial ads. For monetizing this game with ads, everythign is already coded for you. You just need to get VERY SIMPLE ADS here: http://u3d.as/oWD
		/// </summary>
		public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;
			PlayerPrefs.SetInt("GAMEOVER_COUNT",count);
			PlayerPrefs.Save();

			#if APPADVISORY_ADS
			if(count > numberOfPlayToShowInterstitial)
			{
			print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);

			if(AdsManager.instance.IsReadyInterstitial())
			{
			print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
			AdsManager.instance.ShowInterstitial();
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			}
			else
			{
			#if UNITY_EDITOR
			print("AdsManager.instance.IsReadyInterstitial() == false");
			#endif
		}

	}
	else
	{
		PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
	}
	PlayerPrefs.Save();
			#else
	if(count >= numberOfPlayToShowInterstitial)
	{
		Debug.LogWarning("To show ads, please have a look to Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
		Debug.LogWarning("Very Simple Ad is already implemented in this asset");
		Debug.LogWarning("Just import the package and you are ready to use it and monetize your game!");
		Debug.LogWarning("Very Simple Ad : " + VerySimpleAdsURL);
		PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
	}
	else
	{
		PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
	}
	PlayerPrefs.Save();
			#endif


}

public void OnUIAnimOutStarted()
{
	FindObjectOfType<AppAdvisory.UI.UIController>().HideVerySimpleShare();
}

}
}