#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["src/ToDoList.WebApi/ToDoList.WebApi.csproj", "src/ToDoList.WebApi/"]
COPY ["src/ToDoList.Infrastructure/ToDoList.Infrastructure.csproj", "src/ToDoList.Infrastructure/"]
COPY ["src/ToDoList.SharedKernel/ToDoList.SharedKernel.csproj", "src/ToDoList.SharedKernel/"]
COPY ["src/ToDoList.Core/ToDoList.Core.csproj", "src/ToDoList.Core/"]
COPY ["src/ToDoList.Extensions/ToDoList.Extensions.csproj", "src/ToDoList.Extensions/"]
RUN dotnet restore "src/ToDoList.WebApi/ToDoList.WebApi.csproj"
COPY . .
WORKDIR "/src/ToDoList.WebApi"
RUN dotnet build "ToDoList.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDoList.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoList.WebApi.dll"]
