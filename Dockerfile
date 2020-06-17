FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app    
EXPOSE 80

FROM  mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src 
COPY ["Main/Main.csproj", "./Main/"]
COPY ["Authentication/Authentication.csproj", "./Authentication/"]
COPY ["Models/Models.csproj", "./Models/"]
COPY ["Persistence/Persistence.csproj", "./Persistence/"]
COPY ["Export/Export.csproj", "./Export/"]
RUN dotnet restore "Main/Main.csproj"


COPY . .
WORKDIR "/src/."
RUN dotnet build "Main/Main.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Main/Main.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Main.dll"]






