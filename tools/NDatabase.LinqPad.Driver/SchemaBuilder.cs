using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Reflection;
using LINQPad.Extensibility.DataContext;
using Microsoft.CSharp;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.LinqPad.Driver
{
    internal class SchemaBuilder
    {
        private static readonly PluralizationService PluralizationService =
            PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        public static List<ExplorerItem> GetSchemaAndBuildAssembly(NDatabaseDynamicDriverProperties properties,
                                                                   AssemblyName assemblyToBuild, ref string nameSpace,
                                                                   ref string typeName)
        {
            using (var odb = OdbFactory.Open(properties.DbFilePath))
            {
                var metaModel = odb.Ext().GetSchema();
                BuildAssembly(GenerateCode(metaModel, nameSpace, properties.DbFilePath), assemblyToBuild);
                return GetSchema(properties, metaModel, out typeName);
            }
        }

        internal static List<ExplorerItem> GetSchema(NDatabaseDynamicDriverProperties props, IMetaModel metaModel,
                                                     out string typeName)
        {
            typeName = "NDbContext";
            var list = new List<ExplorerItem>();
            foreach (var schemaClass in metaModel.GetSchemaClasses())
            {
                var name = PluralizationService.Pluralize(schemaClass.UnderlyingType.Name);
                
                var item = new ExplorerItem(name, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                               {
                                   IsEnumerable = true,
                                   ToolTipText = "",
                                   Tag = schemaClass.UnderlyingType.FullName
                               };

                list.Add(item);

                var list2 = new List<ExplorerItem>();

                var attributeNames = schemaClass.GetAttributeNames();
                var attributeTypes = schemaClass.GetAttributeTypes();

                for (var i = 0; i < attributeNames.Length; i++)
                {
                    var field = attributeNames[i];
                    var type = attributeTypes[i];
                    var item4 = new ExplorerItem(string.Format("{0} ({1})", field, type), ExplorerItemKind.Property,
                                                 ExplorerIcon.Column);
                    list2.Add(item4);
                }

                item.Children = list2;
            }

            return list;
        }

        private static void BuildAssembly(string generatedCode, AssemblyName assemblyToBuild)
        {
            CompilerResults results;
            var location = Assembly.GetExecutingAssembly().Location;
            var length = location.LastIndexOf('\\');
            location = Path.Combine(location.Substring(0, length), "NDatabase2.dll");
            var providerOptions = new Dictionary<string, string> {{"CompilerVersion", "v4.0"}};
            using (var provider = new CSharpCodeProvider(providerOptions))
            {
                var options =
                    new CompilerParameters(
                        string.Format("System.dll|System.Core.dll|{0}", location).Split("|".ToCharArray()),
                        assemblyToBuild.CodeBase, true);
                results = provider.CompileAssemblyFromSource(options, new[] {generatedCode});
            }
            if (results.Errors.Count > 0)
            {
                throw new Exception(
                    string.Concat(new object[]
                                      {
                                          "Cannot compile typed context: ", results.Errors[0].ErrorText, " (line ",
                                          results.Errors[0].Line, ")"
                                      }));
            }
        }

        private static string GenerateCode(IMetaModel metaModel, string nameSpace, string dbFilePath)
        {
            var writer = new StringWriter();
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("namespace " + nameSpace);
            writer.WriteLine('{');

            foreach (var schemaClass in metaModel.GetSchemaClasses())
            {
                writer.WriteLine(string.Format("public class {0} {{",
                                               schemaClass.UnderlyingType.Name));
                foreach (var field in schemaClass.GetAttributeNames())
                {
                    writer.WriteLine(string.Format("  public string {0} {{ get; set; }}", field));
                }
                writer.WriteLine('}');
                writer.WriteLine();
            }
            writer.WriteLine();
            writer.WriteLine("public class NDbContext {");
            writer.WriteLine();
            writer.WriteLine("string _dbPath;");
            writer.WriteLine();
            writer.WriteLine("public NDbContext( string dbPath )");
            writer.WriteLine('{');
            writer.WriteLine("_dbPath = dbPath;");
            writer.WriteLine('}');
            writer.WriteLine();
            
            foreach (var schemaClass in metaModel.GetSchemaClasses())
            {
                var pluralizedName = PluralizationService.Pluralize(schemaClass.UnderlyingType.Name);
                writer.WriteLine(string.Format("public IList<{0}> {1}", schemaClass.UnderlyingType.Name,
                                               pluralizedName));
                writer.WriteLine('{');
                writer.WriteLine("  get");
                writer.WriteLine("  {");
                writer.WriteLine(string.Format("    IList<{0}> _{1};", schemaClass.UnderlyingType.Name,
                                               pluralizedName));
                writer.WriteLine("    var odb = NDatabase2.Odb.OdbFactory.Open(@\"{0}\");", dbFilePath);
                writer.WriteLine("    try");
                writer.WriteLine("    {");
                writer.WriteLine(string.Format("      _{0} = odb.Query<{1}>().Execute<{1}>().ToList();", pluralizedName,
                                               schemaClass.UnderlyingType.Name));
                writer.WriteLine("    }");
                writer.WriteLine("    finally");
                writer.WriteLine("    {");
                writer.WriteLine("        odb.Close();");
                writer.WriteLine("    }");
                writer.WriteLine(string.Format("    return _{0};", pluralizedName));
                writer.WriteLine("  }");
                writer.WriteLine('}');
                writer.WriteLine();
            }
            writer.WriteLine('}');
            writer.WriteLine('}');
            return writer.ToString();
        }
    }
}