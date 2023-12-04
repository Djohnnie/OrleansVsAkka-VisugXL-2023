FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["OrleansVsAkka.VisugXL.Orleans.Example.Web/OrleansVsAkka.VisugXL.Orleans.Example.Web.csproj", "OrleansVsAkka.VisugXL.Orleans.Example.Web/"]
COPY ["OrleansVsAkka.VisugXL.Orleans.Example.Common/OrleansVsAkka.VisugXL.Orleans.Example.Common.csproj", "OrleansVsAkka.VisugXL.Orleans.Example.Common/"]
RUN dotnet restore "OrleansVsAkka.VisugXL.Orleans.Example.Web/OrleansVsAkka.VisugXL.Orleans.Example.Web.csproj"
COPY . .
WORKDIR "/src/OrleansVsAkka.VisugXL.Orleans.Example.Web"
RUN dotnet build "OrleansVsAkka.VisugXL.Orleans.Example.Web.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OrleansVsAkka.VisugXL.Orleans.Example.Web.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrleansVsAkka.VisugXL.Orleans.Example.Web.dll"]
