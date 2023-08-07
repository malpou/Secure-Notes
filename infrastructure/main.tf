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
    github = {
      source  = "integrations/github"
      version = "4.5.2"
    }
  }
}

data "azurerm_client_config" "current" {}

provider "azurerm" {
  features {}
}

provider "github" {
  token = var.github_token
}

resource "azurerm_resource_group" "rg" {
  name     = "secure-notes"
  location = "northeurope"
}