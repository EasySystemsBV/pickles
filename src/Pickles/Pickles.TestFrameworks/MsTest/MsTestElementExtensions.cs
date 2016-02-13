//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MsTestElementExtensions.cs" company="PicklesDoc">
//  Copyright 2011 Jeffrey Cameron
//  Copyright 2012-present PicklesDoc team and community contributors
//
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using PicklesDoc.Pickles.ObjectModel;

namespace PicklesDoc.Pickles.TestFrameworks.MsTest
{
    internal static class MsTestElementExtensions
    {
        private const string Failed = "failed";

        private static readonly XNamespace Ns = @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        internal static bool HasPropertyFeatureTitle(this XElement parentElement, string featureTitle)
        {
            //// <Properties>
            ////   <Property>
            ////     <Key>FeatureTitle</Key>
            ////     <Value>  featureName  </Value>
            ////   </Property>
            //// </Properties>

            var propertiesElement = parentElement.Element(Ns + "Properties");

            if (propertiesElement == null)
            {
                return false;
            }

            var query = from property in propertiesElement.Elements(Ns + "Property")
                        let key = property.Element(Ns + "Key")
                        let value = property.Element(Ns + "Value")
                        where key.Value == "FeatureTitle" && value.Value == featureTitle
                        select property;
            return query.Any();
        }

        internal static string Name(this XElement scenario)
        {
            return scenario.Element(Ns + "Description").Value;
        }

        internal static IEnumerable<XElement> AllExecutionResults(this XDocument document)
        {
            return document.Root.Descendants(Ns + "UnitTestResult");
        }


        /// <summary>
        /// Retrieves all potential scenarios in the test result file. "Potential" because
        /// there may be some regular unit tests included as well. They cause no problems, however.
        /// </summary>
        /// <param name="document">The test result file.</param>
        /// <returns>
        /// A sequence of <see cref="XElement"/> instances that are called "UnitTest".
        /// </returns>
        internal static IEnumerable<XElement> AllScenarios(this XDocument document)
        {
            // TestRun/TestDefinitions/UnitTests that have a non-empty Description (which is the title of a Scenario).
            return document.Root.Descendants(Ns + "UnitTest").Where(s => s.Element(Ns + "Description") != null);
        }

        internal static Guid ExecutionIdElement(this XElement scenario)
        {
            var xElement = scenario.Element(Ns + "Execution");

            return xElement != null ? new Guid(xElement.Attribute("id").Value) : Guid.Empty;
        }

        internal static IEnumerable<Guid> ExecutionIds(this IEnumerable<XElement> scenarios)
        {
            return scenarios.Select(ExecutionIdElement);
        }

        internal static TestResult Outcome(this XElement scenarioResult)
        {
            var outcomeAttribute = scenarioResult.Attribute("outcome")?.Value ?? Failed;

            switch (outcomeAttribute.ToLowerInvariant())
            {
                case "passed":
                    return TestResult.Passed;
                case Failed:
                    return TestResult.Failed;
                default:
                    return TestResult.Inconclusive;
            }
        }

        internal static Guid ExecutionIdAttribute(this XElement unitTestResult)
        {
            var executionIdAttribute = unitTestResult.Attribute("executionId");
            return executionIdAttribute != null ? new Guid(executionIdAttribute.Value) : Guid.Empty;
        }
    }
}