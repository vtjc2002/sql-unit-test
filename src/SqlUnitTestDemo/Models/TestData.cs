using SqlUnitTestDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlUnitTestDemo.Models
{
    public class TestData
    {
        /// <summary>
        /// Name of the test
        /// </summary>
        public string TestName { get; set; }
        /// <summary>
        /// Whether the test is active or not
        /// </summary>
        public bool Active { get; set; }
        public DataSource Source { get; set; }
        /// <summary>
        /// Query to run against the source
        /// </summary>
        public string SourceQuery { get; set; }
        public DataSource Target { get; set; }
        /// <summary>
        /// Query to run against the target
        /// </summary>
        public string TargetQuery { get; set; }
        public string Assertion { get; set; }
    }
}
