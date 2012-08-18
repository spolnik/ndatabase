namespace Test.Odb.Test.VO.Inheritance
{
	public class OutdoorPlayer : Player
	{
		private string groundName;

	    public string GroundName
	    {
	        get { return groundName; }
	        set { groundName = value; }
	    }
	}
}
