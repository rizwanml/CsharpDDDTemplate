FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

# Copy project files
COPY ["src/SmallService.API/SmallService.API.csproj", "SmallService.API/"]
COPY ["src/SmallService.Domain/SmallService.Domain.csproj", "SmallService.Domain/"]
COPY ["src/SmallService.Infrastructure/SmallService.Infrastructure.csproj", "SmallService.Infrastructure/"]
COPY ["src/SmallService.Shared/SmallService.Shared.csproj", "SmallService.Shared/"]
COPY ["src/SmallService.API.Tests/SmallService.API.Tests.csproj", "SmallService.API.Tests/"]
COPY ["src/SmallService.Domain.Tests/SmallService.Domain.Tests.csproj", "SmallService.Domain.Tests/"]
COPY ["src/SmallService.Integration.Tests/SmallService.Integration.Tests.csproj", "SmallService.Integration.Tests/"]

# Restore dependencies
RUN dotnet restore "SmallService.API/SmallService.API.csproj"

# Copy source required
COPY src/SmallService.API/. SmallService.API/
COPY src/SmallService.Domain/. SmallService.Domain/
COPY src/SmallService.Infrastructure/. SmallService.Infrastructure/
COPY src/SmallService.Shared/. SmallService.Shared/

# Build projects
WORKDIR "/src/SmallService.API.Tests"
RUN dotnet build SmallService.API.Tests.csproj -c Release -o /app/test
WORKDIR "/src/SmallService.Domain.Tests"
RUN dotnet build SmallService.Domain.Tests.csproj -c Release  -o /app/test
WORKDIR "/src/SmallService.Integration.Tests"
RUN dotnet build SmallService.Integration.Tests.csproj -c Release  -o /app/test
WORKDIR "/src/SmallService.API"
RUN dotnet build "SmallService.API.csproj" --no-restore -c Release -o /app/build

FROM build AS ci-test
WORKDIR /app/test
RUN dotnet test SmallService.API.Tests.dll --no-build --no-restore --logger:trx
RUN dotnet test SmallService.Domain.Tests.dll  --no-build --no-restore --logger:trx
RUN dotnet test SmallService.Integration.Tests.dll --no-build --no-restore --logger:trx

FROM build AS publish
RUN dotnet publish "SmallService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
ENV ASPNETCORE_URLS=http://*:80
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "SmallService.API.dll"]