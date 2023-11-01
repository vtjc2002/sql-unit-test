##variable for azure resource group
variable "resource_group_name" {
  type    = string
  default = "rg-sqlunittest-ue"
}

##variable for azure location
variable "location" {
  type    = string
  default = "eastus"
}

variable "sqlusername" {
  description = "The username for the DB master user"
  type        = string
  sensitive   = true
}

variable "sqlpassword" {
  description = "The password for the DB master user"
  type        = string
  sensitive   = true
}

variable "sqldwusername" {
  description = "The username for the DW master user"
  type        = string
  sensitive   = true
}

variable "sqldwpassword" {
  description = "The password for the DW master user"
  type        = string
  sensitive   = true
}
