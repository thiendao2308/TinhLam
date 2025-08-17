# Sử dụng .NET 8.0 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file và restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY . ./

# Build ứng dụng
RUN dotnet build -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish -c Release -o /app/publish

# Sử dụng .NET 8.0 Runtime để chạy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files từ build stage
COPY --from=build /app/publish .

# Expose port 8080 (Render yêu cầu)
EXPOSE 8080

# Set environment variable để chạy trên port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "TinhLam.dll"]
