using UnityEngine;
using System.Collections;
using System;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// SteamSpyData.
	/// </summary>
	public class SteamSpyData : MonoBehaviour
	{
		/// <summary>
		/// SteamSpyView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("steamSpyView")]
		protected SteamSpyView SteamSpyView;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			Load();
		}

		/// <summary>
		/// Load data.
		/// </summary>
		public void Load()
		{
			StartCoroutine(LoadData());
		}

		/// <summary>
		/// Coroutine to load data from web.
		/// </summary>
		/// <returns></returns>
		protected IEnumerator LoadData()
		{
			WWW www = new WWW("https://ilih.ru/steamspy/");
			yield return www;

			var lines = www.text.Split('\n');

			www.Dispose();

			SteamSpyView.DataSource.BeginUpdate();

			SteamSpyView.DataSource.Clear();

			lines.ForEach(x => {
				var item = ParseLine(x);
				if (item!=null)
				{
					SteamSpyView.DataSource.Add(item);
				}
			});

			SteamSpyView.DataSource.EndUpdate();
		}

		/// <summary>
		/// Parse line and add to SteamSpyView.
		/// </summary>
		/// <param name="line">Line to parse.</param>
		/// <returns>Item.</returns>
		static protected SteamSpyItem ParseLine(string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return null;
			}
			
			var info = line.Split('\t');

			var item = new SteamSpyItem(){
				Name = info[0],
				ScoreRank = (string.IsNullOrEmpty(info[1])) ? -1 : int.Parse(info[1]),

				Owners = int.Parse(info[2]),
				OwnersVariance = int.Parse(info[3]),

				Players = int.Parse(info[4]),
				PlayersVariance = int.Parse(info[5]),

				PlayersIn2Week = int.Parse(info[6]),
				PlayersIn2WeekVariance = int.Parse(info[7]),

				AverageTimeIn2Weeks = int.Parse(info[8]),
				MedianTimeIn2Weeks = int.Parse(info[9]),
			};
			
			return item;
		}
	}
}