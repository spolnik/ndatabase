using System;
using System.Collections;
using NDatabase.Odb;
using NDatabase2.Odb;
using NDatabase2.Odb.Main;
using NDatabase2.Tool.Wrappers.Map;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    [TestFixture]
    public class TestQuerySchool : ODBTest
    {
        // possiveis consultas
        // Listar todos os alunos de determinado professor
        // Listar alunos com nota abaixo de x
        // Listar disciplinas que um professor ministrou no semestre

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase("t-school.neodatis");
            var odb = Open("t-school.neodatis");
            var students = odb.GetObjects<Student>(true);
            var numStudents = students.Count;
            var computerScience = new Course("Computer Science");
            var teacher = new Teacher("Jeremias", "Java");
            var dw1 = new Discipline("Des. Web 1", 3);
            var @is = new Discipline("Intranet/Seguran√ßa", 4);
            var std1 = new Student(20, computerScience, new DateTime(), "1cs", "Brenna");
            var h1 = new History(new DateTime(), dw1, 0, teacher);
            var h2 = new History(new DateTime(), @is, 0, teacher);
            std1.AddHistory(h1);
            std1.AddHistory(h2);
            odb.Store(std1);
            odb.Commit();
            odb.Close();
            odb = Open("t-school.neodatis");
            students = odb.GetObjects<Student>(true);
            odb.Close();
            AssertEquals(numStudents + 1, students.Count);
        }

        #endregion

        /// <exception cref="System.Exception"></exception>
        public override void TearDown()
        {
            DeleteBase("t-school.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var odb = Open("t-school.neodatis");
            // List students by name
            var natQuery = new SchoolNativeQueryStudent("Brenna", 23);
            var students = odb.GetObjects<Student>(natQuery);
            var sNatQuery = new SchoolSimpleNativeQueryStudent("Brenna");
            students = odb.GetObjects<Student>(sNatQuery);
            // list disciplines of one teacher by semester
            var natQuery2 = new SchoolNativeQueryTeacher("Jeremias");
            var historys = odb.GetObjects<History>(natQuery2);
            var listDiscipline = new OdbHashMap<string, Discipline>();
            for (IEnumerator iter = historys.GetEnumerator(); iter.MoveNext();)
            {
                var h = (History) iter.Current;
                listDiscipline.Add(h.GetDiscipline().GetName(), h.GetDiscipline());
            }
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test12()
        {
            IOdb odb = null;
            try
            {
                odb = Open("t-school.neodatis");
                var ci = ((OdbAdapter)odb).GetStorageEngine().GetSession(true).GetMetaModel().GetClassInfo(typeof(Student), true);

                AssertFalse(ci.HasCyclicReference());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }
    }
}
