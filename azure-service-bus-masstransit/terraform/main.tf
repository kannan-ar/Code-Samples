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

resource "azurerm_servicebus_queue" "ar-bus-queue" {
  name         = "purchase-created"
  namespace_id = azurerm_servicebus_namespace.ar-bus.id
}

resource "azurerm_servicebus_topic" "ar-bus-topic" {
  name         = "ar_purchase_topic"
  namespace_id = azurerm_servicebus_namespace.ar-bus.id
}

output "bus_connection_string" {
  value = nonsensitive(azurerm_servicebus_namespace.ar-bus.default_primary_connection_string)
}