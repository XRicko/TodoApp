#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["src/ToDoList.MvcClient/ToDoList.MvcClient.csproj", "src/ToDoList.MvcClient/"]
COPY ["src/ToDoList.SharedKernel/ToDoList.SharedKernel.csproj", "src/ToDoList.SharedKernel/"]
COPY ["src/ToDoList.Extensions/ToDoList.Extensions.csproj", "src/ToDoList.Extensions/"]
RUN dotnet restore "src/ToDoList.MvcClient/ToDoList.MvcClient.csproj"
COPY . .
WORKDIR "/src/ToDoList.MvcClient"
RUN dotnet build "ToDoList.MvcClient.csproj" -c Release -o /app/buildz

FROM build AS publish
RUN dotnet publish "ToDoList.MvcClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoList.MvcClient.dll"]
