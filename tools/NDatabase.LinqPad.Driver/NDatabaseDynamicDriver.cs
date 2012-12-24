using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LINQPad.Extensibility.DataContext;

namespace NDatabase.LinqPad.Driver
{
    public class NDatabaseDynamicDriver : DynamicDataContextDriver
    {
        public override string GetConnectionDescription(IConnectionInfo cxInfo)
        {
            return Path.GetFileName(new NDatabaseDynamicDriverProperties(cxInfo).DbFilePath);
        }

        public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo cxInfo)
        {
            // We need to pass the chosen db file into the DataServiceContext's constructor:
            return new[] {new ParameterDescriptor(NDatabaseDynamicDriverProperties.DbFileSettingName, "System.String")};
        }

        public override object[] GetContextConstructorArguments(IConnectionInfo cxInfo)
        {
            // We need to pass the chosen db file into the DataServiceContext's constructor:
            return new object[] {new NDatabaseDynamicDriverProperties(cxInfo).DbFilePath};
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
        {
            // We need the following assembly for compiliation and autocompletion:
            return new[] { "NDatabase2.dll", "Mono.Reflection.dll" };
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
        {
            // Import the commonly used namespaces as a courtesy to the user:
            return new[] { "NDatabase2.Odb" };
        }

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
        {
            // Populate the default URI with a demo value:
            if (isNewConnection) 
                new NDatabaseDynamicDriverProperties(cxInfo).DbFilePath = "";

            var result = new ConnectionDialog(cxInfo).ShowDialog();
            return result == true;
        }

        public override string Name
        {
            get { return "NDatabase 3.0 - Object Database"; }
        }

        public override string Author
        {
            get { return "NDatabase (http://ndatabase.codeplex.com)"; }
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            return SchemaBuilder.GetSchemaAndBuildAssembly(
                new NDatabaseDynamicDriverProperties(cxInfo),
                assemblyToBuild,
                ref nameSpace,
                ref typeName);
        }

        public override bool AreRepositoriesEquivalent(IConnectionInfo c1, IConnectionInfo c2)
        {
            return Equals(c1.DriverData.Element(NDatabaseDynamicDriverProperties.DbFileSettingName),
                          c2.DriverData.Element(NDatabaseDynamicDriverProperties.DbFileSettingName));
        }
    }
}
