
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY . ./
RUN dotnet publish "CalendarDisplay.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalendarDisplay.dll"]
