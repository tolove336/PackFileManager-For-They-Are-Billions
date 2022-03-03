using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PackFileManager.PFMCore
{
    public class RichtextBoxChangeFontColor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="richTextBox1"></param>
        /// <param name="selectLength"></param>
        /// <param name="tpStart"></param>
        /// <param name="tpEnd"></param>
        /// <returns></returns>
        public TextPointer selecta(System.Windows.Media.Color l, RichTextBox richTextBox1, int selectLength, TextPointer tpStart, TextPointer tpEnd)
        {
            TextRange range = richTextBox1.Selection;
            range.Select(tpStart, tpEnd);
            //高亮选择         

            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(l));
            //range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            return tpEnd.GetNextContextPosition(LogicalDirection.Forward);//将指针移到
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l">设置颜色,color.FromRgb(rgb的值)</param>
        /// <param name="richBox">richBox</param>
        /// <param name="keyword">需要高亮显示的文字</param>
        public void ChangeColor(System.Windows.Media.Color l, RichTextBox richBox, string keyword)
        {
            //设置文字指针为Document初始位置           
            //richBox.Document.FlowDirection           
            TextPointer position = richBox.Document.ContentStart;
            while (position != null)
            {
                //向前搜索,需要内容为Text       
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //拿出Run的Text        
                    string text = position.GetTextInRun(LogicalDirection.Forward);
                    //可能包含多个keyword,做遍历查找           
                    int index = 0;
                    index = text.IndexOf(keyword, 0);
                    if (index != -1)
                    {
                        TextPointer start = position.GetPositionAtOffset(index);
                        TextPointer end = start.GetPositionAtOffset(keyword.Length);
                        position = selecta(l, richBox, keyword.Length, start, end);//找到了keyword的位置就将指针移到高亮显示的文字的末尾作为下一次搜索的开始
                    }

                }
                //文字指针向前偏移   
                position = position.GetNextContextPosition(LogicalDirection.Forward);

            }
        }

        public void ChangeColorOne(System.Windows.Media.Color l, RichTextBox richBox, string keyword)
        {
            //设置文字指针为Document初始位置           
            //richBox.Document.FlowDirection           
            TextPointer position = richBox.Document.ContentStart;
            while (position != null)
            {
                //向前搜索,需要内容为Text       
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //拿出Run的Text        
                    string text = position.GetTextInRun(LogicalDirection.Forward);
                    //可能包含多个keyword,做遍历查找           
                    int index = 0;
                    index = text.IndexOf(keyword, 0);
                    if (index != -1)
                    {
                        TextPointer start = position.GetPositionAtOffset(index);
                        TextPointer end = start.GetPositionAtOffset(keyword.Length);
                        position = selecta(l, richBox, keyword.Length, start, end);//找到了keyword的位置就将指针移到高亮显示的文字的末尾作为下一次搜索的开始
                        return;
                    }

                }
                //文字指针向前偏移   
                position = position.GetNextContextPosition(LogicalDirection.Forward);

            }
        }
    }
}
