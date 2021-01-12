FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["./*", "MasterDataService/"]
RUN dotnet restore "MasterDataService/MasterDataService.csproj"
#COPY . .
WORKDIR "/src/MasterDataService"
RUN dotnet build "MasterDataService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MasterDataService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENV ASPNETCORE_URLS http://+:8002
EXPOSE 80
ENTRYPOINT ["dotnet", "MasterDataService.dll"]
