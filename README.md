# Intro
The sample is meant to demonstrate integration test between 2 sql sources utilizing dontnet test and xUnit test framework.  The source db and target db are in Azure in the sample but can be easily modified to work on any sql datastore.

## Project Prerequisites
- [Visual Studio](https://visualstudio.microsoft.com/): (Optional) You can use Visual Studio for developing and managing your .NET 6 test project.
- [Visual Studio Code](https://code.visualstudio.com/): (Optional) Visual Studio Code is a lightweight and highly customizable code editor suitable for .NET development.
- [Terraform](https://www.terraform.io/downloads.html): Terraform is used for managing infrastructure as code. Ensure that Terraform is installed if you plan to use infrastructure automation.
- [Azure cli](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) The Azure Command-Line Interface (CLI) is a cross-platform command-line tool to connect to Azure and execute administrative commands on Azure resources.

## Azure Infrastructure
You can skip this portion if you have your Azure infrastructure setup already.  

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
