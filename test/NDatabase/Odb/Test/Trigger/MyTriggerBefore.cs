using NDatabase.Odb;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Trigger;

namespace Test.NDatabase.Odb.Test.Trigger
{
    public class MyTriggerBefore : InsertTrigger
    {
        public override void AfterInsert(object @object, OID oid)
        {
        }

        public override bool BeforeInsert(object @object)
        {
            var so = (SimpleObject) @object;
            // just add 1
            so.SetId(so.GetId() + 1);
            return true;
        }
    }
}
