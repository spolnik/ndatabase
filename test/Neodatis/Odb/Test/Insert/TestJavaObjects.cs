using System;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Test.Odb.Test;

namespace Insert
{
    [TestFixture]
    public class TestJavaObjects : ODBTest
    {
        #region Setup/Teardown

        [TearDown]
        public override void TearDown()
        {
            DeleteBase(Name);
        }

        [SetUp]
        public override void SetUp()
        {
            try
            {
                DeleteBase(Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public static readonly string Name = "test.neodatis";

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test22StrignBuffer()
        {
            var buffer = new StringBuilder("Ol√° chico");
            var odb = Open(Name);
            odb.Store(buffer);
            odb.Close();
            odb = Open(Name);
            var l = odb.GetObjects<StringBuilder>();
            odb.Close();
            var b2 = l.GetFirst();
            AssertEquals(buffer.ToString(), b2.ToString());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test22TextBox()
        {
            var textBox = new TextBox();
            textBox.Text = "Ol\u00E1 chico";

            var odb = Open(Name);
            odb.Store(textBox);
            odb.Close();
            odb = Open(Name);
            var l = odb.GetObjects<TextBox>();
            odb.Close();
            var textBox2 = l.GetFirst();
            AssertEquals(textBox.Text, textBox2.Text);
        }

        /// <summary>
        ///   This nunit does not work because of a problem? in URL: The hashcode of
        ///   the 2 urls url1 & url2 are equal! even if theu point to different domains
        ///   (having the same IP)
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test22URL()
        {
            var url1 = new Uri("http://wiki.neodatis.org");
            var url2 = new Uri("http://www.neodatis.org");

            var h1 = url1.GetHashCode();
            var h2 = url2.GetHashCode();
            Println(h1 + " - " + h2);
            Println(url1.Host + " - " + url1.Port);
            Println(url2.Host + " - " + url2.Port);

            var odb = Open(Name);
            odb.Store(url1);
            odb.Store(url2);
            odb.Close();
            odb = Open(Name);
            var l = odb.GetObjects<Uri>();
            odb.Close();

            AssertEquals("Same HashCode Problem", 2, l.Count);
        }
    }
}
