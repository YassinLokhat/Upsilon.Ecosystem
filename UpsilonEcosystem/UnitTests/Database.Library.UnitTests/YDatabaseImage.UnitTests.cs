﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            act.Should().ThrowExactly<YWrongDatabaseKeyException>().And.Key.Should().Be("key");

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
            YHelperDatabaseConfiguration configurationReference = new()
            {
                Reference = "202105250546",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            string databaseContentReference = File.ReadAllText(YHelper.GetDatabaseFilePath(configurationReference, false));
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);

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
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_07_MappingToClassesWithMoreData()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105250557",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };
            YHelperDatabaseConfiguration configurationReference = new()
            {
                Reference = "202105241500",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            string databaseContentReference = File.ReadAllText(YHelper.GetDatabaseFilePath(configurationReference, false));
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);

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
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_09_BadConceptionClasses_InconsistenFieldType()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105250606",
                DatabaseDirectory = _databaseDirectory,
                Key = "key",
            };

            // When
            Action act = () => {
                Database database = YHelper.OpenDatabaseImage<Database>(configuration);
            };

            // Then
            act.Should().ThrowExactly<YDatabaseClassesDefinitionException>().WithMessage("Database Classes has bad definition.\n" +
                                                                                         "Table name : 'AUTHOR'\n" +
                                                                                         "Type 'System.DateTime' does not match with 'System.String' type for the 'BirthDay' field.");

            // Finally
            YHelper.ClearDatabaseImage(configuration);
        }

        [TestMethod]
        public void Test_10_CompetitiveAccess()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202105241500",
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

                database1.AUTHORs.Count.Should().Be(2);
                database1.BOOKs.Count.Should().Be(2);
                database1.Push();
                database1.Close();
            });

            // When
            Database database = YHelper.OpenDatabaseImage<Database>(configuration);
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
            YHelper.ClearDatabaseImage(configuration);
        }

        /// TODO : Test 2 tests for SaveAs and RebuildInternalIndex
    }
}
