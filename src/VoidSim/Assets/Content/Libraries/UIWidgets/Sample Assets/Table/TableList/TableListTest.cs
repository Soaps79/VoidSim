using UnityEngine;
using UIWidgets;
using System;
using System.Collections.Generic;

namespace UIWidgetsSamples
{
	[RequireComponent(typeof(TableList))]
	public class TableListTest : MonoBehaviour
	{
		/// <summary>
		/// The table.
		/// </summary>
		[NonSerialized]
		public TableList Table;

		/// <summary>
		/// The header.
		/// </summary>
		[SerializeField]
		public ResizableHeader Header;

		/// <summary>
		/// The header cell.
		/// </summary>
		[SerializeField]
		public TableListCell HeaderCell;

		/// <summary>
		/// The row cell.
		/// </summary>
		[SerializeField]
		public TableListCell RowCell;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			Table = GetComponent<TableList>();

			Table.Init();
			Table.DataSource = Generate(100, 10);

			Header.Refresh();
		}

		/// <summary>
		/// Generate test data.
		/// </summary>
		/// <param name="rows">Rows.</param>
		/// <param name="columns">Columns.</param>
		static protected ObservableList<List<int>> Generate(int rows, int columns)
		{
			var data = new ObservableList<List<int>>();
			for (int i = 0; i < rows; i++)
			{
				var row = new List<int>(columns);
				for (int j = 0; j < columns; j++)
				{
					row.Add(columns * i + j);
				}
				data.Add(row);
			}

			return data;
		}

		int columns = 2;

		/// <summary>
		/// Add the column.
		/// </summary>
		public void AddColumn(string columnName)
		{
			columns += 1;

			Table.ForEachComponent(AddCellToRow);
			Table.ComponentsColoring();

			AddCellToHeader(columnName + " " + columns);
		}
		
		/// <summary>
		/// Add the cell to row.
		/// </summary>
		/// <param name="row">Row.</param>
		protected void AddCellToRow(TableListComponent row)
		{
			var cell = Instantiate(RowCell) as TableListCell;
			cell.transform.SetParent(row.transform, false);
			cell.gameObject.SetActive(true);

			row.TextComponents.Add(cell.Text);
			row.UpdateView();
		}

		/// <summary>
		/// Add the cell to header.
		/// </summary>
		protected void AddCellToHeader(string columnName)
		{
			var cell = Instantiate(HeaderCell) as TableListCell;
			cell.Text.text = columnName;
			Header.AddCell(cell.gameObject);
		}

		int cellToDelete;

		/// <summary>
		/// Removes the column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveColumn(int index)
		{
			cellToDelete = index;
			Table.ForEachComponent(RemoveCellFromRow);

			RemoveCellFromHeader(index);
		}

		/// <summary>
		/// Removes the cell from row.
		/// </summary>
		/// <param name="row">Row.</param>
		protected void RemoveCellFromRow(TableListComponent row)
		{
			var cell = row.transform.GetChild(cellToDelete);
			cell.gameObject.SetActive(false);
			cell.transform.SetParent(null, false);

			row.TextComponents.RemoveAt(cellToDelete);
			row.UpdateView();
		}

		/// <summary>
		/// Removes the cell from header.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void RemoveCellFromHeader(int index)
		{
			var cell = Header.transform.GetChild(index);
			Header.RemoveCell(cell.gameObject);
		}
	}
}