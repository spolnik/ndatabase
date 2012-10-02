#define USER1

using NDatabase.Odb;
#if USER1
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.Refactor;
using NDatabase.Odb.Main;
using NDatabase.Tool.Wrappers;
#endif
using NUnit.Framework;

namespace NDatabase.UnitTests.Refactoring
{
    public class When_we_use_refactor_menager_to_rename_field_in_class
    {
        private const string RefactoringDbName = "Refactoring.odb";

        
#if USER1
        [Test]
        [Ignore]
        public void Step1()
        {
            OdbFactory.Delete(RefactoringDbName);

            var user = new User {Name = "Jacek", Age = 25};

            using (var odb = OdbFactory.Open(RefactoringDbName))
                odb.Store(user);
        }

        [Test]
        [Ignore]
        public void Step2()
        {
            using (var odb = OdbFactory.Open(RefactoringDbName))
            {
                var refactorManager = new RefactorManager(((OdbAdapter)odb).GetStorageEngine());
                refactorManager.RenameField(typeof(User), "age", "_age");
                refactorManager.RenameField(typeof(User), "name", "_name");
            }
        }
#else
        [Test]
        // Change User implementation
        [Ignore]
        public void Step3()
        {
            using (var odb = OdbFactory.Open(RefactoringDbName))
            {
                var users = odb.GetObjects<User>();
                Assert.That(users, Has.Count.EqualTo(1));

                var first = users.GetFirst();
                Assert.That(first.Name, Is.EqualTo("Jacek"));
                Assert.That(first.Age, Is.EqualTo(25));
            }

            OdbFactory.Delete(RefactoringDbName);
        }
#endif
    }

#if USER1
    public class User
    {
        private int age;
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }
    }
#else
    public class User
    {
        private int _age;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
    }
#endif

}