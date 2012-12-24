using System.ComponentModel;
using System.Xml.Linq;
using LINQPad.Extensibility.DataContext;

namespace NDatabase.LinqPad.Driver
{
    internal class NDatabaseDynamicDriverProperties : INotifyPropertyChanged
    {
        internal const string DbFileSettingName = "DbFilePath";

        private readonly IConnectionInfo _cxInfo;
        private readonly XElement _driverData;

        public NDatabaseDynamicDriverProperties(IConnectionInfo cxInfo)
        {
            _cxInfo = cxInfo;
            _driverData = cxInfo.DriverData;
        }

        public bool Persist
        {
            get { return _cxInfo.Persist; }
            set
            {
                _cxInfo.Persist = value;
                OnPropertyChanged("Persist");
            }
        }

        public string DbFilePath
        {
            get { return (string)_driverData.Element(DbFileSettingName) ?? ""; }
            set
            {
                _driverData.SetElementValue(DbFileSettingName, value);
                OnPropertyChanged("DbFilePath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}