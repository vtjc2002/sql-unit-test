using SqlUnitTestDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlUnitTestDemo.Models
{
    public class TestMetaData
    {
        /// <summary>
        /// Name of the test.  Be descriptive as this will be used in the test results.
        /// </summary>
        public string TestName { get; set; }
        /// <summary>
        /// Whether the test is active or not.  Only active tests will be run.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// source data engine type (sql, azureSynapseServerless, azureSynapseDedicatedPool)
        /// </summary>
        public DataSource Source { get; set; }
        /// <summary>
        /// Query to run against the source
        /// </summary>
        public string SourceQuery { get; set; }
        /// <summary>
        /// target data engine type (sql, azureSynapseServerless, azureSynapseDedicatedPool)
        /// </summary>
        public DataSource Target { get; set; }
        /// <summary>
        /// Query to run against the target
        /// </summary>
        public string TargetQuery { get; set; }
        /// <summary>
        /// Assertion to run against the source and target results
        /// </summary>
        public string Assertion { get; set; }
    }
}
