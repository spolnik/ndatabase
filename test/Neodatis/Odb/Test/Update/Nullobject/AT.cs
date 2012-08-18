namespace Test.Odb.Test.Update.Nullobject
{
	/// <summary>AT</summary>
	public class AT : Device
	{
		private string ipAddress;

		private int port;

		private string physicalAddress;

		private string name;

		private string type;

		private bool deleted;

		private bool status;

		private Constructor constructor;

		private System.DateTime creationDate;

		private System.DateTime updateDate;

		private User user;

		public override string ToString()
		{
			return "[" + ipAddress + "][" + port + "][" + name + "][" + type + "]";
		}

		public string GetIpAddress()
		{
			return ipAddress;
		}

		public new string GetType()
		{
			return type;
		}

		public string GetName()
		{
			return name;
		}

		public int GetPort()
		{
			return port;
		}

		public Constructor GetConstructor()
		{
			return constructor;
		}

		public System.DateTime GetCreationDate()
		{
			return creationDate;
		}

		public bool GetDeleted()
		{
			return deleted;
		}

		public bool GetStatus()
		{
			return status;
		}

		public System.DateTime GetUpdateDate()
		{
			return updateDate;
		}

		public User GetUser()
		{
			return user;
		}

		public void SetConstructor(Constructor
			 constructor)
		{
			this.constructor = constructor;
		}

		public void SetCreationDate(System.DateTime creationDate)
		{
			this.creationDate = creationDate;
		}

		public void SetDeleted(bool deleted)
		{
			this.deleted = deleted;
		}

		public void SetStatus(bool status)
		{
			this.status = status;
		}

		public void SetUpdateDate(System.DateTime updateDate)
		{
			this.updateDate = updateDate;
		}

		public void SetUser(User user)
		{
			this.user = user;
		}

		public void SetIpAddress(string ipAddress)
		{
			this.ipAddress = ipAddress;
		}

		public void SetType(string type)
		{
			this.type = type;
		}

		public void SetName(string name)
		{
			this.name = name;
		}

		public void SetPort(int port)
		{
			this.port = port;
		}

		public string GetPhysicalAddress()
		{
			return physicalAddress;
		}

		public void SetPhysicalAddress(string physicalAddress)
		{
			this.physicalAddress = physicalAddress;
		}
	}
}
