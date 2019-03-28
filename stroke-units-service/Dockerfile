FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["stroke-units-service/stroke-units-service.csproj", "stroke-units-service/"]
RUN dotnet restore "stroke-units-service/stroke-units-service.csproj"
COPY . .
WORKDIR "/src/stroke-units-service"
RUN dotnet build "stroke-units-service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "stroke-units-service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "stroke-units-service.dll"]