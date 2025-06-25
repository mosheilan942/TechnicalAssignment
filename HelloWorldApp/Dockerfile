FROM mcr.microsoft.com/dotnet/sdk:8.0.14 AS build
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0.14
WORKDIR /App

RUN apt-get update && apt-get upgrade -y \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "HelloWorldApp.dll"]