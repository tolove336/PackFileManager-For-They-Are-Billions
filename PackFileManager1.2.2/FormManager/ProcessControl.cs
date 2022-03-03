

using PackFileManager.FormManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PackFileManager.FormManager
{
    public class ProcessControl
    {
        public static HookWin WorkingForm = null;

        public static void SetWorkingForm(HookWin CurrentWindow)
        {
            WorkingForm = CurrentWindow;
        }

        public static MyProcessBar CreatProcessBarControl(string ParentName, string ControlID, double Width, double Height, int Ellipses)
        {
            if (WorkingForm == null) return null;

            Canvas ProcessControl = new Canvas();
            ProcessControl.Width = Width;
            ProcessControl.Height = Height;
            ProcessControl.Name = ControlID;
            ProcessControl.Background = null;
            ProcessControl.Width = Width;
            ProcessControl.Height = Height;
            ProcessControl.HorizontalAlignment = HorizontalAlignment.Center;
            ProcessControl.VerticalAlignment = VerticalAlignment.Center;

            List<string> AllEllipses = new List<string>();
            for (int i = 0; i < Ellipses; i++)
            {
                Ellipse NEllipse = new Ellipse();
                NEllipse.Width = Height / 2;
                NEllipse.Height = Height / 2;
                NEllipse.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                NEllipse.Name = ProcessControl.Name + "_" + i;
                AllEllipses.Add(NEllipse.Name);
                NEllipse.SetValue(Canvas.LeftProperty, new double());

                ProcessControl.Children.Add(NEllipse);
            }
            Grid ThisGrid = WorkingForm.MainWin as Grid;

            ControlHelper NControlHelper = new ControlHelper();
            NControlHelper.SetControlInForm(ProcessControl, ThisGrid, ParentName);
            if (NControlHelper.SearchFinds > 0)
            {
                return new MyProcessBar(ControlID, AllEllipses);
            }

            return null;
        }




    }

    public class MyProcessBar
    {
        public string ProcessControlName = "";
        public List<string> EllipseNames = new List<string>();

        public Canvas Partent = null;
        public List<Ellipse> AllEllipse = new List<Ellipse>();
        public bool ThisStop = false;
        public Thread CurrentThread = null;
        public MyProcessBar(string ProcessControlName, List<string> EllipseNames)
        {
            this.ProcessControlName = ProcessControlName;
            this.EllipseNames = EllipseNames;
            ControlHelper NControlHelper = new ControlHelper();

            Grid ThisGrid = ProcessControl.WorkingForm.MainWin as Grid;
            NControlHelper.GetControlInForm(ThisGrid, ProcessControlName);
            if (NControlHelper.SearchFinds > 0)
            {
                Partent = (Canvas)NControlHelper.ThisFindControl;
                foreach (var GetItem in Partent.Children)
                {
                    if (GetItem is Ellipse)
                    {
                        Ellipse ThisEllipse = (Ellipse)GetItem;
                        if (EllipseNames.Contains(ThisEllipse.Name))
                        {
                            ThisEllipse.IsEnabled = false;
                            ThisEllipse.Visibility = Visibility.Hidden;
                            AllEllipse.Add(ThisEllipse);
                        }
                    }
                }
            }
        }

        public void Start(int Speed, Color NewColor, int Range = 0, int MaxSleep = 5000, bool NotReturn = false)
        {
            ThisStop = false;
            int CurrentSleep = MaxSleep;
            int SleepOffset = CurrentSleep / Speed;

            double TargetWidth = 0;

            Partent.Dispatcher.Invoke(new Action(() =>
            {
                TargetWidth = Partent.Width;
            }));


            var ThreadWork = new Thread(
            new ThreadStart(
                  () =>
                  {
                  NewStart:

                      Thread.Sleep(SleepOffset * 50);

                      ProcessControl.WorkingForm.Dispatcher.Invoke(new Action(() =>
                      {
                          foreach (var GetItem in AllEllipse)
                          {

                              GetItem.Visibility = Visibility.Visible;

                          }
                      }));


                      double MinOffset = 0;

                      ProcessControl.WorkingForm.Dispatcher.Invoke(new Action(() =>
                      {
                          for (int i = 0; i < AllEllipse.Count; i++)
                          {
                              Thread.Sleep(1);

                              double ThisWidth = 0;


                              AllEllipse[i].BeginInit();

                              AllEllipse[i].Fill = new SolidColorBrush(NewColor);

                              ThisWidth = AllEllipse[i].Width;

                              Canvas.SetLeft(AllEllipse[i], (i * Range) + ThisWidth);

                              AllEllipse[i].EndInit();

                          }
                      }));





                      while (!this.ThisStop)
                      {
                          bool ExitThis = false;
                          int AllSucess = 0;

                          Thread.Sleep(SleepOffset * 3);

                          ProcessControl.WorkingForm.Dispatcher.Invoke(new Action(() =>
                          {
                              for (int i = 0; i < AllEllipse.Count; i++)
                              {
                                  Thread.Sleep(SleepOffset * 2);

                                  int ThisNumber = i + 1;

                                  Ellipse GetEllipse = AllEllipse[i];

                                  double EllipseWidth = 0;
                                  double EllipseOffset = 0;

                                  if (ThisStop) return;


                                  EllipseOffset = Canvas.GetLeft(GetEllipse);


                                  if (MinOffset == 0)
                                  {
                                      MinOffset = EllipseOffset;
                                  }


                                  EllipseWidth = GetEllipse.Width;
                                  string GetName = GetEllipse.Name;


                                  if (ThisStop) return;

                                  if (EllipseOffset < TargetWidth)
                                  {

                                      AllEllipse[i].BeginInit();

                                      Canvas.SetLeft(GetEllipse, Canvas.GetLeft(GetEllipse) + ((ThisNumber + AllSucess) * 2.5));

                                      GetEllipse.EndInit();

                                  }
                                  else
                                  {
                                      AllSucess++;


                                      GetEllipse.Visibility = Visibility.Hidden;


                                      if (AllSucess == AllEllipse.Count)
                                      {
                                          ExitThis = true;
                                      }
                                  }
                                  //AllMoveSucess
                              }
                          }));

                          if (ExitThis)
                          {
                              break;
                          }
                      }

                      if (!this.ThisStop)
                      {
                          if (!NotReturn)
                              goto NewStart;
                      }
                  }));

            ThreadWork.ApartmentState = ApartmentState.STA;
            ThreadWork.IsBackground = false;
            ThreadWork.Start();

            CurrentThread = ThreadWork;


        }
        public void Stop()
        {
            ThisStop = true;
            if (CurrentThread == null == false)
                CurrentThread.Abort();
        }
    }
}
