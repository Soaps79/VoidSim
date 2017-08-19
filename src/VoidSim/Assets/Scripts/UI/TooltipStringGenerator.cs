using System;
using Assets.Station.Efficiency;

namespace Assets.Scripts.UI
{
	public static class TooltipStringGenerator
	{
		public static string Generate(EfficiencyModule module)
		{
			var str = string.Empty;

			foreach (var affector in module.Affectors)
			{
				var percent = affector.Efficiency * 100;
				str += affector.Name + "  " + percent.ToString("0") + "% " + "\n";
			}

			str = str.Remove(str.Length - 2, 2);

			return str;
		}
	}
}