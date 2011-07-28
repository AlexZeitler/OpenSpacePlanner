using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenSpacePlanner.Client;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Model;

namespace OpenSpacePlanner
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged, IDragContainer
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Session> UnplannedSessions { get; private set; }
    private readonly List<SessionSlotControl> slots;
    // ReSharper disable NotAccessedField.Local
    private Timer updateTimer;
    // ReSharper restore NotAccessedField.Local

    public Session CurrentSession;

    // ReSharper disable NotAccessedField.Local
    private IDisposable kinectTimer;
    // ReSharper restore NotAccessedField.Local
    private int kinectSync;
    private AverageSumDouble kinectX;
    private AverageSumDouble kinectY;
    private Point kinectPos;
    private int kinectDelay;
    private bool kinectHold;
    private IDragContainer kinectSource;
    private Session kinectSession;
    public bool KinectConnected { get { return kinect.IsAvailable; } }
    private KinectOpenSpace kinect;

    private NosClient webClient;
    public string WebUrl { get; set; }
    public bool WebConnected { get; set; }

    public bool Locked { get; set; }

    public MainWindow()
    {
      InitializeComponent();

      UnplannedSessions = new ObservableCollection<Session>();
      slots = new List<SessionSlotControl>();
      slots.AddRange(Day1.Children.OfType<SessionSlotControl>().Where(slot => !slot.IsPredefined));
      slots.AddRange(Day2.Children.OfType<SessionSlotControl>().Where(slot => !slot.IsPredefined));

      if (string.IsNullOrEmpty(Properties.Settings.Default.ServerUrl))
      {
        var sessionList = new List<Session>
                       {
                         new Session("MVVM_1", "howto", "Frank"),
                         new Session("WCF REST API_1", "RESTful API mit ASP.NET", "Alex"),
                         new Session("Naming Conventions_1", "Coding conventions", "Frank"),
                         new Session("Scrum_1", "Agile development", "Andreas"),
                         new Session("MEF_1", "howto", "Alex"),
                         new Session("csharp_1", "howto", "Golo"),
                         new Session("MVVM", "howto", "Frank") {TimeSlot = "Session 1.1"},
                         new Session("WCF REST API", "RESTful API mit ASP.NET", "Alex"),
                         new Session("Naming Conventions", "Coding conventions", "Frank"),
                         new Session("Scrum", "Agile development", "Andreas"),
                         new Session("MEF", "howto", "Alex"),
                         new Session("csharp", "howto", "Golo")
                       };
        LoadTestSessions(sessionList);
      }

      InitializeKinect();

      DataContext = this;
      if (!kinect.IsAvailable)
      {
        kinectTimer = Observable.Interval(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher().Subscribe(_ => OnKinectTimer());
      }
    }

    private void InitializeKinect()
    {
      kinect = new KinectOpenSpace(new Point(Width, Height));
      kinect.KinectEnter += OnRealKinectEnter;
      kinect.KinectLeave += OnRealKinectLeave;
      kinect.KinectPos += OnRealKinectPos;

      kinectX = new AverageSumDouble(3);
      kinectY = new AverageSumDouble(3);

      kinectSync = Environment.TickCount;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      WebUrl = Properties.Settings.Default.ServerUrl;
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs("WebUrl"));

      webClient = new NosClient(new Uri(WebUrl));
      updateTimer = new Timer(AsyncTimerDoWork, null, 100, 10000);
    }

    void AsyncTimerDoWork(object sender)
    {
      try
      {
        var sessions = webClient.GetAllSessions();
        Dispatcher.Invoke((Action)(() => UpdateSessions(sessions)));
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        WebConnected = false;
        if (PropertyChanged != null)
          PropertyChanged(this, new PropertyChangedEventArgs("WebConnected"));
      }
    }

    private void UpdateSessions(IEnumerable<INosSession> sessionList)
    {
      WebConnected = true;
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs("WebConnected"));

      foreach (var session in sessionList)
      {
        var s = new Session(session);
        var existing = UnplannedSessions.Where(us => us.Id == session.Id).FirstOrDefault();
        var slot = GetSlotByName(s.TimeSlot);

        if (s.IsPlanned)
        {
          // remove from unplanned
          if (existing != null)
          {
            UnplannedSessions.Remove(existing);
          }

          // remove from all other planned slots
          foreach (var otherSlot in slots.Where(os => (os.SlotName != slot.SlotName) && (os.PlannedSession != null) && (os.PlannedSession.Id == s.Id)))
          {
            otherSlot.PlannedSession = null;
          }

          // ensure planned in slot
          if ((slot.PlannedSession == null) || (slot.PlannedSession.Id != s.Id))
          {
            s.Planned += SaveSessionPlanning;
            slot.PlannedSession = s;
          }
        }
        else
        {
          // add to unplanned
          if (existing == null)
          {
            s.Planned += SaveSessionPlanning;
            UnplannedSessions.Add(s);
          }

          // remove from all planned slots
          foreach (var planSlot in slots.Where(os => (os.PlannedSession != null) && (os.PlannedSession.Id == s.Id)))
          {
            planSlot.PlannedSession = null;
          }
        }
      }
    }

    void SaveSessionPlanning(Session session, string timeSlot)
    {
      if (!WebConnected)
        return;
      Dispatcher.Invoke((Action)(() => webClient.UpdateSession(session)));
    }

    // ReSharper disable UnusedMember.Local
    private void LoadTestSessions(IEnumerable<Session> sessionList)
    {
      foreach (var session in sessionList)
      {
        if (session.IsPlanned)
        {
          var slot = GetSlotByName(session.TimeSlot);
          if (slot != null)
            slot.PlannedSession = session;
        }
        else
        {
          UnplannedSessions.Add(session);
        }
      }
    }
    // ReSharper restore UnusedMember.Local

    private SessionSlotControl GetSlotByName(string name)
    {
      return slots.Where(s => s.SlotName == name).FirstOrDefault();
    }

    private void SessionListPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (Locked)
      {
        e.Handled = true;
        return;
      }

      var element = InputHitTest(e.GetPosition(this)) as UIElement;
      if (element != null)
      {
        e.Handled = !element.GetType().Name.StartsWith("Scroll");
      }

      Session session;
      FindSession(e.GetPosition(this), out session);
      if (session == null)
        return;

      BeginDrag(session);
      var item = (ListBoxItem)(SessionList.ItemContainerGenerator.ContainerFromItem(session));
      try
      {
        DragDrop.DoDragDrop(item, new DataObject(session), DragDropEffects.Move);
      }
      catch (COMException)
      {
      }
      EndDrag(session);
      ShowDragSession(null);
    }

    void CurrentSessionPlanned(Session session, string plannedTo)
    {
      if (CurrentSession == null)
        return;

      CurrentSession.Planned -= CurrentSessionPlanned;

      if (string.IsNullOrEmpty(plannedTo))
      {
        UnplannedSessions.Add(CurrentSession);
      }
      else
      {
        UnplannedSessions.Remove(CurrentSession);
      }
      CurrentSession = null;
    }

    private void LabelMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      Day1.Visibility = Day1.IsVisible ? Visibility.Hidden : Visibility.Visible;
      Day2.Visibility = Day2.IsVisible ? Visibility.Hidden : Visibility.Visible;
    }

    private void SessionListDrop(object sender, DragEventArgs e)
    {
      var session = e.Data.GetData(typeof(Session)) as Session;
      if (session == null)
        return;

      EndDrag(session);
      DropHere(session);
      e.Handled = true;
    }

    public void ShowDragSession(Session session)
    {
      KinectHand.Source = session != null ?
        new BitmapImage(new Uri(@"/Images/hand_grab.png", UriKind.RelativeOrAbsolute)) :
        new BitmapImage(new Uri(@"/Images/hand.png", UriKind.RelativeOrAbsolute));
      DragSession.DataContext = session;
      DragSession.Visibility = (session == null) ? Visibility.Hidden : Visibility.Visible;
    }

    private void WindowGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      try
      {
        var cursorpos = System.Windows.Forms.Cursor.Position;
        var mousepos = PointFromScreen(new Point(cursorpos.X, cursorpos.Y));
        DragSession.Margin = new Thickness(mousepos.X - DragSession.Width / 2, mousepos.Y + 1, 0, 0);
      }
      // ReSharper disable EmptyGeneralCatchClause
      catch (Exception) { }
      // ReSharper restore EmptyGeneralCatchClause
    }

    private IDragContainer FindSession(Point pos, out Session session)
    {
      var oldVisibility = KinectHand.Visibility;
      KinectHand.Visibility = Visibility.Hidden;
      IDragContainer target = null;
      session = null;
      var element = InputHitTest(pos) as UIElement;
      while (element != null)
      {
        if (element.GetType() == typeof(SessionSlotControl))
        {
          var ctrl = (SessionSlotControl)element;
          session = ctrl.PlannedSession;
          target = ctrl;
          break;
        }
        if (element.GetType() == typeof(ListBoxItem))
        {
          var ctrl = (ListBoxItem)element;
          session = ctrl.Content as Session;
          target = this;
          break;
        }
        if (element.GetType() == typeof(ListBox))
        {
          target = this;
          break;
        }
        element = VisualTreeHelper.GetParent(element) as UIElement;
      }
      KinectHand.Visibility = oldVisibility;
      return target;
    }

    public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject
    {
      // Search immediate children first (breadth-first)     
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child != null && child is TChildItem)
          return (TChildItem)child;
        var childOfChild = FindVisualChild<TChildItem>(child);
        if (childOfChild != null)
          return childOfChild;
      }
      return null;
    }

    private void ThemesMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (Locked)
      {
        Password.Visibility = Visibility.Visible;
        Password.PreviewKeyDown += PasswordPreviewKeyDown;
        Password.Focus();
        return;
      }

      if (string.IsNullOrEmpty(Properties.Settings.Default.UnlockPassword))
        return;

      Locked = true;
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs("Locked"));
    }

    void PasswordPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter)
        return;

      Password.Visibility = Visibility.Hidden;
      if (Password.Password != Properties.Settings.Default.UnlockPassword)
        return;

      Locked = false;
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs("Locked"));
    }

    #region KinectSimulation using right mouse button


    private void WindowMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      OnKinectEnter();
    }

    private void WindowMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      OnKinectLeave();
    }

    private void WindowMouseMove(object sender, MouseEventArgs e)
    {
      if (!KinectHand.IsVisible || kinect.IsAvailable)
        return;

      MoveKinectHand();
    }

    private void MoveKinectHand()
    {
      Thickness margin;
      if (!kinect.IsAvailable)
      {
        var cursorpos = System.Windows.Forms.Cursor.Position;
        var mousepos = PointFromScreen(new Point(cursorpos.X, cursorpos.Y));
        margin = new Thickness(mousepos.X, mousepos.Y, 0, 0);
      }
      else
      {
        margin = new Thickness(kinectPos.X, kinectPos.Y, 0, 0);
        if (DragSession.IsVisible)
          DragSession.Margin = new Thickness(kinectPos.X - DragSession.Width / 2, kinectPos.Y + 1, 0, 0);
      }
      KinectHand.Margin = margin;
    }

    private void OnKinectTimer()
    {
      if (!KinectHand.IsVisible)
        return;

      var cursorpos = System.Windows.Forms.Cursor.Position;
      var mousepos = PointFromScreen(new Point(cursorpos.X, cursorpos.Y));
      var newPosX = mousepos.X; // - KinectHand.ActualWidth/2;
      var newPosY = mousepos.Y; // - KinectHand.ActualHeight/2;

      OnKinectPos(newPosX, newPosY);
    }

    #endregion

    #region KinectInterface

    private void OnRealKinectPos(object sender, KinectPosEventHandlerArgs args)
    {
      Console.WriteLine("Kinect x={0}, y={1}", args.Point.X, args.Point.Y);
      //kinectPos = args.Point;
      OnKinectPos(args.Point.X, args.Point.Y);
      MoveKinectHand();
    }

    private void OnRealKinectLeave(object sender, KinectLeaveEventHandlerArgs args)
    {
      Console.WriteLine("Kinect Leave");
      OnKinectLeave();
    }

    private void OnRealKinectEnter(object sender, KinectEnterEventHandlerArgs args)
    {
      Console.WriteLine("Kinect Enter");
      OnKinectEnter();
    }

    private void OnKinectEnter()
    {
      if (Locked)
        return;

      KinectHand.Visibility = Visibility.Visible;
      MoveKinectHand();
      kinectPos = new Point(0, 0);
      kinectDelay = 0;
      kinectHold = false;
    }

    private void OnKinectLeave()
    {
      if (Locked)
        return;

      KinectHand.Visibility = Visibility.Hidden;
      kinectDelay = 0;
      kinectHold = false;

      ShowDragSession(null);
      if (kinectSource != null)
        kinectSource.EndDrag(kinectSession);
    }

    private void OnKinectPos(double newPosX, double newPosY)
    {
      if (Locked)
        return;

      if (Environment.TickCount < kinectSync + 200)
        return;

      kinectSync = Environment.TickCount;

      newPosX = kinectX.GetAverageValue(newPosX);
      newPosY = kinectY.GetAverageValue(newPosY);

      var dX = newPosX - kinectPos.X;
      var dY = newPosY - kinectPos.Y;
      var distance = Math.Sqrt((dX * dX) + (dY * dY));

      kinectPos = new Point(newPosX, newPosY);

      if (distance > 10)
      {
        kinectDelay = 0;
      }

      kinectDelay++;
      if (kinectSession == null)
      {
        if (kinectDelay == 3)
        {
          Session session;
          var source = FindSession(kinectPos, out session);
          if ((source != null) && (session != null))
            KinectHand.Source = new BitmapImage(new Uri(@"/Images/hand_grab2.png", UriKind.RelativeOrAbsolute));
        }
        if (kinectDelay == 4)
        {
          Session session;
          var source = FindSession(kinectPos, out session);
          if ((source != null) && (session != null))
            KinectHand.Source = new BitmapImage(new Uri(@"/Images/hand_grab1.png", UriKind.RelativeOrAbsolute));
        }

        var element = InputHitTest(new Point(newPosX, newPosY)) as UIElement;
        if ((element != null) && element.GetType().Name.StartsWith("Scroll"))
        {
          var sv = FindVisualChild<ScrollViewer>(SessionList);
          sv.ScrollToVerticalOffset(sv.VerticalOffset + ((newPosY < ActualHeight / 2) ? -10 : 10));
        }

      }
      if (kinectDelay == 5)
      {
        Console.WriteLine("Kinect *** HOLD ***");
        kinectHold = !kinectHold;
        if (kinectHold)
        {
          Session session;
          kinectSource = FindSession(kinectPos, out session);
          if ((kinectSource == null) || (session == null))
          {
            kinectHold = false;
            kinectDelay = 0;
            return;
          }

          kinectSource.BeginDrag(session);
          ShowDragSession(session);
          kinectSession = session;
        }
        else
        {
          Session session;
          var intf = FindSession(kinectPos, out session);
          if ((intf == null) || !intf.CanDropHere(session))
          {
            kinectHold = true;
            kinectDelay = 0;
            return;
          }

          ShowDragSession(null);
          intf.DropHere(kinectSession);
          kinectSource.EndDrag(kinectSession);
          kinectSession = null;
          kinectSource = null;
        }
        return;
      }
      if (kinectDelay < 5)
      {
        WindowGiveFeedback(this, null);
      }
    }

    #endregion

    #region IDragContainer

    public void BeginDrag(Session session)
    {
      CurrentSession = session;
      CurrentSession.Planned += CurrentSessionPlanned;
      ShowDragSession(session);

      var item = (ListBoxItem)(SessionList.ItemContainerGenerator.ContainerFromItem(session));
      item.Opacity = 0.5;
    }

    public void EndDrag(Session session)
    {
      var item = (ListBoxItem)(SessionList.ItemContainerGenerator.ContainerFromItem(session));
      if (item != null)
        item.Opacity = 1.0;
    }

    public bool CanDropHere(Session session)
    {
      return true;
    }

    public void DropHere(Session session)
    {
      if ((session == null) || (session.TimeSlot == null))
        return;
      session.TimeSlot = null;
      UnplannedSessions.Add(session);
    }

    #endregion

  }
}
