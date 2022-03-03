using DXVision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class PFMHelper
    {
        public static List<TableItem> TableItems = new List<TableItem>();

    }

    public class TableItem
    {
        public string TableName = "";

        public DXTableManager Content = null;

        public TableItem(string TableName, DXTableManager Content)
        {
            this.TableName = TableName;
            this.Content = Content;
        }

        public TableItem()
        { 
        }
    }
}
