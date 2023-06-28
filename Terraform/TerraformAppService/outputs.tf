output "webapp_plan" {
  value = azurerm_service_plan.appserviceplan.name
}

output "webapp" {
  value = azurerm_linux_web_app.webapp.name
}