﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace HomeInventory.Tests.Acceptance.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class AddJugOfLiquidFeature : object, Xunit.IClassFixture<AddJugOfLiquidFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "AddJugOfLiquid.feature"
#line hidden
        
        public AddJugOfLiquidFeature(AddJugOfLiquidFeature.FixtureData fixtureData, HomeInventory_Tests_Acceptance_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "AddJugOfLiquid", @"    As a registered user
    I would like to register that:
    - I bought a jug of liquid at some non-future date in some store and payed some price
    - Liquid that is stored in a jug will expire at some future date, relative to manufactured date or absolute
    - The jug is stored in the fridge storage area
    So that I know how much liquid I have and when I need to buy more", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 9
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Store",
                        "Product",
                        "Price",
                        "Expiration",
                        "UnitVolume"});
            table1.AddRow(new string[] {
                        "Walmart",
                        "Milk",
                        "2.99",
                        "12/12/2022",
                        "1"});
#line 10
    testRunner.Given("That today is 12/02/2022 and following environment", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Area"});
            table2.AddRow(new string[] {
                        "Fridge"});
#line 13
    testRunner.And("Following context", ((string)(null)), table2, "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="User bought a gallon jug of milk from store")]
        [Xunit.TraitAttribute("FeatureTitle", "AddJugOfLiquid")]
        [Xunit.TraitAttribute("Description", "User bought a gallon jug of milk from store")]
        [Xunit.TraitAttribute("Category", "WI21")]
        public void UserBoughtAGallonJugOfMilkFromStore()
        {
            string[] tagsOfScenario = new string[] {
                    "WI21"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("User bought a gallon jug of milk from store", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 19
    testRunner.Given("User bought a 1 gallon jug of \"Milk\" at 12/02/2022 in \"Walmart\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 20
    testRunner.When("User stores jug in to the \"Fridge\" storage area", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 21
    testRunner.Then("The \"Fridge\" storage area should contain 1 gallon jug of \"Milk\" that will expire " +
                        "at 12/12/2022", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 22
    testRunner.And("A transaction was registered: User bought 1 gallon jug of \"Milk\" at 12/02/2022 in" +
                        " \"Walmart\" and payed $2.99", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                AddJugOfLiquidFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                AddJugOfLiquidFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
