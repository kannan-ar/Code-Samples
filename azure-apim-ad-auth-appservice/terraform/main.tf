data "azurerm_resource_group" "rg" {
  name = "rg"
}

resource "azurerm_api_management" "api" {
  name                = var.api_management_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  publisher_email     = var.publisher_email
  publisher_name      = var.publisher_name
  sku_name            = "${var.sku}_${var.sku_count}"
}

/*
resource "azurerm_service_plan" "app_service" {
  name                = var.app_service_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type = "Linux"
  sku_name = "B1"
}

resource "azuread_application" "aad_app" {
  display_name = var.aad_application_name
  api {
    oauth2_permission_scope {
      admin_consent_description  = "Allow the application to access the API."
      admin_consent_display_name = "Access API"
      enabled                    = true
      id                         = uuid()
      value                      = "access_as_user"
    }
  }
}

resource "azuread_application_password" "aad_app_password" {
  application_id = azuread_application.aad_app.id
}

resource "azuread_service_principal" "aad_service_principal" {
  client_id = azuread_application.aad_app.client_id
  app_role_assignment_required = false
}

resource "azuread_application_redirect_uris" "redirect_uri" {
  application_id = azuread_application.aad_app.id
  type = "Web"
  redirect_uris = [
    "https://${azurerm_api_management.apim.gateway_url}/signin-oauth"
  ]
}


resource "azurerm_api_management" "apim" {
  name                = var.api_management_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  publisher_email     = "kannanar@outlook.com"
  publisher_name      = "API Publisher"
  sku_name            = "Developer_1"
}

resource "azurerm_api_management_api" "apim_api" {
  name                = "ar-api"
  resource_group_name = azurerm_resource_group.rg.name
  api_management_name = azurerm_api_management.apim.name
  revision            = "1"
  display_name        = "AR API"
  path                = "ar"
  protocols           = ["https"]
}

resource "azurerm_api_management_authorization_server" "auth_server" {
  api_management_name = azurerm_api_management.apim.name
  resource_group_name = azurerm_resource_group.rg.name
  name                = "azuread"
  display_name        = "Azure AD"
  client_registration_endpoint = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/oauth2/v2.0/authorize"
  authorization_endpoint       = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/oauth2/v2.0/authorize"
  token_endpoint               = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/oauth2/v2.0/token"
  client_id                    = azuread_application.aad_app.application_id
  client_secret                = azuread_application_password.aad_app_password.value
  grant_types                  = ["authorization_code"]
  default_scope                = "api://${azuread_application.aad_app.application_id}/access_as_user"
  authorization_methods = [ "GET", "POST" ]
}

resource "azurerm_api_management_api_policy" "apim_api_policy" {
  api_name             = azurerm_api_management_api.apim_api.name
  api_management_name  = azurerm_api_management.apim.name
  resource_group_name  = azurerm_resource_group.rg.name
  xml_content = <<XML
  <policies>
      <inbound>
          <base />
          <validate-jwt header-name="Authorization" failed-validation-httpcode="401" require-https="true">
              <openid-config url="https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/v2.0/.well-known/openid-configuration" />
              <audiences>
                  <audience>api://${azuread_application.aad_app.application_id}</audience>
              </audiences>
          </validate-jwt>
      </inbound>
      <backend>
          <base />
      </backend>
      <outbound>
          <base />
      </outbound>
  </policies>
  XML
}
*/