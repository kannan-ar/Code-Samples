resource "azurerm_resource_group" "ar-rg" {
  name     = "ar-rg"
  location = "Central India"
}

resource "azurerm_servicebus_namespace" "ar-bus" {
  name                = "ar-servicebus-namespace"
  location            = azurerm_resource_group.ar-rg.location
  resource_group_name = azurerm_resource_group.ar-rg.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

output "bus_connection_string" {
  value = nonsensitive(azurerm_servicebus_namespace.ar-bus.default_primary_connection_string)
}