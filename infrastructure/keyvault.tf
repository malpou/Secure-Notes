resource "azurerm_key_vault" "vault" {
  name                            = "secure-notes-secrets"
  location                        = azurerm_resource_group.rg.location
  resource_group_name             = azurerm_resource_group.rg.name
  tenant_id                       = data.azurerm_client_config.current.tenant_id
  sku_name                        = "premium"
  enabled_for_deployment          = false
  enabled_for_disk_encryption     = false
  enabled_for_template_deployment = false
  enable_rbac_authorization       = false
  purge_protection_enabled        = false
  soft_delete_retention_days      = 7
}

resource "azurerm_key_vault_access_policy" "service_principal_access_policy" {
  key_vault_id = azurerm_key_vault.vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  secret_permissions = [
    "Get", "Set", "Purge"
  ]

  key_permissions = [
    "Create", "Get", "Delete", "List", "WrapKey", "UnwrapKey", "Encrypt", "Decrypt", "Purge", "Recover"
  ]
}

resource "azurerm_key_vault_secret" "jwt_secret" {
  name         = "jwt-secret"
  value        = random_string.jwt_secret.result
  key_vault_id = azurerm_key_vault.vault.id

  lifecycle {
    ignore_changes = [value]
  }

  depends_on = [azurerm_key_vault_access_policy.service_principal_access_policy]
}

resource "azurerm_key_vault_access_policy" "function_app_access_policy" {
  key_vault_id = azurerm_key_vault.vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_function_app.function_app.identity[0].principal_id

  secret_permissions = [
    "Get", "List"
  ]

  key_permissions = [
    "Create", "Get", "List", "UnwrapKey", "Encrypt", "Decrypt"
  ]
}

resource "azurerm_key_vault_access_policy" "malthe_access_policy" {
  key_vault_id = azurerm_key_vault.vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = "c03dac5c-954a-4938-8117-33b7d56b86d6"

  secret_permissions = [
    "Get", "Set", "Delete", "List", "Purge", "Recover", "Restore"
  ]

  key_permissions = [
    "Create", "Get", "Delete", "List", "WrapKey", "UnwrapKey", "Encrypt", "Decrypt", "Purge", "Recover"
  ]
}