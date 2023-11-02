# sql-unit-test
Demo on sql-unit-test

## infra
use infra/terraform to create infra.
The sql db and synapse dw username and passwords will be saved to keyvault

set enviornment variables to desired sql auth values
$env:TF_VAR_sqlusername
$env:TF_VAR_sqlpassword
$env:TF_VAR_sqldwusername
$env:TF_VAR_sqldwpassword

run az login
az account set --subscription "subscriptionid"
terraform plan -out main.tf
terraform apply main.tf


## application
add keyvault name to appsettings.json
add sql connection string to appsettings.json and replace username with ##username## and password with ##password## 
add sql dw connection string to appsettings.json and replace username with ##username## and password with ##password## 
the unit tests will grab the username/pwd from keyvault and replace them in connection strings.

## Azure DevOps
https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity?view=azure-devops
use the service principle created and assign Key Vault Reader role in keyvault.
create service principle secret and save to AzDo for pipeline use.