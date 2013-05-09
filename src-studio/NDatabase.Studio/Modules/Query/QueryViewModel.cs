using Microsoft.Practices.Prism.Events;
using NDatabase.Studio.Utils;

namespace NDatabase.Studio.Modules.Query
{
    public class QueryViewModel : NotifyPropertyChanged
    {
        private string _sourceCode;
        private readonly IEventAggregator _eventAggregator;

        public QueryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public string SourceCode
        {
            get { return _sourceCode; }
            set { SetField(ref _sourceCode, value, () => SourceCode); }
        }
    }
}