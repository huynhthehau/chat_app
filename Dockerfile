FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App
EXPOSE 80/tcp
EXPOSE 433/tcp
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore

RUN dotnet publish -c Release -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "WebChatApp.dll"]
