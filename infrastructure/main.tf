terraform {
  cloud {
    organization = "malpou"

    workspaces {
      name = "SecureNotes"
    }
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.54.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.0"
    }
  }
}

data "azurerm_client_config" "current" {}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "secure-notes"
  location = "northeurope"
}