using NDatabase2.Odb.Core;
using NDatabase2.Odb.Core.Layers.Layer3;

namespace NDatabase2.Odb.Main
{
    internal sealed class OdbForTrigger : OdbAdapter
    {
        public OdbForTrigger(IStorageEngine storageEngine) : base(storageEngine)
        {
        }

        public override ITriggerManager TriggerManagerFor<T>()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Close()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Commit()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void DefragmentTo(string newFileName)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Disconnect<T>(T @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override IIndexManager IndexManagerFor<T>()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override IRefactorManager GetRefactorManager()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Rollback()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }
    }
}
