FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Client/Client.csproj", "Client/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "Client/Client.csproj"
COPY . .
WORKDIR "/src/Client"
RUN dotnet publish "./Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]