using System;
using Assets.Station.Efficiency;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public static class TooltipStringGenerator
	{
		public static string Generate(EfficiencyModule module)
		{
			var str = "Overall   " + (module.CurrentAmount * 100).ToString("0") + "%\n";
			foreach (var affector in module.Affectors)
			{
				var percent = affector.Efficiency * 100;
				str += affector.Name + "  " + percent.ToString("0") + "% " + "\n";
			}

			str = str.Remove(str.Length - 2, 2);

			return str;
		}

		public static string Generate(EfficiencyModule module, GameColors colors)
		{
			var percent = module.CurrentAmount * 100;
			var str = "Overall   " + percent.ToString("0") + "%\n\n";

			foreach (var affector in module.Affectors)
			{
				percent = affector.Efficiency * 100;
				str += affector.Name + "  ";
				str += GenerateOpenColorTag(affector.Efficiency, module.MinimumAmount, colors);
				str += percent.ToString("0") + "% ";
				str += GenerateCloseColorTag();
				str += "\n";
			}

			str = str.Remove(str.Length - 2, 2);

			return str;
		}

		private static string GenerateCloseColorTag()
		{
			return "</color> ";
		}

		private static string GenerateOpenColorTag(float currentAmount, float moduleMinimumAmount, GameColors colors)
		{
			var range = 1.0f - moduleMinimumAmount;
			var current = currentAmount - moduleMinimumAmount;
			var openTag = "<color=";

			if (current > range)
				return openTag + HexString(colors.Go) + ">";
			if (Mathf.Abs(current - range) < .01f)
				return openTag + HexString(colors.TextNormal) + ">";
			if (current >= range / 2)
				return openTag + HexString(colors.Caution) + ">";

			return openTag + HexString(colors.Stop) + ">";
		}

		// from http://orbcreation.com/orbcreation/page.orb?974 "Convert Color to Hex String"
		public static string HexString(Color32 aColor, bool includeAlpha = true)
		{
			var rs = Convert.ToString(aColor.r, 16).ToUpper();
			var gs = Convert.ToString(aColor.g, 16).ToUpper();
			var bs = Convert.ToString(aColor.b, 16).ToUpper();
			var a_s = Convert.ToString(aColor.a, 16).ToUpper();
			while (rs.Length < 2) rs = "0" + rs;
			while (gs.Length < 2) gs = "0" + gs;
			while (bs.Length < 2) bs = "0" + bs;
			while (a_s.Length < 2) a_s = "0" + a_s;
			if (includeAlpha) return "#" + rs + gs + bs + a_s;
			return "#" + rs + gs + bs;
		}
	}
}