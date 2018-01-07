using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Country Flag.
	/// </summary>
	[System.Serializable]
	public class CountryFlag
	{
		/// <summary>
		/// County code.
		/// </summary>
		[SerializeField]
		public string Country;

		/// <summary>
		/// Flag sprite.
		/// </summary>
		[SerializeField]
		public Sprite Flag;
	}

	/// <summary>
	/// SteamSpy component.
	/// </summary>
	public class SteamSpyComponent : ListViewItem, IResizableItem, IViewData<SteamSpyItem>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Name, ScoreRank, Owners, Players, PlayersIn2Week, TimeIn2Week, };
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsBackground {
			get {
				return new Graphic[] { };
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// ScoreRank.
		/// </summary>
		[SerializeField]
		public Text ScoreRank;
		
		/// <summary>
		/// Owners.
		/// </summary>
		[SerializeField]
		public Text Owners;

		/// <summary>
		/// Players.
		/// </summary>
		[SerializeField]
		public Text Players;

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		[SerializeField]
		public Text PlayersIn2Week;

		/// <summary>
		/// TimeIn2Week.
		/// </summary>
		[SerializeField]
		public Text TimeIn2Week;

		/// <summary>
		/// TooltipText.
		/// </summary>
		[SerializeField]
		public Text TooltipText;

		/// <summary>
		/// Flag.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public Image Flag;

		/// <summary>
		/// Flags.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public CountryFlag[] Flags;

		//SteamSpyItem Item;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize {
			get {
				return new GameObject[] {
					Name.transform.parent.gameObject,
					//Flag.transform.parent.gameObject,
					ScoreRank.transform.parent.gameObject,
					Owners.transform.parent.gameObject,
					Players.transform.parent.gameObject,
					PlayersIn2Week.transform.parent.gameObject,
					TimeIn2Week.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SteamSpyItem item)
		{
			//Item = item;

			Name.text = item.Name;
			TooltipText.text = item.Name;
			ScoreRank.text = (item.ScoreRank==-1) ? string.Empty : item.ScoreRank.ToString();
			Owners.text = item.Owners.ToString("N0") + "\n±" + item.OwnersVariance.ToString("N0");
			Players.text = item.Players.ToString("N0") + "\n±" + item.PlayersVariance.ToString("N0");
			PlayersIn2Week.text = item.PlayersIn2Week.ToString("N0") + "\n±" + item.PlayersIn2WeekVariance.ToString("N0");
			TimeIn2Week.text = Minutes2String(item.AverageTimeIn2Weeks) + "\n(" + Minutes2String(item.MedianTimeIn2Weeks) + ")";

			//Flag.sprite = GetFlag("uk");
		}

		/// <summary>
		/// Get flag sprite by country code.
		/// </summary>
		/// <param name="country">Country code.</param>
		/// <returns>Flag sprite if country code found; otherwise, false.</returns>
		protected virtual Sprite GetFlag(string country)
		{
			foreach (var flag in Flags)
			{
				if (flag.Country==country)
				{
					return flag.Flag;
				}
			}
			return null;
		}

		static string Minutes2String(int minutes)
		{
			return string.Format("{0:00}:{1:00}", minutes / 60, minutes % 60);
		}
	}
}