FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ps-coding-challenge.sln .
COPY ps-coding-challenge/ps-coding-challenge.csproj ps-coding-challenge/ps-coding-challenge.csproj
COPY Model/Models.csproj Model/Models.csproj
COPY Repositories/Repositories.csproj Repositories/Repositories.csproj
COPY Services/Services.csproj Services/Services.csproj
COPY Tests.Services/Tests.Services.csproj Tests.Services/Tests.Services.csproj
COPY Utilities/Common.csproj Utilities/Common.csproj
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish ps-coding-challenge -c Release -o out
RUN dotnet build Model
RUN dotnet build Repositories
RUN dotnet build Services
RUN dotnet build Tests.Services
RUN dotnet build Utilities

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build-env /app/out/ .
CMD ["dotnet", "ps-coding-challenge.dll"]