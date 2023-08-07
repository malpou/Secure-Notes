resource "azurerm_storage_account" "storage" {
  name                      = "securenotesstorage"
  resource_group_name       = azurerm_resource_group.rg.name
  location                  = azurerm_resource_group.rg.location
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  account_kind              = "StorageV2"
  enable_https_traffic_only = true
  min_tls_version           = "TLS1_2"

  network_rules {
    default_action             = "Allow"
    bypass                     = ["AzureServices"]
    ip_rules                   = []
    virtual_network_subnet_ids = []
  }
}

resource "azurerm_storage_table" "user_table" {
  name                 = "users"
  storage_account_name = azurerm_storage_account.storage.name
}

resource "azurerm_storage_table" "note_table" {
  name                 = "notes"
  storage_account_name = azurerm_storage_account.storage.name
}