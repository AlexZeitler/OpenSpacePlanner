namespace OpenSpacePlanner
{

  public class AverageSumDouble
  {
    private int averageCount;
    private double averageSum;

    public const int DefaultCount = 2;

    public AverageSumDouble(int count = DefaultCount)
    {
      averageCount = count;
    }

    public void Reset()
    {
      averageSum = 0;
    }
    public void SetAverageCount(int newCount)
    {
      averageCount = newCount;
      Reset();
    }
    public double GetAverageValue(double newValue)
    {
      averageSum += newValue;
      var retValue = averageSum / averageCount;
      averageSum -= retValue;
      return retValue;
    }

  }
}
