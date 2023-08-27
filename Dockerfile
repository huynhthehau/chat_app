FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App
EXPOSE 80
EXPOSE 433
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore

RUN dotnet publish -c Release -o out
# RUN dotnet tool install --global dotnet-ef --version 6.0.0
# ENV PATH="$PATH:/root/.dotnet/tools"
# RUN dotnet ef migrations add InitialCreate
# RUN dotnet ef database update
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "WebChatApp.dll"]
