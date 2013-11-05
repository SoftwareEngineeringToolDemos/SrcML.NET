﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ABB.SrcML.Data.Test {

    [TestFixture]
    [Category("Build")]
    public class SerializationTests {

        [SetUp]
        public void Setup() {
            if(Directory.Exists("DataRepositoryTests")) {
                Directory.Delete("DataRepositoryTests", true);
            }
        }

        [Test]
        [ExpectedException("System.Runtime.Serialization.SerializationException")]
        public void TestLoad_BadFile() {
            var archive = new SrcMLArchive("DataRepositoryTests");
            var data = new DataRepository(archive);
            data.Load(@"..\..\TestInputs\bad_file.dar");
        }

        [Test]
        public void TestRoundtrip() {
            var archive = new SrcMLArchive("DataRepositoryTests");
            archive.AddOrUpdateFile(@"..\..\TestInputs\A.h");
            archive.AddOrUpdateFile(@"..\..\TestInputs\A.cpp");
            var data = new DataRepository(archive);
            data.InitializeData();
            data.Save(@"DataRepositoryTests\saved.dar");

            var newData = new DataRepository(@"DataRepositoryTests\saved.dar");
            newData.InitializeData();
            Assert.IsTrue(TestHelper.ScopesAreEqual(data.GetGlobalScope(), newData.GetGlobalScope()));
        }

        [Test]
        [Category("LongRunning")]
        public void TestRoundtrip_Self() {
            var archive = new SrcMLArchive("DataRepositoryTests");
            foreach(var csFile in Directory.GetFiles(@"..\..\ABB.SrcML", "*.cs", SearchOption.AllDirectories)) {
                archive.AddOrUpdateFile(csFile);
            }
            var data = new DataRepository(archive);
            data.InitializeData();
            data.Save(@"DataRepositoryTests\saved.dar");

            var newData = new DataRepository(archive, @"DataRepositoryTests\saved.dar");
            newData.InitializeData();

            Assert.IsTrue(TestHelper.ScopesAreEqual(data.GetGlobalScope(), newData.GetGlobalScope()));
        }
    }
}