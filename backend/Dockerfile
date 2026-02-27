# Estágio 1: build
# O contexto Docker é a raiz do repositório (docker-compose define context: ..)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar apenas arquivos de projeto para restaurar dependências antes de copiar o código-fonte,
# aproveitando o cache de camadas do Docker quando só o código muda.
COPY backend/src/DiarioDeBordo.Domain/DiarioDeBordo.Domain.csproj                   backend/src/DiarioDeBordo.Domain/
COPY backend/src/DiarioDeBordo.Application/DiarioDeBordo.Application.csproj         backend/src/DiarioDeBordo.Application/
COPY backend/src/DiarioDeBordo.Infrastructure/DiarioDeBordo.Infrastructure.csproj   backend/src/DiarioDeBordo.Infrastructure/
COPY backend/src/DiarioDeBordo.Persistence/DiarioDeBordo.Persistence.csproj         backend/src/DiarioDeBordo.Persistence/
COPY backend/src/DiarioDeBordo.Api/DiarioDeBordo.Api.csproj                         backend/src/DiarioDeBordo.Api/
# Tests não fazem parte da imagem de runtime; o .sln referencia o projeto de testes,
# por isso copiamos o .csproj vazio para satisfazer o restore via solution.
COPY backend/src/DiarioDeBordo.Tests/DiarioDeBordo.Tests.csproj                     backend/src/DiarioDeBordo.Tests/
COPY backend/DiarioDeBordo.sln backend/

RUN dotnet restore backend/DiarioDeBordo.sln

# Copiar o restante do código-fonte e publicar
COPY backend/src/ backend/src/
RUN dotnet publish backend/src/DiarioDeBordo.Api/DiarioDeBordo.Api.csproj \
    -c Release \
    -o /app/publish

# Estágio 2: runtime (imagem mínima)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Porta padrão do Kestrel em containers ASP.NET Core 9+
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DiarioDeBordo.Api.dll"]
