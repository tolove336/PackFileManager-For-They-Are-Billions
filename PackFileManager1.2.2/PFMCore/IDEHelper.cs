using PackFileManager.ConvertManager;
using PackFileManager.FormManager;
using PackFileManager.IDEManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PackFileManager.PFMCore
{
    public class IDEHelper
    {
        public static List<OneColor> CheckLineSign(string Line)
        {
            List<OneColor> OneColors = new List<OneColor>();
            if (Line.IndexOf("<") >= 0 && Line.IndexOf(">") >= 0)
            {
                string GetFunction = Line.Split('<')[0];

                if (GetFunction.Contains(" "))
                {
                    GetFunction = GetFunction.Trim().Substring(GetFunction.Trim().IndexOf(" ") + " ".Length);
                    UserInputAction.CurrentFunction = GetFunction;
                }

                string TableName = ConvertHelper.StringDivision(Line, "<", ">");

                if (TableName.Contains("."))
                {
                    var GetTable = FormHelper.SelectTable(TableName.Split('.')[0], TableName.Split('.')[1]);

                    if (GetTable.Rows.Count > 0)
                    {
                        OneColors.Add(new OneColor(Line.IndexOf("<") + "<".Length, TableName.Length, Color.FromRgb(9, 202, 60)));
                    }
                    else
                    {
                        OneColors.Add(new OneColor(Line.IndexOf("<") + "<".Length, TableName.Length, Color.FromRgb(203, 4, 49)));
                    }

                    if (GetFunction.Trim().Replace(" ", "") == string.Empty == false)
                    {
                        bool FindKey = false;

                        foreach (var Get in (GetFunction + ",").Split(','))
                        {
                            if (Get.Trim().Length > 0)
                            {
                                string ProcessKey = Get.Replace(" ", "").Replace("\r\n", "");

                                foreach (var GetItem in GetTable.Rows)
                                {
                                    if (GetItem.Key.Equals(ProcessKey))
                                    {
                                        FindKey = true;
                                    }
                                }
                            }
                        }

                        if (FindKey)
                        {
                            OneColors.Add(new OneColor(Line.IndexOf(GetFunction), GetFunction.Length, Color.FromRgb(15, 121, 255)));

                        }
                        else
                        {
                            OneColors.Add(new OneColor(Line.IndexOf(GetFunction), GetFunction.Length, Color.FromRgb(203, 4, 49)));
                        }
                    }
                }


            }

            return OneColors;
        }
        public static string GetKeyHelp(string OneKey)
        {
            string AutoType = ";//{Value}";
            List<string> AllValue = new List<string>();

            foreach (var GetCol in DeFine.LastTable.Cols)
            {
                if (GetCol.Key.Equals(OneKey))
                {
                    string GetNextValue = "";

                    foreach (var GetRow in DeFine.LastTable.Rows)
                    {
                        if (DeFine.LastTable.Rows[GetRow.Key][GetCol.Value] == null == false)
                        {
                            GetNextValue = DeFine.LastTable.Rows[GetRow.Key][GetCol.Value];

                            AllValue.Add(GetNextValue);
                        }
                    }
                }

            }

            for (int i = 0; i < AllValue.Count; i++)
            {
                string GetValue = AllValue[i];

                if (GetValue.Trim().Length > 0)
                {
                    if (GetValue == "Y" || GetValue == "N")
                    {
                        AutoType = "N;//{Bool}Y or N," + GetValue;
                    }
                    else
                    if (GetValue.Contains(","))
                    {

                        try
                        {
                            foreach (var Get in GetValue.Split(','))
                            {
                                double.Parse(Get);
                            }


                            AutoType = "0,0;//{Point}";
                        }
                        catch { AutoType = "'';//{String}".Replace("'", "\""); }
                    }
                    else
                    if (ConvertHelper.ObjToInt(GetValue) > 0)
                    {
                        AutoType = "0;//{Int}";
                    }
                    else
                    if (GetValue.Length > 0)
                    {
                        AutoType = "'';//{String}".Replace("'", "\"");
                    }

                    return " " + AutoType;
                }
            }

            return string.Empty;
        }
    }

    public class AnyText
    {
        public int StartOffset = 0;
        public int Length = 0;
    }

    public class OneColor : AnyText
    {
        public Color FontColor;

        public OneColor(int StartOffset, int Length, Color FontColor)
        {
            this.StartOffset = StartOffset;
            this.Length = Length;
            this.FontColor = FontColor;
        }
    }

}
