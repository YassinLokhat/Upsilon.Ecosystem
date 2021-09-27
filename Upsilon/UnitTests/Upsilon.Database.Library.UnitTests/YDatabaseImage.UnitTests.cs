using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.IO;
using System.Linq;
using System.Threading;
using Upsilon.Common.MetaHelper;

namespace Upsilon.Database.Library.UnitTests
{
    [TestClass]
    public class YDatabaseImage_UnitTests : YUnitTestsClass
    {
        [TestMethod]
        public void Test_00_DatabaseCreation()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Directory = YUnitTestFilesDirectory.Database,
                Key = string.Empty,
                CheckExistingFile = false,
            };
            YHelper.ClearTestFile(configuration);
            string filepath = YHelper.GetDatabaseFilePath(configuration);

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            database.Push();
            database.Close();

            // Then
            File.Exists(filepath).Should().BeTrue();

            // Finally
            configuration.CheckExistingFile = true;
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_01_DatabaseCreation_AddRecord()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Directory = YUnitTestFilesDirectory.Database,
                Key = string.Empty,
                CheckExistingFile = false,
            };
            YHelper.ClearTestFile(configuration);

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
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
            database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            DatabaseClasses.AUTHOR author = database.AUTHORs[0];
            DatabaseClasses.BOOK book1 = database.BOOKs[0];
            DatabaseClasses.BOOK book2 = database.BOOKs[1];

            // Then
            author.Name.Should().Be("William Shakespeare");
            author.BirthDay.ToString("yyyy-MM-dd").Should().Be("1564-04-01");
            author.Books.Should().Equal(new DatabaseClasses.BOOK[] { book1, book2 });
            book1.Title.Should().Be("Hamlet");
            book1.Author.Should().Be("William Shakespeare");
            book1.Synopsis.Should().Be("Hamlet's Synopsis");
            book2.Title.Should().Be("Macbeth");
            book2.Author.Should().Be("William Shakespeare");
            book2.Synopsis.Should().Be("Macbeth's Synopsis");

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_02_PersistanceTest_RemoveRecord()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105240752",
                Directory = YUnitTestFilesDirectory.Database,
                Key = string.Empty,
            };

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            database.Pull();

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            DatabaseClasses.AUTHOR author = database.AUTHORs[0];
            DatabaseClasses.BOOK book1 = database.BOOKs[0];
            DatabaseClasses.BOOK book2 = database.BOOKs[1];
            database.BOOKs.Remove(book2);
            database.Push();

            configuration.ResetTempFile = false;
            database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            database.Pull(false);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(1);
            database.AUTHORs[0].InternalIndex.Should().Be(author.InternalIndex);
            author.Books[0].InternalIndex.Should().Be(book1.InternalIndex);

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_03_Changing_Key()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105240752",
                Directory = YUnitTestFilesDirectory.Database,
                Key = string.Empty,
            };
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // When
            configuration.Key = "key";
            configuration.ResetTempFile = false;
            database.SaveAs(string.Empty, configuration.Key);
            database.Close();

            database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            DatabaseClasses.AUTHOR author = database.AUTHORs[0];
            DatabaseClasses.BOOK book1 = database.BOOKs[0];
            DatabaseClasses.BOOK book2 = database.BOOKs[1];

            // Then
            author.Name.Should().Be("William Shakespeare");
            author.BirthDay.ToString("yyyy-MM-dd").Should().Be("1564-04-01");
            author.Books.Should().Equal(new DatabaseClasses.BOOK[] { book1, book2 });
            book1.Title.Should().Be("Hamlet");
            book1.Author.Should().Be("William Shakespeare");
            book1.Synopsis.Should().Be("Hamlet's Synopsis");
            book2.Title.Should().Be("Macbeth");
            book2.Author.Should().Be("William Shakespeare");
            book2.Synopsis.Should().Be("Macbeth's Synopsis");

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_04_WrongKey()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105240752",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            // When
            Action act = () =>
            {
                DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            };

            // Then
            act.Should().ThrowExactly<YWrongDatabaseKeyException>().And.Key.Should().Be("key");

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_05_WrongKey_CorruptHash()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105241501",
                Directory = YUnitTestFilesDirectory.Database,
                Key = string.Empty,
            };

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(0);
            database.BOOKs.Count.Should().Be(0);

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_06_MappingToClassesWithLessData()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105241602",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };
            YDatabaseHelperConfiguration configurationReference = new()
            {
                Reference = "202105250546",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            // When
            string databaseContentReference = File.ReadAllText(YHelper.GetDatabaseFilePath(configurationReference, false));
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);
            database.BOOKs[0].Title.Should().Be("Hamlet");
            database.BOOKs[1].Title.Should().Be("Macbeth");

            // When
            database.Pull();
            database.BOOKs.Add(new(database)
            {
                Title = "Macbeth",
                Author = "William Shakespeare",
                Synopsis = "Macbeth's Synopsis",
            });
            database.Push();
            string databaseContent = File.ReadAllText(YHelper.GetDatabaseFilePath(configuration));

            // Then
            databaseContent.Should().Be(databaseContentReference);

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_07_MappingToClassesWithMoreData()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105250557",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };
            YDatabaseHelperConfiguration configurationReference = new()
            {
                Reference = "202105241500",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            // When
            string databaseContentReference = File.ReadAllText(YHelper.GetDatabaseFilePath(configurationReference, false));
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);
            database.BOOKs[0].Title.Should().Be("Hamlet");
            database.BOOKs[0].Synopsis.Should().BeNull();
            database.BOOKs[1].Title.Should().Be("Macbeth");
            database.BOOKs[1].Synopsis.Should().BeNull();

            // When
            database.Pull();
            database.BOOKs[0].Synopsis = "Hamlet's Synopsis";
            database.BOOKs[1].Synopsis = "Macbeth's Synopsis";
            database.Push();
            string databaseContent = File.ReadAllText(YHelper.GetDatabaseFilePath(configuration));

            // Then
            databaseContent.Should().Be(databaseContentReference);

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_09_BadConceptionClasses_InconsistenFieldType()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105250606",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            // When
            Action act = () => {
                DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            };

            // Then
            act.Should().ThrowExactly<YDatabaseClassesDefinitionException>().WithMessage("Database Classes has bad definition.\n" +
                                                                                         "Table name : 'AUTHOR'\n" +
                                                                                         "Type 'System.DateTime' does not match with 'System.String' type for the 'BirthDay' field.");

            // Finally
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_10_CompetitiveAccess()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105241500",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            long timeAfterFirstPush = long.MaxValue;
            long timeAfterSecondPull = long.MaxValue - 1;
            Thread thread = new(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                configuration.ResetTempFile = false;
                DatabaseClasses.Database database1 = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
                database1.Pull();
                timeAfterSecondPull = DateTime.Now.Ticks;

                database1.AUTHORs.Count.Should().Be(2);
                database1.BOOKs.Count.Should().Be(2);
                database1.Push();
                database1.Close();
            });

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);
            database.Pull();
            thread.Start();
            Thread.Sleep(500);
            database.AUTHORs.Add(new(database)
            {
                Name = "Molière",
                BirthDay = new DateTime(1622,01,15),
            });

            database.AUTHORs.Count.Should().Be(2);
            database.BOOKs.Count.Should().Be(2);
            database.Push();
            timeAfterFirstPush = DateTime.Now.Ticks;
            database.Close();

            thread.Join();

            // Then
            timeAfterFirstPush.Should().BeLessOrEqualTo(timeAfterSecondPull);

            // Finally
            configuration.CheckExistingFile = true;
            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_11_SaveAs()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105241500",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            YDatabaseHelperConfiguration configuration2 = new()
            {
                Reference = "202105241500_copy",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
                CheckExistingFile = false,
            };

            // When
            database.SaveAs(YHelper.GetDatabaseFilePath(configuration2), configuration2.Key);
            database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration2); 

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(2);

            // When
            DatabaseClasses.AUTHOR author = database.AUTHORs[0];
            DatabaseClasses.BOOK book1 = database.BOOKs[0];
            DatabaseClasses.BOOK book2 = database.BOOKs[1];

            // Then
            author.Name.Should().Be("William Shakespeare");
            author.BirthDay.ToString("yyyy-MM-dd").Should().Be("1564-04-01");
            author.Books.Should().Equal(new DatabaseClasses.BOOK[] { book1, book2 });
            book1.Title.Should().Be("Hamlet");
            book1.Author.Should().Be("William Shakespeare");
            book1.Synopsis.Should().Be("Hamlet's Synopsis");
            book2.Title.Should().Be("Macbeth");
            book2.Author.Should().Be("William Shakespeare");
            book2.Synopsis.Should().Be("Macbeth's Synopsis");

            // Finally
            YHelper.ClearTestFile(configuration);
            YHelper.ClearTestFile(configuration2);
        }

        [TestMethod]
        public void Test_12_RebuildInternalIndex()
        {
            // Given
            YDatabaseHelperConfiguration configuration = new()
            {
                Reference = "202105281926",
                Directory = YUnitTestFilesDirectory.Database,
                Key = "key",
            };

            // When
            DatabaseClasses.Database database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(3);

            // When
            DatabaseClasses.BOOK book1 = database.BOOKs[0];
            DatabaseClasses.BOOK book2 = database.BOOKs[1];
            DatabaseClasses.BOOK book3 = database.BOOKs[2];

            // Then
            book1.InternalIndex.Should().Be(4);
            book2.InternalIndex.Should().Be(5);
            book3.InternalIndex.Should().Be(6);

            // When
            database.RebuildInternalIndex(new[] { "BOOK" });
            configuration.ResetTempFile = false;
            database = YHelper.OpenDatabaseImage<DatabaseClasses.Database>(configuration);

            // Then
            database.AUTHORs.Count.Should().Be(1);
            database.BOOKs.Count.Should().Be(3);

            // When
            book1 = database.BOOKs[0];
            book2 = database.BOOKs[1];
            book3 = database.BOOKs[2];

            // Then
            book1.InternalIndex.Should().Be(1);
            book2.InternalIndex.Should().Be(2);
            book3.InternalIndex.Should().Be(3);

            // Finally
            YHelper.ClearTestFile(configuration);
        }
    }
}
