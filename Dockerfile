ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder AS deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.DepositAPI.Domain/Fanex.DepositAPI.Domain.csproj", "./Fanex.DepositAPI.Domain/"]
RUN dotnet restore "Fanex.DepositAPI.Domain/Fanex.DepositAPI.Domain.csproj" -v n -r linux-x64

COPY ["src/Fanex.DepositAPI.Application/Fanex.DepositAPI.Application.csproj", "./Fanex.DepositAPI.Application/"]
RUN dotnet restore "Fanex.DepositAPI.Application/Fanex.DepositAPI.Application.csproj" -v n -r linux-x64

COPY ["src/Fanex.DepositAPI.Infrastructure/Fanex.DepositAPI.Infrastructure.csproj", "./Fanex.DepositAPI.Infrastructure/"]
RUN dotnet restore "Fanex.DepositAPI.Infrastructure/Fanex.DepositAPI.Infrastructure.csproj" -v n -r linux-x64

COPY ["src/Fanex.DepositAPI.Presentation/Fanex.DepositAPI.Presentation.csproj", "./Fanex.DepositAPI.Presentation/"]
RUN dotnet restore "Fanex.DepositAPI.Presentation/Fanex.DepositAPI.Presentation.csproj" -v n -r linux-x64

COPY ["src/Fanex.DepositAPI.WebApi/Fanex.DepositAPI.WebApi.csproj", "./Fanex.DepositAPI.WebApi/"]
RUN dotnet restore "Fanex.DepositAPI.WebApi/Fanex.DepositAPI.WebApi.csproj" -v n -r linux-x64

COPY src .

RUN dotnet publish "Fanex.DepositAPI.WebApi/Fanex.DepositAPI.WebApi.csproj" -c Release -o /publish -r linux-x64 \
    --use-current-runtime \
    --self-contained false

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS final

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

# Dont run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app

COPY --from=builder /publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.DepositAPI.WebApi.dll"]
