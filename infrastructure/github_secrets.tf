resource "github_actions_secret" "update_function_app_name_secret" {
  repository      = "Secure-Notes"
  secret_name     = "SECRET_NOTES_FUNCTION_APP_NAME"
  plaintext_value = azurerm_linux_function_app.function_app.name
}