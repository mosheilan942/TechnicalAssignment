FROM bitnami/dotnet-sdk:8.0.408 AS build
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -o out

FROM bitnami/aspnet-core:8.0.15 AS final
WORKDIR /App

RUN apt-get update && apt-get upgrade -y \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "HelloWorldApp.dll"]