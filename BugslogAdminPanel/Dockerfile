# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5001


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BugslogAdminPanel/BugslogAdminPanel.csproj", "BugslogAdminPanel/"]
RUN dotnet restore "./BugslogAdminPanel/BugslogAdminPanel.csproj"
COPY . .
WORKDIR "/src/BugslogAdminPanel"
RUN dotnet build "./BugslogAdminPanel.csproj" -c $BUILD_CONFIGURATION -o /app/build

#Добавить сертификат SSL
EXPOSE 80
EXPOSE 443
EXPOSE 5001
EXPOSE 8081
WORKDIR /app
ENV ASPNETCORE_URLS="https://+:5001;http://+:80"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/certs/my-certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="Password123"
WORKDIR "/src/BugslogAdminPanel"

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BugslogAdminPanel.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BugslogAdminPanel.dll"]