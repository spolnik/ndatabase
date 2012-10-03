using NDatabase.Odb.Core.Layers.Layer1.Introspector;

namespace NDatabase.UnitTests.Layer1
{
    public class When_we_use_class_introspector : InstanceSpecification<IClassIntrospector>
    {
        protected override IClassIntrospector Create_subject_under_test()
        {
            return new ClassIntrospector();
        }
    }
}