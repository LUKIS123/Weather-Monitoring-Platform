terraform {
  required_version = ">= 1.9.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.6.0"
    }
  }
}

provider "azurerm" {
  # resource_provider_registrations = "none"
  features {}
  subscription_id = var.subscription_id
}
