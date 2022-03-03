using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PackFileManager.FormManager
{
    public class ControlHelper
    {
        public int SearchFinds = 0;
        public int SearchDeep = 0;
        public UIElement ThisFindControl = null;
        public void SetControlInForm(UIElement AnyControl,Grid Parent,string SearchName)
        {
            foreach (var GetItem in Parent.Children)
            {
                if (GetItem is Grid)
                {
                    Grid CurrentGrid = (Grid)GetItem;
                    if (CurrentGrid.Children.Count > 0)
                    {
                        this.SearchDeep++;
                        SetControlInForm(AnyControl, CurrentGrid, SearchName);
                    }
                    else
                    if (CurrentGrid.Name == SearchName)
                    {
                        CurrentGrid.Children.Add(AnyControl);
                        SearchFinds++;
                        break;
                    }
                }
            }

        }

        public void GetControlInForm(Grid Parent, string SearchName)
        {
            foreach (var GetItem in Parent.Children)
            {
                if (GetItem is Canvas)
                {
                    Canvas CurrentCanvas = (Canvas)GetItem;
                    if (CurrentCanvas.Name == SearchName)
                    {
                        SearchFinds++;
                        ThisFindControl = CurrentCanvas;
                        break;
                    }
                    else
                    { 
                    
                    }
                }
                if (GetItem is Grid)
                {
                    Grid CurrentGrid = (Grid)GetItem;
                    if (CurrentGrid.Children.Count > 0)
                    {
                        this.SearchDeep++;
                        GetControlInForm(CurrentGrid, SearchName);
                    }
                }
            }
        }
    }
}
