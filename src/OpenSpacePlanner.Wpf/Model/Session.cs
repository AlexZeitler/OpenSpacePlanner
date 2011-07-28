using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Model
{
  public class Session : INosSession
  {
    public Guid Id { get; set; }
    public string Tag { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    private string timeSlot;
    public string TimeSlot
    {
      get { return timeSlot; }
      set
      {
        timeSlot = value;

        Debug.WriteLine("Session {0} planned for timeslot {1}", Title, timeSlot);

        if (Planned != null)
          Planned(this, timeSlot);
      }
    }

    public bool IsPlanned { get { return !string.IsNullOrEmpty(TimeSlot); } }

    public string Owner { get; set; }

    public string OwnerTag { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Room
    {
      get { return timeSlot; }
      set { timeSlot = value; }
    }

    public List<string> Attendees { get; set; }

    public Session()
    {
#if DEBUG
      Title = "REST";
      Description = "Laber bla und immer noch so weiter"; //und immer noch so 
      Owner = "Alex";
#endif
      Attendees = new List<string>();
    }

    public Session(INosSession session)
    {
      Id = session.Id;
      Title = session.Title;
      Description = session.Description;
      Owner = session.Owner;
      timeSlot = session.Room;

      Tag = session.Tag;
      Start = session.Start;
      End = session.End;
      OwnerTag = session.OwnerTag;
      CreatedOn = session.CreatedOn;
    }

    public Session(string title, string description, string owner)
    {
      Title = title;
      Description = description;
      Owner = owner;
      Attendees = new List<string>();
    }

    public event Action<Session, string> Planned;

  }
}
