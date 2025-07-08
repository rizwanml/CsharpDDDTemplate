FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

# Copy project files
COPY ["src/CqrsService.API/CqrsService.API.csproj", "CqrsService.API/"]
COPY ["src/CqrsService.Application/CqrsService.Application.csproj", "CqrsService.Application/"]
COPY ["src/CqrsService.Domain/CqrsService.Domain.csproj", "CqrsService.Domain/"]
COPY ["src/CqrsService.Infrastructure/CqrsService.Infrastructure.csproj", "CqrsService.Infrastructure/"]
COPY ["src/CqrsService.Shared/CqrsService.Shared.csproj", "CqrsService.Shared/"]
COPY ["src/CqrsService.API.Tests/CqrsService.API.Tests.csproj", "CqrsService.API.Tests/"]
COPY ["src/CqrsService.Application.Tests/CqrsService.Application.Tests.csproj", "CqrsService.Application.Tests/"]
COPY ["src/CqrsService.Domain.Tests/CqrsService.Domain.Tests.csproj", "CqrsService.Domain.Tests/"]
COPY ["src/CqrsService.Integration.Tests/CqrsService.Integration.Tests.csproj", "CqrsService.Integration.Tests/"]

# Restore dependencies
RUN dotnet restore "CqrsService.API/CqrsService.API.csproj"

# Copy source required
COPY src/CqrsService.API/. CqrsService.API/
COPY src/CqrsService.Application/. CqrsService.Application/
COPY src/CqrsService.Domain/. CqrsService.Domain/
COPY src/CqrsService.Infrastructure/. CqrsService.Infrastructure/
COPY src/CqrsService.Shared/. CqrsService.Shared/

# Build projects
WORKDIR "/src/CqrsService.API.Tests"
RUN dotnet build CqrsService.API.Tests.csproj -c Release -o /app/test
WORKDIR "/src/CqrsService.Application.Tests"
RUN dotnet build CqrsService.Application.Tests.csproj -c Release -o /app/test
WORKDIR "/src/CqrsService.Domain.Tests"
RUN dotnet build CqrsService.Domain.Tests.csproj -c Release  -o /app/test
WORKDIR "/src/CqrsService.Integration.Tests"
RUN dotnet build CqrsService.Integration.Tests.csproj -c Release  -o /app/test
WORKDIR "/src/CqrsService.API"
RUN dotnet build "CqrsService.API.csproj" --no-restore -c Release -o /app/build

FROM build AS ci-test
WORKDIR /app/test
RUN dotnet test CqrsService.API.Tests.dll --no-build --no-restore --logger:trx
RUN dotnet test CqrsService.Application.Tests.dll --no-build --no-restore --logger:trx
RUN dotnet test CqrsService.Domain.Tests.dll  --no-build --no-restore --logger:trx
RUN dotnet test CqrsService.Integration.Tests.dll --no-build --no-restore --logger:trx

FROM build AS publish
RUN dotnet publish "CqrsService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
ENV ASPNETCORE_URLS=http://*:80
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "CqrsService.API.dll"]