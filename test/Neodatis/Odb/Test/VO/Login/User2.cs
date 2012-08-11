namespace Test.Odb.Test.VO.Login
{
	public class User2 : User
	{
		private int nbLogins;

		public User2(int nbLogins) : base()
		{
			this.nbLogins = nbLogins;
		}

		public User2() : base()
		{
		}

		public User2(string name, string email, Profile profile
			, int nbLogins) : base(name, email, profile)
		{
			this.nbLogins = nbLogins;
		}
	}
}
