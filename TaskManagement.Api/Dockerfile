### Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração...
##
### Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
### IMAGEM BASE LINUX: .NET 9.0 no Ubuntu 22.04 (Jammy)
##FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy-chiseled AS base
##WORKDIR /app
##EXPOSE 8080
##EXPOSE 8081
##
##
### Esta fase é usada para compilar o projeto de serviço
### IMAGEM SDK LINUX: .NET 9.0 no Ubuntu 22.04 (Jammy)
##FROM mcr.microsoft.com/dotnet/sdk:9.0-jammy AS build
##ARG BUILD_CONFIGURATION=Release
##WORKDIR /src
### Adicione todos os projetos de sua solução para o restore
##COPY ["TaskManagement.Api/TaskManagement.Api.csproj", "TaskManagement.Api/"]
###COPY ["TaskManagement.Application/TaskManagement.Application.csproj", "TaskManagement.Application/"]
###COPY ["TaskManagement.Domain/TaskManagement.Domain.csproj", "TaskManagement.Domain/"]
###COPY ["TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj", "TaskManagement.Infrastructure/"]
###RUN dotnet restore "./TaskManagement.Api/TaskManagement.Api.csproj"
##COPY . .
##WORKDIR "/src/TaskManagement.Api"
##RUN dotnet build "./TaskManagement.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/build
##
### Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
##FROM build AS publish
##ARG BUILD_CONFIGURATION=Release
##RUN dotnet publish "./TaskManagement.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
##
### Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
##FROM base AS final
##WORKDIR /app
##COPY --from=publish /app/publish .
##ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]
#
#
## Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.
#
## Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
## Esta fase é usada para compilar o projeto de serviço
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["TaskManagement.Api/TaskManagement.Api.csproj", "TaskManagement.Api/"]
##COPY ["TaskManagement.Application/TaskManagement.Application.csproj", "TaskManagement.Application/"]
##COPY ["TaskManagement.Domain/TaskManagement.Domain.csproj", "TaskManagement.Domain/"]
##COPY ["TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj", "TaskManagement.Infrastructure/"]
#RUN dotnet restore "./TaskManagement.Api/TaskManagement.Api.csproj"
#COPY . .
#WORKDIR "/src/TaskManagement.Api"
#RUN dotnet build "./TaskManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./TaskManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]
#
#


# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TaskManagement.Api/TaskManagement.Api.csproj", "TaskManagement.Api/"]
RUN dotnet restore "TaskManagement.Api/TaskManagement.Api.csproj"
COPY . .
WORKDIR "/src/TaskManagement.Api"
RUN dotnet publish "TaskManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]
