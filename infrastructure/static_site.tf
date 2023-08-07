resource "azurerm_static_site" "static_site" {
  name                = "secure-notes-static-site"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "westeurope"
  sku_tier            = "Free"
  sku_size            = "Free"
}