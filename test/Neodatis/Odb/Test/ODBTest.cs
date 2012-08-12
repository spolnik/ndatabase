/*
NDatabase ODB : Native Object Database (odb.info@NDatabase.org)
Copyright (C) 2008 NDatabase Inc. http://www.NDatabase.org

"This file is part of the NDatabase ODB open source object database".

NDatabase ODB is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

NDatabase ODB is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.IO;
using NDatabase.Odb;
using NDatabase.Tool.Wrappers.IO;

namespace Test.Odb.Test
{
    public class ODBTest : NDatabaseAssert
    {
        public static String HOST = "localhost";
        public static int PORT = 10000;
        public static string Directory = ".";
        public static bool useSameVmOptimization;
        public static bool testNewFeature;
        public static bool testPerformance;

        public string GetBaseName()
        {
            var guid = Guid.NewGuid();
            return string.Format("{0}{1}.neodatis", GetName(), guid.ToString());
        }

        public virtual IOdb Open(String fileName, String user, String password)
        {
            return NDatabase.Odb.OdbFactory.Open(fileName);
        }

        public virtual IOdb Open(String fileName)
        {
            return NDatabase.Odb.OdbFactory.Open(fileName);
        }

        public virtual IOdb OpenLocal(String fileName)
        {
            return NDatabase.Odb.OdbFactory.Open(fileName);
        }

        public virtual void failCS()
        {
            AssertTrue(true);
        }

        protected internal virtual void FailNotImplemented(String string_Renamed)
        {
            AssertTrue(true);
        }

        protected internal virtual void DeleteBase(String baseName)
        {
            if (File.Exists(baseName))
                OdbFile.DeleteFile(baseName);
        }

        public void Println(object o)
        {
            Console.WriteLine(o.ToString());
        }
    }
}
