using Newtonsoft.Json;
using SqlUnitTestDemo.Models;
using System.Collections;

namespace SqlUnitTestDemo.TestData
{
    public class CovidDeathTestDataGenerator: IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            List<TestMetaData> items;

            //read data from testData.json file and deserialize into a list of TestData objects
            using (StreamReader r = new StreamReader(@"TestData/CovidDeathTestSet.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<TestMetaData>>(json);
            }

            //return the list of active TestData objects
            foreach (var item in items.Where(w => w.Active))
            {
                yield return new object[] { item.TestName, item.Source, item.SourceQuery, item.Target, item.TargetQuery, item.Assertion };
            }
            
        }
 
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
