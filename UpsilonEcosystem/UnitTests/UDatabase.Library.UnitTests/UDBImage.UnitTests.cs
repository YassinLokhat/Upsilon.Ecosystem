using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using UDatabase.Library;
using System.IO;
using System.Linq;
using System.Threading;

namespace UDatabase.Library.UnitTests
{
    [TestClass]
    public class UDatabaseImage_UnitTests
    {
        [TestMethod]
        public void Test_00_UDatabaseImage_LoadDatabase_KeyEmty_OK()
        {
            // Given
            string reference = "202102270623";
            string sourceFilePath = Common.UnitTestsHelper.Helper.GetDatabaseFilePath(reference);
            string databaseFilePath = sourceFilePath.Replace(reference, reference + "_tmp");
            string beforeLoad = File.ReadAllText(sourceFilePath);

            // When
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, string.Empty);
            database.Sync();
            string afterSync = File.ReadAllText(databaseFilePath);
            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);

            // Then
            afterSync.Should().Be(beforeLoad);
        }

        [TestMethod]
        public void Test_01_UDatabaseImage_LoadDatabase_OptionalAttributMissing()
        {
            // Given
            string reference = "202102270623";
            string sourceFilePath = Common.UnitTestsHelper.Helper.GetDatabaseFilePath(reference);
            string withAllOptionalAttribute = File.ReadAllText(sourceFilePath);

            reference = "202102270624";
            sourceFilePath = Common.UnitTestsHelper.Helper.GetDatabaseFilePath(reference);
            string databaseFilePath = sourceFilePath.Replace(reference, reference + "_tmp");

            // When
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, string.Empty);
            database.Sync();
            string afterSync = File.ReadAllText(databaseFilePath);
            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);

            // Then
            afterSync.Should().Be(withAllOptionalAttribute);
        }

        [TestMethod]
        public void Test_02_UDatabaseImage_LoadDatabase_KeyNotEmty_OK()
        {
            // Given
            string reference = "202103010924";

            // When
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // Then
            database.Tables.Count.Should().Be(2);
            database.Tables.ContainsKey("STUDENT").Should().BeTrue();
            database.Tables["STUDENT"].Name.Should().Be("STUDENT");
            database.Tables["STUDENT"].Fields.Count.Should().Be(4);
            database.Tables["STUDENT"].Fields[0].Name.Should().Be("STUDENT_ID");
            database.Tables["STUDENT"].Fields[1].Name.Should().Be("Name");
            database.Tables["STUDENT"].Fields[2].Name.Should().Be("BirthDate");
            database.Tables["STUDENT"].Fields[3].Name.Should().Be("Photo");
            database.Tables["STUDENT"].Records.Count.Should().Be(3);
            database.Tables["STUDENT"].Records[0][database.Tables["STUDENT"].Fields[0]].Should().Be(1);
            database.Tables["STUDENT"].Records[0][database.Tables["STUDENT"].Fields[1]].Should().Be("S1");
            database.Tables["STUDENT"].Records[0][database.Tables["STUDENT"].Fields[2]].Should().Be(new DateTime(637500041711851137));
            ((short[])database.Tables["STUDENT"].Records[0][database.Tables["STUDENT"].Fields[3]]).Should().BeEquivalentTo("Photo1".Select(x => (short)x).ToArray());

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_03_UDatabaseImage_LoadDatabase_KeyCorrupted()
        {
            // Given
            string reference = "202103010924";
            string key = Common.UnitTestsHelper.Helper.GetRandomString();

            // When
            Action act = () =>
            {
                UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, key);
            };

            // Then
            act.Should().Throw<Exception>().WithMessage("Database Xml error : Wrong key");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_04_UDatabaseImage_LoadDatabase_HashCorrupted()
        {
            // Given
            string reference = "202103010925";
            string key = string.Empty;

            // When
            Action act = () =>
            {
                UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, key);
            };

            // Then
            act.Should().Throw<Exception>();

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_05_UDatabaseImage_AddTable_AddField_AddRecord_OK()
        {
            // Given
            string reference = "202103010924";

            // When
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);
            database.Pull();

            Table professor = database.AddTable("PROFESSOR");
            professor.AddField("PROFESSOR_ID", FieldType.Integer, 1, true);
            professor.AddField("Name", FieldType.String, "P");
            professor.AddRecord(null, "P1");
            professor.AddRecord(null, null);

            Table course = database.Tables["COURSE"];
            Field professorIdField = course.AddField("PROFESSOR_ID", FieldType.Integer, 0);
            foreach (Record record in course.Records)
            {
                record[professorIdField] = 1;
            }

            database.Push();

            database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference, false);
            professor = database.Tables["PROFESSOR"];

            // Then
            database.Tables.Count.Should().Be(3);
            database.Tables.Keys.Should().Equal(new string[] { "STUDENT", "COURSE", "PROFESSOR" });
            professor.Fields.Count.Should().Be(2);
            professor.Records.Count.Should().Be(2);
            professor.Records[0][professor.Fields[0]].Should().BeOfType(typeof(Int64)).And.Be(1);
            professor.Records[0][professor.Fields[1]].Should().BeOfType(typeof(string)).And.Be("P1");
            professor.Records[1][professor.Fields[0]].Should().BeOfType(typeof(Int64)).And.Be(2);
            professor.Records[1][professor.Fields[1]].Should().BeOfType(typeof(string)).And.Be("P");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_06_UDatabaseImage_AddTable_TableNameNotValid()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table professor = database.AddTable("PROFE?SSOR");
            };

            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Table definition not valid.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_07_UDatabaseImage_AddTable_TableAlreadyExists()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table professor = database.AddTable("STUDENT");
            };
            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Table 'STUDENT' already exists.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_08_UDatabaseImage_AddTable_NoField()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table professor = database.AddTable("PROFESSOR");

                database.Push();
            };
            database.Close();

            // Then
            act.Should().Throw<Exception>().WithMessage("Table definition not valid.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_09_UDatabaseImage_AddField_FieldNameNotValid()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table course = database.Tables["COURSE"];
                Field professorIdField = course.AddField("PROFESSOR ID", FieldType.Integer, 0);
            };

            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Field definition not valid.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_10_UDatabaseImage_AddField_FieldAlreadyExists()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table course = database.Tables["COURSE"];
                Field professorIdField = course.AddField("COURSE_ID", FieldType.Integer, 0);
            };
            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Field 'COURSE_ID' already exists.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_11_UDatabaseImage_AddRecord_WrongFieldNumber()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table course = database.Tables["COURSE"];
                course.AddRecord();
            };
            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Wrong record field count. 4 fields expected.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_12_UDatabaseImage_AddRecord_WrongFieldType()
        {
            // Given
            string reference = "202103010924";
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);

            // When
            database.Pull();

            Action act = () =>
            {
                Table course = database.Tables["COURSE"];
                course.AddRecord(7, "1", "Sport", 0.0);
            };
            database.Push();

            // Then
            act.Should().Throw<Exception>().WithMessage("Wrong record field type. STUDENT_ID expect Integer type.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_13_UDatabaseImage_AddRecord_AutoIncrement_OK()
        {
            // Given
            string reference = "202103010924";

            // When
            UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);
            database.Pull();

            Table student = database.Tables["STUDENT"];
            DateTime dateTime = DateTime.Now;
            short[] photo = "Photo4".Select(x => (short)x).ToArray();
            student.AddRecord(null, null, dateTime, photo);

            database.Push();

            database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference, false);
            student = database.Tables["STUDENT"];
            Record s4 = student.Records.Last();

            // Then
            student.Records.Count.Should().Be(4);
            s4[student.Fields[0]].Should().BeOfType(typeof(Int64)).And.Be(4);
            s4[student.Fields[1]].Should().BeOfType(typeof(string)).And.Be(string.Empty);
            s4[student.Fields[2]].Should().BeOfType(typeof(DateTime)).And.Be(dateTime);
            ((short[])s4[student.Fields[3]]).Should().BeEquivalentTo(photo);

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_14_UDatabaseImage_AddRecord_AutoIncrement_KO()
        {
            // Given
            string reference = "202103041006";

            // When
            Action act = () =>
            {
                UDatabaseImage database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, string.Empty);
                database.Pull();

                Table student = database.Tables["STUDENT"];
                student.AddRecord(null, null, DateTime.Now, "Photo4".Select(x => (short)x).ToArray());

                database.Push();

                database = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference, false);
                student = database.Tables["STUDENT"];
                Record s4 = student.Records.Last();
            };

            // Then
            act.Should().Throw<Exception>().WithMessage("String type cannot be AutoIncrement.");

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }

        [TestMethod]
        public void Test_15_UDatabaseImage_CompetitiveAccess()
        {
            // Given
            string reference = "202103010924";
            DateTime start = DateTime.Now;
            DateTime db2Access = DateTime.Now;
            int waitTime = new Random((int)DateTime.Now.Ticks).Next(100, 3000);

            // When
            UDatabaseImage database1 = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference);
            database1.Pull();
            Table student = database1.Tables["STUDENT"];
            Record s4 = student.AddRecord(null, "S4", DateTime.Now, "Photo4".Select(x => (short)x).ToArray());

            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                
                UDatabaseImage database2 = Common.UnitTestsHelper.Helper.OpenTempDatabaseImage(reference, reference, false);
                db2Access = DateTime.Now;

                student = database2.Tables["STUDENT"];
                s4 = student.Records.Find(x => x[student.Fields[1]].ToString() == "S4");
                s4[student.Fields[3]] = "4ème Photo".Select(x => (short)x).ToArray();
            });

            thread.Start();

            Thread.Sleep(waitTime);
            database1.Push();

            thread.Join();

            // Then
            db2Access.Ticks.Should().BeGreaterThan(start.Ticks + waitTime);
            s4.Should().NotBeNull();
            student.Should().NotBeNull();
            ((short[])s4[student.Fields[3]]).Should().BeEquivalentTo("4ème Photo".Select(x => (short)x).ToArray());

            Common.UnitTestsHelper.Helper.ClearDatabaseImage(reference);
        }
    }
}
