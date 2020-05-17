namespace DemoJSON
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class TableRowUIItem : MonoBehaviour
	{
		public Image background;
		public TableColumnUIItem tableColumnUIItemPrefab;
		public List<TableColumnUIItem> tableColumnBuiltUIItems = new List<TableColumnUIItem>();

		public void Build(List<DemoTableColumnData> items, bool _asHeader)
		{
			CleanBuiltItems();
			this.DelayedActionAfterEndOfFrame(() =>
			{
			   foreach (DemoTableColumnData _item in items)
			   {
				   TableColumnUIItem _clon = Instantiate(tableColumnUIItemPrefab, this.transform);
				   _clon.Build(_item, _asHeader);
			   }
		   });
		}

		public void CleanBuiltItems()
		{
			foreach(TableColumnUIItem _item in tableColumnBuiltUIItems)
			{
				_item.gameObject.SetActive(false);
				Destroy(_item.gameObject, 0.1f);
			}
		}
	}
}