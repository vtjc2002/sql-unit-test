using Newtonsoft.Json;
using System.Collections;

namespace SqlUnitTestDemo.TestData
{
    public class TestDataGenerator: IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            List<Models.TestData> items;

            //read data from testData.json file and deserialize into a list of TestData objects
            using (StreamReader r = new StreamReader(@"TestData/testData.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Models.TestData>>(json);
            }

            //return the list of active TestData objects
            foreach (var item in items.Where(w => w.Active))
            {
                yield return new object[] { item.TestName, item.Source, item.SourceQuery, item.Target, item.TargetQuery, item.Assert };
            }
            
        }
 
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
