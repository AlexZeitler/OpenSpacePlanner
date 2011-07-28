using OpenSpacePlanner.Model;

namespace OpenSpacePlanner
{
  public interface IDragContainer
  {
    void BeginDrag(Session session);
    void EndDrag(Session session);

    bool CanDropHere(Session session);
    void DropHere(Session session);
  }
}
