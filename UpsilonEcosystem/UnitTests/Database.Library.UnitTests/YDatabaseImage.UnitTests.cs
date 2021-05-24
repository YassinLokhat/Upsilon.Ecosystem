using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.IO;
using System.Linq;
using System.Threading;
using Upsilon.Common.UnitTestsHelper;

namespace Upsilon.Database.Library.UnitTests
{
    [TestClass]
    public class YDatabaseImage_UnitTests
    {
        private readonly string _databaseDirectory = @"\UpsilonEcosystem\UnitTests\Tests\Database\";

        [TestMethod]
        public void Test_00_DatabaseCreation()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "empty",
                DatabaseDirectory = _databaseDirectory,
                Key = string.Empty,
                CheckExistingFile = false,
            };
            YHelper.ClearDatabaseImage(configuration);
            string filepath = YHelper.GetDatabaseFilePath(configuration);

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Push();
            database.Close();

            // Then
            File.Exists(filepath).Should().BeTrue();

            // Finally
            configuration.CheckExistingFile = true;
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_01_DatabaseCreation_AddRecord()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "empty",
                DatabaseDirectory = _databaseDirectory,
                Key = string.Empty,
                CheckExistingFile = false,
            };
            YHelper.ClearDatabaseImage(configuration);

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Pull();

            database.AUTHORs.Add(new(database)
            {
                Name = "William Shakespeare",
                BirthDay = new DateTime(1564, 04, 01),
            });

            database.BOOKs.Add(new(database)
            {
                Title = "Hamlet",
                Author = "William Shakespeare",
                Synopsis = "Hamlet's Synopsis",
            });

            database.BOOKs.Add(new(database)
            {
                Title = "Macbeth",
                Author = "William Shakespeare",
                Synopsis = "Macbeth's Synopsis",
            });

            database.Push();

            configuration.CheckExistingFile = true;
            database = YHelper.OpenDatabaseImage<Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            AUTHOR author = database.AUTHORs[0];
            BOOK book1 = database.BOOKs[0];
            BOOK book2 = database.BOOKs[1];

            // Then
            author.Name.Should().Be("William Shakespeare");
            author.BirthDay.ToString("yyyy-MM-dd").Should().Be("1564-04-01");
            author.Books.Should().Equal(new BOOK[] { book1, book2 });
            book1.Title.Should().Be("Hamlet");
            book1.Author.Should().Be("William Shakespeare");
            book1.Synopsis.Should().Be("Hamlet's Synopsis");
            book2.Title.Should().Be("Macbeth");
            book2.Author.Should().Be("William Shakespeare");
            book2.Synopsis.Should().Be("Macbeth's Synopsis");

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_02_PersistanceTest_RemoveRecord()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105240752",
                DatabaseDirectory = _databaseDirectory,
                Key = string.Empty,
            };

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Pull();

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            AUTHOR author = database.AUTHORs[0];
            BOOK book1 = database.BOOKs[0];
            BOOK book2 = database.BOOKs[1];
            database.BOOKs.Remove(book2);
            database.Push();

            configuration.ResetTempDatabase = false;
            database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Pull(false);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(1);
            database.AUTHORs[0].InternalIndex.Should().Be(author.InternalIndex);
            author.Books[0].InternalIndex.Should().Be(book1.InternalIndex);

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_03_Changing_Key()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105240752",
                DatabaseDirectory = _databaseDirectory,
                Key = string.Empty,
            };
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);

            // When
            configuration.Key = "key";
            configuration.ResetTempDatabase = false;
            database.ChangeKey(configuration.Key);
            database.Close();

            database = YHelper.OpenDatabaseImage<Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            AUTHOR author = database.AUTHORs[0];
            BOOK book1 = database.BOOKs[0];
            BOOK book2 = database.BOOKs[1];

            // Then
            author.Name.Should().Be("William Shakespeare");
            author.BirthDay.ToString("yyyy-MM-dd").Should().Be("1564-04-01");
            author.Books.Should().Equal(new BOOK[] { book1, book2 });
            book1.Title.Should().Be("Hamlet");
            book1.Author.Should().Be("William Shakespeare");
            book1.Synopsis.Should().Be("Hamlet's Synopsis");
            book2.Title.Should().Be("Macbeth");
            book2.Author.Should().Be("William Shakespeare");
            book2.Synopsis.Should().Be("Macbeth's Synopsis");

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_04_WrongKey()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105240752",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            Action act = () =>
            {
                Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            };

            // Then
            act.Should().ThrowExactly<YWrongDatabaseKeyException>();

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_05_WrongKey_CorruptHash()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105241501",
                DatabaseDirectory = _databaseDirectory,
                Key = string.Empty,
            };

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(0);
            database.BOOKs.Count.Should().Be(0);

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_06_MappingToClassesWithLessData()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105241602",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            string databaseBefore = File.ReadAllText(YHelper.GetDatabaseFilePath(configuration));
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);
            database.BOOKs[0].Title.Should().Be("Hamlet");
            database.BOOKs[1].Title.Should().Be("Macbeth");

            // When
            configuration.ResetTempDatabase = false;
            Database2 database2 = YHelper.OpenDatabaseImage<Database2>(configuration);
            database2.Sync();

            // Then
            database2.PLATFORMs.Count.Should().Be(1);
            database2.LOGINs.Count.Should().Be(2);
            database2.Close();

            // When
            PLATFORM2 platform = database2.PLATFORMs[0];
            LOGIN2 login1 = database2.LOGINs[0];
            LOGIN2 login2 = database2.LOGINs[1];

            // Then
            platform.Label.Should().Be("MyWebsite");
            platform.LOGINs.Should().Equal(new LOGIN2[] { login1, login2 });
            login1.Label.Should().Be("user 1");
            login1.UserName.Should().Be("user1");
            login1.Password.Should().Be("password1");
            login1.PLATFORM_Label.Should().Be("MyWebsite");
            login2.Label.Should().Be("user 2");
            login2.UserName.Should().Be("user2");
            login2.Password.Should().Be("password2");
            login2.PLATFORM_Label.Should().Be("MyWebsite");

            // When
            database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Sync();

            // Then
            database.PLATFORMs.Count.Should().Be(1);
            database.LOGINs.Count.Should().Be(2);
            database.PLATFORMs.First().Label.Should().Be("MyWebsite");
            database.PLATFORMs.First().Url.Should().Be(string.Empty);   /* /!\ Data Loss /!\ */
            database.Close();

            // Finally
            YHelper.ClearDatabaseImage(configuration);*/
        }

        /*[TestMethod]
        public void Test_08_MappingToClassesWithMoreData()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202102270700",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Sync();

            // Then
            database.PLATFORMs.Count.Should().Be(1);
            database.LOGINs.Count.Should().Be(2);
            database.PLATFORMs.First().Url.Should().Be("www.mywebsite.com");
            database.Close();

            // When
            configuration.ResetTempDatabase = false;
            Database3 database3 = YHelper.OpenDatabaseImage<Database3>(configuration);
            database3.Sync();

            // Then
            database3.PLATFORMs.Count.Should().Be(1);
            database3.LOGINs.Count.Should().Be(2);
            database3.Close();

            // When
            PLATFORM3 platform = database3.PLATFORMs[0];
            LOGIN3 login1 = database3.LOGINs[0];
            LOGIN3 login2 = database3.LOGINs[1];

            // Then
            platform.PLATFORM_ID.Should().Be(1);
            platform.Label.Should().Be("MyWebsite");
            platform.Url.Should().Be("www.mywebsite.com");
            platform.Description.Should().Be("My default platform description");
            platform.LOGINs.Should().Equal(new LOGIN3[] { login1, login2 });
            login1.Label.Should().Be("user 1");
            login1.UserName.Should().Be("user1");
            login1.Password.Should().Be("password1");
            login1.PLATFORM_Label.Should().Be("MyWebsite");
            login2.Label.Should().Be("user 2");
            login2.UserName.Should().Be("user2");
            login2.Password.Should().Be("password2");
            login2.PLATFORM_Label.Should().Be("MyWebsite");

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        /*[TestMethod]
        public void Test_09_BadConceptionClasses_WrongFieldNumber()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202102270700",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            Action act = () => {
                Database4 database4 = YHelper.OpenDatabaseImage<Database4>(configuration);
                database4.Sync();
            };

            // Then
            act.Should().ThrowExactly<YInconsistentRecordFieldCountException>();

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        /*[TestMethod]
        public void Test_10_BadConceptionClasses_InconsistenFieldType()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202102270700",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            Action act = () => {
                Database5 database5 = YHelper.OpenDatabaseImage<Database5>(configuration);
                database5.Sync();
            };

            // Then
            act.Should().ThrowExactly<YInconsistentRecordFieldTypeException>();

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        /*[TestMethod]
        public void Test_11_AutoIncrementAndDefaultValues()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "empty",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
                CheckExistingFile = false
            };
            YHelper.ClearDatabaseImage(configuration);

            // When
            Database3 database3 = YHelper.OpenDatabaseImage<Database3>(configuration);
            database3.Pull();
            database3.PLATFORMs.Add(new PLATFORM3(database3)
            {
                Label = "My first website",
                Url = "www.mywebsite.com",
                Description = "My first website description",
            });
            database3.PLATFORMs.Add(new PLATFORM3(database3)
            {
                Label = "My second website",
                Url = "www.mywebsite2.com",
            });
            database3.Push();

            // Then
            database3.PLATFORMs.Count.Should().Be(2);

            // When
            database3.Sync();
            PLATFORM3 platform1 = database3.PLATFORMs[0];
            PLATFORM3 platform2 = database3.PLATFORMs[1];

            // Then
            platform1.PLATFORM_ID.Should().Be(1);
            platform1.Label.Should().Be("My first website");
            platform1.Url.Should().Be("www.mywebsite.com");
            platform1.Description.Should().Be("My first website description");
            platform1.LOGINs.Count.Should().Be(0);
            platform2.PLATFORM_ID.Should().Be(2);
            platform2.Label.Should().Be("My second website");
            platform2.Url.Should().Be("www.mywebsite2.com");
            platform2.Description.Should().Be("My default platform description");
            platform2.LOGINs.Count.Should().Be(0);

            // Finally
            configuration.CheckExistingFile = true;
            YHelper.ClearDatabaseImage(configuration);
        }

        /*[TestMethod]
        public void Test_12_CompetitiveAccess()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202102270700",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            long timeAfterFirstPush = long.MaxValue;
            long timeAfterSecondPull = long.MaxValue - 1;
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                configuration.ResetTempDatabase = false;
                Database database1 = YHelper.OpenDatabaseImage<Database>(configuration);
                database1.Pull();
                timeAfterSecondPull = DateTime.Now.Ticks;

                database1.PLATFORMs.Count.Should().Be(2);
                database1.LOGINs.Count.Should().Be(2);
                database1.Push();
                database1.Close();
            });

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            database.Pull();
            thread.Start();
            Thread.Sleep(500);
            database.PLATFORMs.Add(new(database)
            {
                Label = "Label",
                Url = "www.label.lab",
            });

            database.PLATFORMs.Count.Should().Be(2);
            database.LOGINs.Count.Should().Be(2);
            database.Push();
            timeAfterFirstPush = DateTime.Now.Ticks;
            database.Close();

            thread.Join();

            // Then
            timeAfterFirstPush.Should().BeLessOrEqualTo(timeAfterSecondPull);

            // Finally
            configuration.CheckExistingFile = true;
            YHelper.ClearDatabaseImage(configuration);
        }
    */

        /// TODO : Test 2 tests for SaveAs
        /// TODO : Add fields check mechanism (Tests 07 and 08 should failed)
    }
}
