# ============================================================
# BabyTurismo Backend – Dockerfile (ASP.NET Core 10)
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY BabyTurismo.sln .

COPY src/BabyTurismo.Api/BabyTurismo.Api.csproj src/BabyTurismo.Api/
COPY src/BabyTurismo.Application/BabyTurismo.Application.csproj src/BabyTurismo.Application/
COPY src/BabyTurismo.Domain/BabyTurismo.Domain.csproj src/BabyTurismo.Domain/
COPY src/BabyTurismo.Infrastructure/BabyTurismo.Infrastructure.csproj src/BabyTurismo.Infrastructure/
COPY src/BabyTurismo.Shared/BabyTurismo.Shared.csproj src/BabyTurismo.Shared/

COPY tests/BabyTurismo.Tests/BabyTurismo.Tests.csproj tests/BabyTurismo.Tests/


RUN dotnet restore BabyTurismo.sln


COPY . .


RUN dotnet build BabyTurismo.sln -c Release --no-restore


RUN dotnet publish src/BabyTurismo.Api/BabyTurismo.Api.csproj \
    -c Release \
    --no-build \
    -o /app/publish



# ============================================================
# Runtime
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app


RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && groupadd --gid 1001 babyturismo \
    && useradd --uid 1001 --gid 1001 --create-home babyturismo \
    && rm -rf /var/lib/apt/lists/*


COPY --from=build --chown=babyturismo:babyturismo /app/publish .


RUN mkdir -p /app/logs \
    && chown babyturismo:babyturismo /app/logs


USER babyturismo


EXPOSE 8080


ENTRYPOINT ["dotnet", "BabyTurismo.Api.dll"]