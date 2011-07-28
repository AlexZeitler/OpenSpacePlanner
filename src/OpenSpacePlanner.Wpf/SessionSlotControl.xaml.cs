using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using OpenSpacePlanner.Model;

namespace OpenSpacePlanner
{
  /// <summary>
  /// Interaction logic for SessionSlotControl.xaml
  /// </summary>
  public partial class SessionSlotControl : UserControl, INotifyPropertyChanged, IDragContainer  
  {
    public string SlotId { get; set; }
    public string SlotName { get; set; }

    public string PredefinedUsage { get; set; }
    public bool IsPredefined { get { return PredefinedUsage != null; } }

    private Session plannedSession;
    public Session PlannedSession
    {
      get { return plannedSession; }
      set 
      { 
        plannedSession = value;
        NotifySessionChanged();
      }
    }

    public bool HasSession { get { return PlannedSession != null; } }
    public bool CanBePlanned { get { return !IsPredefined && !HasSession; } }

    public event PropertyChangedEventHandler PropertyChanged;

    public SessionSlotControl()
    {
      InitializeComponent();
      DataContext = this;
    }

    private void ControlDragOver(object sender, DragEventArgs e)
    {
      e.Handled = true;
      var session = e.Data.GetData(typeof(Session));
      if (!CanBePlanned || (session == null))
      {
        e.Effects = DragDropEffects.None;
        return;
      }
      e.Effects = DragDropEffects.Move;
    }

    private void ControlDrop(object sender, DragEventArgs e)
    {
      var session = e.Data.GetData(typeof(Session)) as Session;
      if (!CanBePlanned || (session == null))
        return;

      DropHere(session);
      e.Handled = true;
    }

    private void NotifySessionChanged()
    {
      if (PropertyChanged == null)
        return;
      PropertyChanged(this, new PropertyChangedEventArgs("HasSession"));
      PropertyChanged(this, new PropertyChangedEventArgs("PlannedSession"));
      PropertyChanged(this, new PropertyChangedEventArgs("CanBePlanned"));
    }

    private void GridPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var wnd = (MainWindow)Application.Current.MainWindow;
      if (wnd.Locked)
      {
        e.Handled = true;
        return;
      }

      if (PlannedSession == null)
        return;

      wnd.ShowDragSession(PlannedSession);

      BeginDrag(PlannedSession);
      try
      {
        DragDrop.DoDragDrop(this, new DataObject(PlannedSession), DragDropEffects.Move);
      }
      catch (COMException)
      {
      }
      EndDrag(PlannedSession);
      wnd.ShowDragSession(null);
    }

    void CurrentSessionPlanned(Session session, string plannedTo)
    {
      if (PlannedSession == null)
        return;

      if (plannedTo != SlotName)
      {
        PlannedSession.Planned -= CurrentSessionPlanned;
        PlannedSession = null;
        NotifySessionChanged();
      }
    }

    #region IDragContainer 

    public void BeginDrag(Session session)
    {
      if (PlannedSession == null)
        return;
      PlannedSession.Planned += CurrentSessionPlanned;
      SessionDetails.Opacity = 0.5;
    }

    public void EndDrag(Session session)
    {
      SessionDetails.Opacity = 1.0;
    }

    public bool CanDropHere(Session session)
    {
      return CanBePlanned;
    }

    public void DropHere(Session session)
    {
      PlannedSession = session;
      if (PlannedSession != null)
        PlannedSession.TimeSlot = SlotName;
      NotifySessionChanged();
    }

    #endregion
  }
}
