using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Values;
using NDatabase2.Odb;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    
    public class TestCustomQueryFieldAction2 : CustomQueryFieldAction
    {
        /// <summary>
        ///   The number of logins
        /// </summary>
        private long nbLoggedUsers;

        public TestCustomQueryFieldAction2()
        {
            nbLoggedUsers = 0;
        }

        /// <summary>
        ///   The method that actually computes the logins
        /// </summary>
        public override void Execute(OID oid, AttributeValuesMap values)
        {
            // Gets the name of the user
            var userName = (string) values["name"];
            // Call an external class (Users) to check if the user is logged in
            if (Sessions.IsLogged(userName))
                nbLoggedUsers++;
        }

        public override object GetValue()
        {
            return nbLoggedUsers;
        }

        public override bool IsMultiRow()
        {
            return false;
        }

        public override void Start()
        {
        }

        // Nothing to do
        public override void End()
        {
        }
    }

    internal class Sessions
    {
        public static bool IsLogged(string userName)
        {
            return false;
        }
    }
}
