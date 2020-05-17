namespace DemoJSON
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class DemoTableColumnData
	{
		public string key;
		public string value;
	}

	[Serializable]
	public class DemoTableRowData
	{
		public List<DemoTableColumnData> items;
	}

	[Serializable]
	public class DemoTableData
	{
		public string Title;
		public List<string> ColumnHeaders;
		public List<DemoTableRowData> Data;
	}
}