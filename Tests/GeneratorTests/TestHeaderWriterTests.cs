﻿using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using Should;

namespace GeneratorTests
{
    [TestFixture]
    public class TestHeaderWriterTests : TestGeneratorTestsBase
    {
        private TestHeaderWriter CreateTestHeaderWriter()
        {
            return new TestHeaderWriter();
        }

        [Test]
        public void Should_return_version_from_a_cs_file_with_specific_generator_version()
        {
            var testHeaderWriter = CreateTestHeaderWriter();
            var result = testHeaderWriter.DetectGeneratedTestVersion(@"// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.4.0.0
//      SpecFlow Generator Version:1.3.0.0
//      Runtime Version:4.0.30319.225
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace Bowling.Specflow
{
}
#endregion
");

            result.ShouldNotBeNull();
            result.ToString().ShouldEqual("1.3.0.0");
        }

        [Test]
        public void Should_return_version_from_a_vb_file_with_specific_generator_version()
        {
            var testHeaderWriter = CreateTestHeaderWriter();
            var result = testHeaderWriter.DetectGeneratedTestVersion(@"'------------------------------------------------------------------------------
' <auto-generated>
' This code was generated by SpecFlow (http://www.specflow.org/).
' SpecFlow Version:1.3.2.0
' SpecFlow Generator Version:1.4.0.0
' Runtime Version:2.0.50727.4927
'
' Changes to this file may cause incorrect behavior and will be lost if
' the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------
");

            result.ShouldNotBeNull();
            result.ToString().ShouldEqual("1.4.0.0");
        }

        [Test]
        public void Should_return_version_from_a_cs_file_with_specflow_version()
        {
            var testHeaderWriter = CreateTestHeaderWriter();
            var result = testHeaderWriter.DetectGeneratedTestVersion(@"// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.6.0.0
//      Runtime Version:4.0.30319.225
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace Bowling.Specflow
{
}
#endregion
");

            result.ShouldNotBeNull();
            result.ToString().ShouldEqual("1.6.0.0");
        }

        [Test]
        public void Should_return_null_from_other_files()
        {
            var testHeaderWriter = CreateTestHeaderWriter();
            var result = testHeaderWriter.DetectGeneratedTestVersion(@"not-a-generated-file");

            result.ShouldBeNull();
        }
    }
}
