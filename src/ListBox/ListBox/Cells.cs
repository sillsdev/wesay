using System;
using System.Data;
using SourceGrid3;

namespace ListBox
{
	public class Cell : SourceGrid3.Cells.Virtual.CellVirtual
	{
		public Cell(DataColumn column)
		{
			Model.AddModel(new ListBox.BindingGridValueModel());
		}
	}
//
	public class CheckBox : SourceGrid3.Cells.Virtual.CheckBox
	{
		public CheckBox(DataColumn column)
		{
			Model.AddModel(new BindingGridValueModel());
		}
	}
//
//    public class Image : SourceGrid3.Cells.Virtual.Image
//    {
//        public Image(DataColumn column)
//        {
//            Model.AddModel(new BindingGridValueModel());
//        }
//    }
//
//    public class Link : SourceGrid3.Cells.Virtual.Link
//    {
//        public Link(DataColumn column)
//        {
//            Model.AddModel(new BindingGridValueModel());
//        }
//    }
}
