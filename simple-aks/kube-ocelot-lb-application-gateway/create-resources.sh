LOCATION='centralindia'
RG_NAME='ar-rg'
VNET_NAME='ar-vnet'
VNET_PREFIX='10.1.0.0/16'
SUBNET_NAME='ar-subnet'
SUBNET_PREFIX='10.1.1.0/24'
AKS_CLUSTER_NAME='ar-cluster'
ACR_NAME=arkuberegistry

echo "Creating resource group"

az group create --name $RG_NAME --location $LOCATION

echo "Creating VNet"

az network vnet create --name $VNET_NAME --resource-group $RG_NAME --address-prefixes $VNET_PREFIX --location $LOCATION

echo "Creating Sub Net"

az network vnet subnet create --resource-group $RG_NAME --vnet-name $VNET_NAME --name $SUBNET_NAME --address-prefixes $SUBNET_PREFIX

SUBNET_ID=$(az network vnet subnet show --resource-group $RG_NAME --vnet-name $VNET_NAME --name $SUBNET_NAME --query id -o tsv)

echo $SUBNET_ID

export MSYS_NO_PATHCONV=1

echo "Creating Kubernetes cluster"

az aks create --resource-group $RG_NAME --name $AKS_CLUSTER_NAME --location $LOCATION --node-count 1 --node-vm-size "Standard_B2s" --network-plugin azure --vnet-subnet-id $SUBNET_ID --generate-ssh-keys

echo "Creating container registry"

az acr create --resource-group $RG_NAME  --name $ACR_NAME --sku Basic