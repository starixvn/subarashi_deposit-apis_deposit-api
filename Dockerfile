FROM hub.nexdev.net/base/dotnet/core/sdk:8.0 AS builder
ARG TRANSFORM_CONFIG

WORKDIR /src

COPY NuGet.Config .
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

COPY src/Fanex.DepositAPI.Application/Fanex.DepositAPI.Application.csproj ./Fanex.DepositAPI.Application/
RUN dotnet restore Fanex.DepositAPI.Application/Fanex.DepositAPI.Application.csproj -v n

COPY src/Fanex.DepositAPI.Domain/Fanex.DepositAPI.Domain.csproj ./Fanex.DepositAPI.Domain/
RUN dotnet restore Fanex.DepositAPI.Domain/Fanex.DepositAPI.Domain.csproj -v n

COPY src/Fanex.DepositAPI.Infrastructure/Fanex.DepositAPI.Infrastructure.csproj ./Fanex.DepositAPI.Infrastructure/
RUN dotnet restore Fanex.DepositAPI.Infrastructure/Fanex.DepositAPI.Infrastructure.csproj -v n

COPY src/Fanex.DepositAPI.Presentation/Fanex.DepositAPI.Presentation.csproj ./Fanex.DepositAPI.Presentation/
RUN dotnet restore Fanex.DepositAPI.Presentation/Fanex.DepositAPI.Presentation.csproj -v n

COPY src/Fanex.DepositAPI.WebApi/Fanex.DepositAPI.WebApi.csproj ./Fanex.DepositAPI.WebApi/
RUN dotnet restore Fanex.DepositAPI.WebApi/Fanex.DepositAPI.WebApi.csproj -v n

COPY src .

WORKDIR /src/Fanex.DepositAPI.WebApi

RUN dotnet publish "Fanex.DepositAPI.WebApi.csproj" -c Release -o /publish \
    --use-current-runtime \
    --self-contained false

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine

ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000


WORKDIR /app

COPY --from=builder /publish .

ENTRYPOINT ["dotnet", "Fanex.DepositAPI.WebApi.dll"]
