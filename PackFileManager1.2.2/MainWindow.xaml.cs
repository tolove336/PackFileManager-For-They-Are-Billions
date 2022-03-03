using DXVision;
using PackFileManager.ConvertManager;
using PackFileManager.FileManager;
using PackFileManager.FormManager;
using PackFileManager.PFMCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackFileManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
   
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeFine.StartCheckProcessService(true);
            NotifyIconHelper.Init(this, "PackFileManager" + DeFine.CurrentVersion);
            this.ShowVersion.Content = " Version:" + DeFine.CurrentVersion;
            NotifyIconHelper.ShowMsgInNotifyIcon("Message","StartSucess!",1);
            FormHelper.Initialization(this);
            DeFine.StartGameListenService(true);
            FormHelper.ProcessFile();
          


            DeFine.GameEndActions.Add(new DeFine.GameStateAction(
            new Action<int>((int OnePid)=> 
            {
                if (Directory.Exists(DeFine.GamePath))
                {
                    foreach (var Get in PFMHelper.TableItems)
                    {
                        if (File.Exists(DeFine.GamePath + Get.TableName + ".backup"))
                        {
                            if (File.Exists(DeFine.GamePath + Get.TableName + ".dat"))
                            {
                                File.Delete(DeFine.GamePath + Get.TableName + ".dat");
                            }

                            File.Copy(DeFine.GamePath + Get.TableName + ".backup", DeFine.GamePath + Get.TableName + ".dat");
                        }
                    }
                }
            }
            ),-1));



        }

        private void SelectNav(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = (Label)sender;

                switch (ConvertHelper.ObjToStr(LockerLab.Content))
                {
                    case "GameData":
                        {
                            GameDataView.Visibility = Visibility.Visible;
                            GameResourcesView.Visibility = Visibility.Hidden;
                            FormHelper.ShowDate();
                        }
                        break;
                   case "ModEdit":
                        {
                            GameDataView.Visibility = Visibility.Hidden;
                            GameResourcesView.Visibility = Visibility.Hidden;
                            DeFine.ShowModEdit();
                        }
                        break;
                    case "GameResources":
                        {
                            GameResourcesView.Visibility = Visibility.Visible;
                            GameDataView.Visibility = Visibility.Hidden;
                           
                            ResourcesDeCode.TryConvertResources(DeFine.CurrentSetting.GamePath);
                        }
                        break;
                    case "ShowVersion":
                        {
                            MessageBox.Show(string.Format("Ver:{0},QQ技术交流群欢迎亿万僵尸作者!群号:143878510", "1.2Alpha"));
                        }
                        break;

                }
            
            }
        }

        private void CurrentTagNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                string GetContent = ConvertHelper.ObjToStr((sender as ComboBox).SelectedValue);
                CurrentTag.Text = GetContent;
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FormHelper.ShowDate(true);
        }

        private void EditCode_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void EditCode_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

       

        private void EditCode_SelectionChanged(object sender, RoutedEventArgs e)
        {
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ResourcesClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
