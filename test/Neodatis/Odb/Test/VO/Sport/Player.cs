namespace Test.Odb.Test.VO.Sport
{
	public class Player
	{
		private string name;

		private System.DateTime birthDate;

		private Sport favoriteSport;

		public Player(string name, System.DateTime birthDate, Sport
			 favoriteSport)
		{
			this.name = name;
			this.birthDate = birthDate;
			this.favoriteSport = favoriteSport;
		}

		public virtual System.DateTime GetBirthDate()
		{
			return birthDate;
		}

		public virtual void SetBirthDate(System.DateTime birthDate)
		{
			this.birthDate = birthDate;
		}

		public virtual Sport GetFavoriteSport()
		{
			return favoriteSport;
		}

		public virtual void SetFavoriteSport(Sport favoriteSport
			)
		{
			this.favoriteSport = favoriteSport;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
