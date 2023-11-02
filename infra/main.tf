
##create a resource group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

##create random id
resource "random_string" "randomsqlserver1" {
  length  = 8
  lower = true
  upper = false
  special = false
}

resource "random_string" "randomsqlserver2" {
  length  = 8
  lower = true
  upper = false
  special = false
}

##create sql server source
resource "azurerm_mssql_server" "sqlserversource" {
  name                         = "sqlserver${random_string.randomsqlserver1.result}"
  location                     = azurerm_resource_group.rg.location
  resource_group_name          = azurerm_resource_group.rg.name
  version                      = "12.0"
  administrator_login          = var.sqlusername
  administrator_login_password = var.sqlpassword
  public_network_access_enabled = true
}

##create sql database source
resource "azurerm_mssql_database" "sqldbsource" {
  name           = "testsourcedb"
  server_id      = azurerm_mssql_server.sqlserversource.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 4
  read_scale     = false
  sku_name       = "S0"
  zone_redundant = false
}

##create synapse sql pool target
resource "azurerm_storage_account" "sqldwsatarget" {
  name                     = "sa${random_string.randomsqlserver2.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "BlobStorage"
}

resource "azurerm_storage_data_lake_gen2_filesystem" "sqldwadls" {
  name               = "targetadls"
  storage_account_id = azurerm_storage_account.sqldwsatarget.id
}

resource "azurerm_synapse_workspace" "targetworkspace" {
  name                                 = "sy${random_string.randomsqlserver2.result}}"
  resource_group_name                  = azurerm_resource_group.rg.name
  location                             = azurerm_resource_group.rg.location
  storage_data_lake_gen2_filesystem_id = azurerm_storage_data_lake_gen2_filesystem.sqldwadls.id
  sql_administrator_login              = var.sqldwusername
  sql_administrator_login_password     = var.sqldwpassword
  public_network_access_enabled = true

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_synapse_sql_pool" "sqlpooltarget" {
  name                 = "sqlpooltarget"
  synapse_workspace_id = azurerm_synapse_workspace.targetworkspace.id
  sku_name             = "DW100c"
  create_mode          = "Default"
  storage_account_type = "LRS"
  geo_backup_policy_enabled = false
}

##create azure key vault
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "unittest" {
  name                       = "unittestkeyvault${random_string.randomsqlserver1.result}"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Create",
      "Get",
    ]

    secret_permissions = [
      "Set",
      "Get",
      "Delete",
      "Purge",
      "Recover"
    ]
  }
}

##store sql username/pwd in keyvault
resource "azurerm_key_vault_secret" "sqlusernamesource" {
  name         = "secret-sql-username-source"
  value        = var.sqlusername
  key_vault_id = azurerm_key_vault.unittest.id
}

resource "azurerm_key_vault_secret" "sqlpasswordsource" {
  name         = "secret-sql-password-source"
  value        = var.sqlpassword
  key_vault_id = azurerm_key_vault.unittest.id
}

##store sql dw username/pwd in keyvault
resource "azurerm_key_vault_secret" "sqldwusernametarget" {
  name         = "secret-sqldw-username-target"
  value        = var.sqldwusername
  key_vault_id = azurerm_key_vault.unittest.id
}

resource "azurerm_key_vault_secret" "sqldwpasswordtarget" {
  name         = "secret-sqldw-password-target"
  value        = var.sqldwpassword
  key_vault_id = azurerm_key_vault.unittest.id
}