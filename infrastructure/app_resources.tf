resource "azurerm_application_insights" "app_insights" {
  name                = "secure-notes-app-insights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_service_plan" "service_plan" {
  name                = "secure-notes-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "Y1"
}

resource "azurerm_linux_function_app" "function_app" {
  name                = "secure-notes-function-app"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  service_plan_id = azurerm_service_plan.service_plan.id

  app_settings = {
    "KeyVaultUrl" = azurerm_key_vault.vault.vault_uri
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE" = "true"
  }

  site_config {
    application_insights_connection_string = azurerm_application_insights.app_insights.connection_string
    application_insights_key = azurerm_application_insights.app_insights.instrumentation_key

    cors {
      allowed_origins = [
        "https://secure-notes.net", 
        "http://localhost:5173"
      ]
    }

    application_stack {
      dotnet_version              = "7.0"
      use_dotnet_isolated_runtime = true
    }
  }

  identity {
    type = "SystemAssigned"
  }
}