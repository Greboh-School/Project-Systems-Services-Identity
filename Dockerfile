FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443
ARG NUGET_USERNAME
ARG NUGET_TOKEN
ARG version

COPY ["School.Project.Systems.Services.Identity/School.Project.Systems.Services.Identity.csproj", "School.Project.Systems.Services.Identity/"]
COPY ["NuGet.config", "School.Project.Systems.Services.Identity/"]

RUN dotnet restore "School.Project.Systems.Services.Identity/School.Project.Systems.Services.Identity.csproj" --configfile School.Project.Systems.Services.Identity/NuGet.config

COPY . .

RUN dotnet publish "School.Project.Systems.Services.Identity/School.Project.Systems.Services.Identity.csproj" -c Release -o out /p:Version=$version

FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app

COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "School.Project.Systems.Services.Identity.dll"]
