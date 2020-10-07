FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .

# Set env vars
# ENV AWS_ACCESS_KEY_ID AKIAWYU7YIH4ZUNUOI62
# ENV AWS_SECRET_ACCESS_KEY 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
ENV AWS_DEFAULT_REGION us-east-1

ENTRYPOINT ["dotnet", "DataHubFileService.dll"]

