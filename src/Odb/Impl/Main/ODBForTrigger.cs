using System;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace NDatabase.Odb.Impl.Main
{
    public sealed class OdbForTrigger : OdbAdapter
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

        public override void CommitAndClose()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void DefragmentTo(string newFileName)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Disconnect(object @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override IClassRepresentation GetClassRepresentation(Type clazz)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override IClassRepresentation GetClassRepresentation(string fullClassName)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override IRefactorManager GetRefactorManager()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override ISession GetSession()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Reconnect(object @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Rollback()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }

        public override void Run()
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotAllowedInTrigger);
        }
    }
}
