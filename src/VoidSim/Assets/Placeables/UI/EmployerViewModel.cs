using System.Collections.Generic;
using Assets.Placeables.Nodes;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Placeables.UI
{
	public class EmployerViewModel : QScript
	{
		private PopEmployer _employer;
		[SerializeField] private Image _popImage;
		[SerializeField] private Color _employedColor;
		[SerializeField] private Color _freeColor;
		private readonly List<Image> _icons = new List<Image>();

		public void Bind(PopEmployer employer)
		{
			_employer = employer;
			_employer.OnEmployeesChanged += HandleEmployeeChanged;
			HandleEmployeeChanged();
		}

		private void HandleEmployeeChanged()
		{
			var employed = _employer.CurrentEmployeeCount;
			var open = _employer.MaxEmployeeCount - employed;
			_icons.ForEach(i => Destroy(i.gameObject));
			_icons.Clear();

			for (var i = 0; i < employed; i++)
			{
				AddPop(true);
			}

			for (var i = 0; i < open; i++)
			{
				AddPop(false);
			}
		}

		private void AddPop(bool isEmployed)
		{
			var popIcon = Instantiate(_popImage, transform);
			popIcon.color = isEmployed ? _employedColor : _freeColor;
			popIcon.gameObject.SetActive(true);
			_icons.Add(popIcon);
		}
	}
}