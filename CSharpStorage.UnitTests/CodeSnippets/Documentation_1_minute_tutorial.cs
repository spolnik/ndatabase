using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers.IO;
using NDatabase.UnitTests.CodeSnippets.Data;
using NUnit.Framework;

namespace NDatabase.UnitTests.CodeSnippets
{
    public class Documentation_1_minute_tutorial
    {
        [SetUp]
        public void SetUp()
        {
            const string dbFileName = "game.db";
            OdbFile.DeleteFile(dbFileName);
        }

        [Test]
        public void Create_two_heroes_store_them_and_then_retrive()
        {
            const string dbFileName = "game.db";

            const double mageAttackValue = 3.3;
            const double mageDefenseValue = 3.4;
            
            const double warriorAttackValue = 4.4;
            const double warriorDefenseValue = 2.2;

            // create two objects
            IHero mage = new Mage("Merlin", mageAttackValue, mageDefenseValue);
            IHero warrior = new Warrior("Conan", warriorAttackValue, warriorDefenseValue);

            // store them
            using (var odb = OdbFactory.Open(dbFileName))
            {
                odb.Store(mage);
                odb.Store(warrior);
            }

            // retrieve them by classes and by interface
            using (var odb = OdbFactory.Open(dbFileName))
            {
                // work with mages
                var mages = odb.GetObjects<Mage>();
                foreach (var hero in mages)
                    Console.WriteLine(hero);

                Assert.That(mages, Has.Count.EqualTo(1));
                Assert.That(mages.GetFirst().Attack, Is.EqualTo(mageAttackValue));
                Assert.That(mages.GetFirst().Defense, Is.EqualTo(mageDefenseValue));

                // work with warriors
                var warriors = odb.GetObjects<Warrior>();
                foreach (var hero in warriors)
                    Console.WriteLine(hero);

                Assert.That(warriors, Has.Count.EqualTo(1));
                Assert.That(warriors.GetFirst().Attack, Is.EqualTo(warriorAttackValue));
                Assert.That(warriors.GetFirst().Defense, Is.EqualTo(warriorDefenseValue));

                Console.WriteLine("Start working with IHero interface.");

                // work with heroes
                var query = CriteriaQuery.New<IHero>().SetPolymorphic(true);

                var heroes = odb.GetObjects<IHero>(query);
                foreach (var hero in heroes)
                    Console.WriteLine(hero);

                Assert.That(heroes, Has.Count.EqualTo(2));
            }
        }
    }
}