using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using NDatabase.Studio.Infrastructure;
using NDatabase.Studio.Utils;

namespace NDatabase.Studio.Modules.Connections
{
    public class ConnectionsViewModel : NotifyPropertyChanged
    {
        private readonly IEventAggregator _eventAggregator;
        private List<string> _connections ;
        private string _selectedConnection;

        public ConnectionsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            LoadConnections();
        }

        public List<string> Connections
        {
            get { return _connections; }
            set { SetField(ref _connections, value, () => Connections); }
        }

        /// <summary>
        /// Gets or sets the selected account.
        /// </summary>
        /// <value>The selected account.</value>
        public string SelectedConnection
        {
            get
            {
                return _selectedConnection;
            }
            set
            {
                SetField(ref _selectedConnection, value, () => SelectedConnection);

                _eventAggregator.GetEvent<Events>().Publish(_selectedConnection);
            }
        }

        private void LoadConnections()
        {
        }
    }
}