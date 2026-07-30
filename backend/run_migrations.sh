#!/bin/bash
set -e
echo "Installing dotnet-ef..."
dotnet tool install --global dotnet-ef
export PATH="$PATH:/root/.dotnet/tools"

echo "Restoring packages..."
dotnet restore BabyTurismo.sln

echo "Running migrations..."
dotnet ef migrations add InitialCore --project src/BabyTurismo.Infrastructure --startup-project src/BabyTurismo.Api
