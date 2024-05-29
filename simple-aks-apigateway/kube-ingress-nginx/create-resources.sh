LOCATION=centralindia
RG_NAME=ar-rg
ACR_NAME=arkuberegistry

echo "Creating resource group"

az group create --name=$RG_NAME --location=$LOCATION

echo "Resource group created"
echo "Creating container registry"

az acr create --resource-group $RG_NAME  --name $ACR_NAME --sku Basic

echo "Container registry created"
echo "Creating Kubernetes cluster"

az aks create --resource-group $RG_NAME --name arCluster --location $LOCATION --node-count 1 --generate-ssh-keys --node-vm-size "Standard_B2s" --network-plugin azure --attach-acr $ACR_NAME

echo "Kubernetes cluster created"