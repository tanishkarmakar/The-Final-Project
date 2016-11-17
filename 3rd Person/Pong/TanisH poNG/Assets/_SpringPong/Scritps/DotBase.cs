/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/

using UnityEngine;
using System.Collections;

namespace AppAdvisory.SpringPong
{
	/// <summary>
	/// Player (= "ball who bounce") and "big dots" (the 2 dots on the top and the 2 dots on the bottom) inherite from this class.
	/// We handle seom references and intialize sprites.
	/// </summary>
	public class DotBase : MonoBehaviour 
	{
		/// <summary>
		/// The dot color: Pink or Blue. Please refer to DotColor.
		/// </summary>
		public DotColor color;
		/// <summary>
		/// Reference to the GameManager.
		/// </summary>
		public GameManager gm;
		/// <summary>
		/// Reference to the SpriteRenderer.
		/// </summary>
		public SpriteRenderer sr;
		/// <summary>
		/// Some initializations.
		/// </summary>
		virtual public void Awake()
		{
			gm = FindObjectOfType<GameManager>();

			sr = GetComponent<SpriteRenderer>();
		}
		/// <summary>
		/// Set the color of the SpriteRenderer.
		/// </summary>
		public void SetColor(DotColor c)
		{
			if(c == DotColor.Pink)
			{
				color = DotColor.Pink;

				sr.color = gm.colorPink;
			}
			else
			{
				color = DotColor.Blue;

				sr.color = gm.colorBlue;
			}
		}
	}
}