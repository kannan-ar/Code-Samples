resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_service_plan" "appserviceplan" {
  name                = "webapp-plan-${random_integer.ri.result}"
  location            = var.resource_location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "webapp" {
  name                  = "webapp-${random_integer.ri.result}"
  location              = var.resource_location
  resource_group_name   = var.resource_group_name
  service_plan_id       = azurerm_service_plan.appserviceplan.id
  https_only            = true
  site_config { 
    minimum_tls_version = "1.2"
  }
}