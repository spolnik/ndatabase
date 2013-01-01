﻿using NDatabase.Odb;
using NUnit.Framework;

namespace NDatabase.Client.UnitTests.Misc
{
    public class TestCase_OdbConfiguration
    {
        [Test]
        public void Check_turning_on_the_logging()
        {
            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.False);

            OdbConfiguration.EnableLogging();

            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.True);

            OdbConfiguration.DisableLogging();

            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.False);
        }

        [Test]
        public void Check_changing_btree_degree()
        {
            Assert.That(OdbConfiguration.GetIndexBTreeDegree(), Is.EqualTo(OdbConfiguration.DefaultIndexBTreeDegree));
            
            const int newIndexBTreeSize = 30;
            OdbConfiguration.SetIndexBTreeDegree(newIndexBTreeSize);

            Assert.That(OdbConfiguration.GetIndexBTreeDegree(), Is.EqualTo(newIndexBTreeSize));
        }

        [Test]
        public void Check_turning_on_the_btree_validation()
        {
            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.False);

            OdbConfiguration.EnableBTreeValidation();

            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.True);

            OdbConfiguration.DisableBTreeValidation();

            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.False);
        }

        [Test]
        public void Check_changing_max_number_of_write_actions_per_transaction()
        {
            Assert.That(OdbConfiguration.GetMaxNumberOfWriteObjectPerTransaction(),
                        Is.EqualTo(OdbConfiguration.DefaultMaxNumberOfWriteObjectPerTransaction));

            const int newValue = 30000;
            OdbConfiguration.SetMaxNumberOfWriteObjectPerTransaction(newValue);

            Assert.That(OdbConfiguration.GetMaxNumberOfWriteObjectPerTransaction(), Is.EqualTo(newValue));
        }
    }
}