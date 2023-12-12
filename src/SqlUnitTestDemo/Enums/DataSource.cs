
namespace SqlUnitTestDemo.Enums
{
    public enum DataSource
    {
        /// <summary>
        /// sql database source type matching ConnectionStrings:Sql
        /// </summary>
        sql,
        /// <summary>
        /// Azure Synapse Serverless source type matching ConnectionStrings:AzureSynapseServerless
        /// </summary>
        azureSynapseServerless,
        /// <summary>
        /// Azure Synapse Dedicated Pool source type matching ConnectionStrings:AzureSynapseDedicatedPool
        /// </summary>
        azureSynapseDedicatedPool
    }
}
