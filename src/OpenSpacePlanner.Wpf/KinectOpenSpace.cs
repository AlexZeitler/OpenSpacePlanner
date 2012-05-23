using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Kinect;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OpenSpacePlanner
{
  public class KinectOpenSpace
  {
    private readonly KinectSensor nui;
    private int player = -1;
    private int inactivityCounter;
    // ReSharper disable NotAccessedField.Local
    private IDisposable inactivityTimer;
    // ReSharper restore NotAccessedField.Local

    public Point Screen { get; set; }
    public Boolean IsAvailable;

    public event KinectEnterEventHandler KinectEnter;
    public event KinectLeaveEventHandler KinectLeave;
    public event KinectPosEventHandler KinectPos;

    private void OnKinectPos(KinectPosEventHandlerArgs kinectPosEventHandlerArgs)
    {
      if ((KinectPos != null) && (IsAvailable))
        KinectPos(this, kinectPosEventHandlerArgs);
    }

    private void OnKinectLeave(KinectLeaveEventHandlerArgs kinectLeaveEventHandlerArgs)
    {
      if ((KinectLeave != null) && (IsAvailable))
        KinectLeave(this, kinectLeaveEventHandlerArgs);
    }

    private void OnKinectEnter(KinectEnterEventHandlerArgs kinectEnterEventHandlerArgs)
    {
      if ((KinectEnter != null) && (IsAvailable))
        KinectEnter(this, kinectEnterEventHandlerArgs);
    }

    public KinectOpenSpace(Point screenSize)
    {
      Screen = screenSize;

      try
      {
        if (KinectSensor.KinectSensors.Count > 0)
        {
          nui = KinectSensor.KinectSensors[0];
          nui.Start();
          nui.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
          nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
          nui.SkeletonFrameReady += NuiSkeletonFrameReady;
          inactivityTimer = Observable.Interval(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher().Subscribe(_ => OnInactivityTimer());
          IsAvailable = true;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Kinect initialization failed. Please make sure Kinect device is plugged in." + Environment.NewLine + ex.Message);
      }
    }

    private void OnInactivityTimer()
    {
      inactivityCounter++;
      if (inactivityCounter < 10)
        return;

      if (player == -1)
        return;

      player = -1;
      OnKinectLeave(new KinectLeaveEventHandlerArgs { Player = player });
    }

    private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
    {
      var skeletonFrame = e.OpenSkeletonFrame();
      var skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo(skeletons);

      inactivityCounter = 0;
      if (skeletons.FirstOrDefault(x => x.TrackingId == player) == null)
      {
        player = -1;
        OnKinectLeave(new KinectLeaveEventHandlerArgs { Player = player });
      }
      foreach (var skeleton in skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked))
      {
        foreach (Joint joint in skeleton.Joints.Where(j => j.JointType == JointType.HandRight))
        {
          // Neuer Spieler gekommen
          if (player < 0)
          {
            player = skeleton.TrackingId;
            OnKinectEnter(new KinectEnterEventHandlerArgs { Player = player });
          }

          // Wenn aktueller Spieler, dann Event auslösen
          if (player != skeleton.TrackingId) 
            continue;

          var screenPos = ReSizeForScreen(new Point(joint.Position.X, joint.Position.Y));
          OnKinectPos(new KinectPosEventHandlerArgs { Player = player, Point = screenPos } );
        }
      }
    }

    private Point ReSizeForScreen(Point jointPos)
    {
      Console.WriteLine("Joint in x={0}, y={1}", jointPos.X, jointPos.Y);

      var x = Math.Min(Math.Max(jointPos.X * 1.5, -1), 1);
      var y = Math.Min(Math.Max(jointPos.Y * 1.5, 0), 1);
      return new Point((x * Screen.X) / 2 + (Screen.X / 2), Screen.Y - (y * Screen.Y));
    }

  }
  // ReSharper restore UnusedMember.Local

  public delegate void KinectPosEventHandler(object sender, KinectPosEventHandlerArgs args);
  public delegate void KinectLeaveEventHandler(object sender, KinectLeaveEventHandlerArgs args);
  public delegate void KinectEnterEventHandler(object sender, KinectEnterEventHandlerArgs args);

  public class KinectPosEventHandlerArgs
  {
    public int Player { get; set; }
    public Point Point { get; set; }
  }

  public class KinectLeaveEventHandlerArgs
  {
    public int Player { get; set; }
  }

  public class KinectEnterEventHandlerArgs
  {
    public int Player { get; set; }
  }
}
