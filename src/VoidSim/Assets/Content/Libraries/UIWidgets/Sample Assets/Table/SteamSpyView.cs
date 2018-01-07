﻿using System.Collections.Generic;
using System;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// SteamSpy sort fields.
	/// </summary>
	public enum SteamSpySortFields
	{
		/// <summary>
		/// Name.
		/// </summary>
		Name,

		/// <summary>
		/// ScoreRank.
		/// </summary>
		ScoreRank,

		/// <summary>
		/// Owners.
		/// </summary>
		Owners,

		/// <summary>
		/// Players.
		/// </summary>
		Players,

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		PlayersIn2Week,

		/// <summary>
		/// Time.
		/// </summary>
		Time,
	}

	/// <summary>
	/// SteamSpyView.
	/// </summary>
	public class SteamSpyView : TileViewCustomSize<SteamSpyComponent,SteamSpyItem>
	{
		bool isSteamSpyViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isSteamSpyViewInited)
			{
				return ;
			}

			isSteamSpyViewInited = true;

			sortComparers = new Dictionary<int,Comparison<SteamSpyItem>>(){
				{(int)SteamSpySortFields.Name, NameComparer},
				{(int)SteamSpySortFields.ScoreRank, ScoreRankComparer},
				{(int)SteamSpySortFields.Owners, OwnersComparer},
				{(int)SteamSpySortFields.Players, PlayersComparer},
				{(int)SteamSpySortFields.PlayersIn2Week, PlayersIn2WeekComparer},
				{(int)SteamSpySortFields.Time, TimeComparer},
			};

			Sort = false;
			base.Init();
		}

		SteamSpySortFields currentSortField = SteamSpySortFields.Players;

		Dictionary<int,Comparison<SteamSpyItem>> sortComparers;

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(SteamSpySortFields field)
		{
			if (field==currentSortField)
			{
				DataSource.Reverse();
			}
			else if (sortComparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(sortComparers[(int)field]);
			}
		}

		#region used in Button.OnClick()
		/// <summary>
		/// Sort by Name.
		/// </summary>
		public void SortByName()
		{
			ToggleSort(SteamSpySortFields.Name);
		}

		/// <summary>
		/// Sort by ScoreRank.
		/// </summary>
		public void SortByScoreRank()
		{
			ToggleSort(SteamSpySortFields.ScoreRank);
		}

		/// <summary>
		/// Sort by Owners.
		/// </summary>
		public void SortByOwners()
		{
			ToggleSort(SteamSpySortFields.Owners);
		}

		/// <summary>
		/// Sort by Players.
		/// </summary>
		public void SortByPlayers()
		{
			ToggleSort(SteamSpySortFields.Players);
		}

		/// <summary>
		/// Sort by PlayersIn2Week.
		/// </summary>
		public void SortByPlayersIn2Week()
		{
			ToggleSort(SteamSpySortFields.PlayersIn2Week);
		}

		/// <summary>
		/// Sort by Time.
		/// </summary>
		public void SortByTime()
		{
			ToggleSort(SteamSpySortFields.Time);
		}
		#endregion

		#region Items comparers
		/// <summary>
		/// Name comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int NameComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.Name.CompareTo(y.Name);
		}

		/// <summary>
		/// ScoreRank comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int ScoreRankComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.ScoreRank.CompareTo(y.ScoreRank);
		}

		/// <summary>
		/// Owners comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int OwnersComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.Owners.CompareTo(y.Owners);
		}

		/// <summary>
		/// Players comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int PlayersComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.Players.CompareTo(y.Players);
		}

		/// <summary>
		/// PlayersIn2Week comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int PlayersIn2WeekComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.PlayersIn2Week.CompareTo(y.PlayersIn2Week);
		}

		/// <summary>
		/// AverageTimeIn2Weeks comparer.
		/// </summary>
		/// <param name="x">First SteamSpyItem.</param>
		/// <param name="y">Second SteamSpyItem.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static protected int TimeComparer(SteamSpyItem x, SteamSpyItem y)
		{
			return x.AverageTimeIn2Weeks.CompareTo(y.AverageTimeIn2Weeks);
		}
		#endregion
	}
}