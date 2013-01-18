using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NDatabase.Silverlight.UnitTests;

namespace NDatabase.Silverlight.Client.UnitTests
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            new When_we_use_ndatabase_on_silverlight().It_should_pass_basic_sample();
        }
    }
}
