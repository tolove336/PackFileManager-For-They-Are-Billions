using DXVision;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using PackFileManager.ConvertManager;
using PackFileManager.FormManager;
using PackFileManager.IDEManager;
using PackFileManager.PFMCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace PackFileManager
{
    /// <summary>
    /// ModEdit.xaml 的交互逻辑
    /// </summary>
    public partial class ModEdit : Window
    {
        public ModEdit()
        {
            InitializeComponent();
        }

        public void ActiveThis()
        {
            StartListen = true;
        }
        private void CurrentEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Paragraph NParagraph = new Paragraph();
            //NParagraph.Inlines.Add(new Run((sender as TextBox).Text));
            //EditCode.Document.Blocks.Clear();
            //EditCode.Document.Blocks.Add(NParagraph);
        }



        public bool StartListen = false;
        public string CurrentKey = "";
  

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPanel.Install(TextEditor.TextArea);
            TextEditor.TextArea.TextEntered += TextAreaOnTextEntered;

            OffsetColorizer NOffsetColorizer = new OffsetColorizer();
            TextEditor.TextArea.TextView.LineTransformers.Add(NOffsetColorizer);

            string GetName = "PackFileManager" + ".IDERule.Lua.xshd";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream s = assembly.GetManifestResourceStream(GetName))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    var xshd = HighlightingLoader.LoadXshd(reader);
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
                }
            }

            DeFine.ActiveIDE = TextEditor;
        }

     
    

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            StartListen = false;
        }

        private void EditCode_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void EditCode_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

      

      

        private void UsingCode(object sender, RoutedEventArgs e)
        {
          
        }

        private void CheckCode(object sender, RoutedEventArgs e)
        {
           
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = (Label)sender;

                string GetConetnt = ConvertHelper.ObjToStr(LockerLab.Content);

                if (GetConetnt == "TestScript")
                {
                    var Get = CodeCreatHelper.GetTheyAreBillionsEntity(TextEditor.Text);
                    this.Title = DeFine.ModEditTittle + " ->"+ string.Format("以修改或者新建:{0}条目!", CodeCreatHelper.ChangeGameDate(Get));
                }
             
                // CurrentEdit.ScrollToLine();
            }
        }

  
    

  

      

        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            UserInputAction.TextAreaOnTextEntered(e, TextEditor.TextArea);
        }
    }


    public class BraceFoldingStrategy
    {
        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public char OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public char ClosingBrace { get; set; }

        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public BraceFoldingStrategy()
        {
            this.OpeningBrace = '{';
            this.ClosingBrace = '}';
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            char openingBrace = this.OpeningBrace;
            char closingBrace = this.ClosingBrace;
            for (int i = 0; i < document.TextLength; i++)
            {
                char c = document.GetCharAt(i);
                if (c == openingBrace)
                {
                    startOffsets.Push(i);
                }
                else if (c == closingBrace && startOffsets.Count > 0)
                {
                    int startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset)
                    {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }

}
