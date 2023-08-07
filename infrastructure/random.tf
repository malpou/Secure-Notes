resource "random_string" "jwt_secret" {
  length  = 32
  special = true
  upper   = true
  numeric = true
}