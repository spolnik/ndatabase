using NDatabase2.Odb.Core;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Trigger;

namespace NDatabase2.Odb.Main
{
    internal sealed class OdbForTrigger : OdbAdapter
    {
        public OdbForTrigger(IStorageEngine storageEngine) : base(storageEngine)
        {
        }

        public void AddDeleteTrigger(DeleteTrigger trigger)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public void AddInsertTrigger(InsertTrigger trigger)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public void AddSelectTrigger(SelectTrigger trigger)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public void AddUpdateTrigger(UpdateTrigger trigger)
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
