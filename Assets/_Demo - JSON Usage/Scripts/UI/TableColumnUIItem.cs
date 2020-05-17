namespace DemoJSON
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class TableColumnUIItem : MonoBehaviour
	{
		public string id;
		public TMPro.TextMeshProUGUI title;

		private bool _isHeader = false;
		public bool IsHeader
		{
			get { return _isHeader; }
			set
			{
				_isHeader = value;
				SetHeaderStyle(value);
			}
		}

		private void SetHeaderStyle(bool _asHeader = false)
		{
			title.fontStyle = _asHeader ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
		}

		public void Build(DemoTableColumnData _data, bool _asHeader = false)
		{
			_isHeader = _asHeader;
			id = _data.key;
			title.text = _data.value;
		}
	}
}