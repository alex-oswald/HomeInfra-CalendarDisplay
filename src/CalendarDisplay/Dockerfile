
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG BUILDPLATFORM=linux/arm64
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /source
COPY src .
RUN dotnet restore "CalendarDisplay/CalendarDisplay.csproj"
RUN dotnet build "CalendarDisplay/CalendarDisplay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CalendarDisplay/CalendarDisplay.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalendarDisplay.dll"]
