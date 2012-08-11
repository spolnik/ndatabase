namespace NDatabase.Odb.Core
{
    public interface IError
    {
        IError AddParameter(object o);

        IError AddParameter(string s);

        IError AddParameter(int i);

        IError AddParameter(byte i);

        IError AddParameter(long l);
    }
}
