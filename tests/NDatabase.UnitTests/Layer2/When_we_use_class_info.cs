using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace NDatabase.UnitTests.Layer2
{
    internal class When_we_use_class_info
    {
        private class Country
        {
            private readonly string _name;

            public Country(string name, int population, string continent)
            {
                _name = name;
                Population = population;
                Continent = continent;
            }

            public string Name
            {
                get { return _name; }
            }

            public int Population { get; set; }

            public virtual string Continent { get; private set; }
        }

        [Test]
        public void It_should_have_set_proper_base_settings()
        {
            var classInfo = new ClassInfo(typeof (Country));
            
            Assert.That(classInfo.ClassCategory, Is.EqualTo(ClassInfo.CategoryUserClass));

            Assert.That(classInfo.UnderlyingType, Is.EqualTo(typeof(Country)));
            Assert.That(classInfo.FullClassName, Is.EqualTo(OdbClassUtil.GetFullName(typeof(Country))));

            Assert.That(classInfo.Position, Is.EqualTo(-1));
            Assert.That(classInfo.MaxAttributeId, Is.EqualTo(-1));
            Assert.That(classInfo.ClassInfoId, Is.Null);
        }

        [Test]
        public void It_should_have_meaningful_default_string_representation()
        {
            var classInfo = new ClassInfo(typeof(Country));

            Assert.That(classInfo.ToString(),
                        Is.StringEnding(
                            "- id= - previousClass= - nextClass= - attributes=(not yet defined) ]"));
        }
    }
}