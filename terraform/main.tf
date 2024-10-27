resource "azurerm_resource_group" "db_rg" {
  name     = var.db_resource_group_name
  location = var.db_resource_group_location
}

# SQL Server
resource "azurerm_mssql_server" "weather_monitor_sql" {
  name                          = var.sql_server_name
  resource_group_name           = azurerm_resource_group.db_rg.name
  location                      = azurerm_resource_group.db_rg.location
  version                       = "12.0"
  administrator_login           = var.sql_admin_username
  administrator_login_password  = var.sql_admin_password
  public_network_access_enabled = true
}

# SQL Database
resource "azurerm_mssql_database" "weather_monitor_database" {
  name      = var.sql_database_name
  server_id = azurerm_mssql_server.weather_monitor_sql.id

  sku_name    = var.sql_database_sku
  max_size_gb = var.sql_database_max_size_gb
}

data "azurerm_resource_group" "app_rg" {
  name = var.resource_group_name
}

# App Service for Weather Monitor Web App
resource "azurerm_service_plan" "weather_monitor_client_app_plan" {
  name                = "weather-monitor-client-free-plan"
  location            = data.azurerm_resource_group.app_rg.location
  resource_group_name = data.azurerm_resource_group.app_rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "weather_monitor_client_app_service" {
  name                = "weather-monitor"
  location            = data.azurerm_resource_group.app_rg.location
  resource_group_name = data.azurerm_resource_group.app_rg.name
  service_plan_id     = azurerm_service_plan.weather_monitor_client_app_plan.id

  site_config {
    always_on = false
  }

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
  }
}

# App Service for Core Microservice
resource "azurerm_service_plan" "weather_monitor_microservice_plan" {
  name                = "weather-monitor-core-microservice-plan"
  location            = data.azurerm_resource_group.app_rg.location
  resource_group_name = data.azurerm_resource_group.app_rg.name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "weather_monitor_core_app_service" {
  name                = "weather-monitor-core"
  location            = data.azurerm_resource_group.app_rg.location
  resource_group_name = data.azurerm_resource_group.app_rg.name
  service_plan_id     = azurerm_service_plan.weather_monitor_microservice_plan.id

  site_config {
    always_on = true
  }

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
  }
}

# Container App for MQTT Broker
resource "azurerm_container_app_environment" "weather_monitor_containers_env" {
  name                = "weather-monitor-broker-container-env"
  location            = data.azurerm_resource_group.app_rg.location
  resource_group_name = data.azurerm_resource_group.app_rg.name

  # This section assumes a default subnet is used. Update this based on your network settings.
  # subnet_id = azurerm_subnet.example_subnet.id
}

# Azure Container App
resource "azurerm_container_app" "weather_monitor_container_app" {
  name                         = "weather-monitor-container-app"
  resource_group_name          = data.azurerm_resource_group.app_rg.name
  container_app_environment_id = azurerm_container_app_environment.weather_monitor_containers_env.id
  revision_mode                = "Single"

  template {
    container {
      name   = "weather-monitor-container"
      image  = "lukis123/mosquitto-go-auth-custom:23092024"
      cpu    = "0.25"
      memory = "0.5Gi"

      env {
        name  = "WEATHER_MONITOR_ENV"
        value = "production"
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 1883
    transport        = "auto"

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}

# name: The name of the container app.
# image: Replace "dockerhubusername/your-docker-image:latest" with your actual Docker Hub image (for example: username/weather-monitor:latest).
# CPU and Memory: Set to 0.25 vCPU and 128Mi memory, slightly more than the required 10.15MiB to give some buffer.
# Scaling: Auto-scaling is set based on HTTP concurrent requests, with a minimum of 1 replica and a maximum of 2 replicas.
# Ingress: Configured to expose the container to the internet on port 80.
# Notes:
# Make sure to replace the placeholder values for Docker Hub (dockerhubusername/your-docker-image) with your actual username and image name.
# If you're using a specific virtual network/subnet for your Azure resources, update the subnet_id in the container environment resource.
# The scaling rule uses HTTP requests for auto-scaling, but you can adjust it based on other requirements such as CPU or memory usage.
