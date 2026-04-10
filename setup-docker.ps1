# ================================
# Setup completo Docker (PowerShell)
# ================================

Write-Host "Iniciando setup do Docker..." -ForegroundColor Cyan

# ================================
# 1. Criar docker-compose.yml
# ================================

$dockerComposePath = "docker-compose.yml"

$dockerComposeContent = @"
version: "3.9"

services:
  postgres:
    image: postgres:17
    container_name: clinic-saas-postgres
    environment:
      POSTGRES_DB: clinicsaasdb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - clinicsaas_pgdata:/var/lib/postgresql/data

volumes:
  clinicsaas_pgdata:
"@

Set-Content -Path $dockerComposePath -Value $dockerComposeContent -Encoding UTF8

Write-Host "docker-compose.yml criado!" -ForegroundColor Green

# ================================
# 2. Criar script create-docker.ps1
# ================================

$createScriptPath = "create-docker.ps1"

$createScriptContent = @"
Write-Host "Subindo container PostgreSQL..." -ForegroundColor Yellow
docker compose up -d

Write-Host "Containers ativos:" -ForegroundColor Cyan
docker ps
"@

Set-Content -Path $createScriptPath -Value $createScriptContent -Encoding UTF8

Write-Host "create-docker.ps1 criado!" -ForegroundColor Green

# ================================
# 3. Permissão de execução (opcional)
# ================================

Write-Host "Configurando política de execução (se necessário)..." -ForegroundColor Yellow
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force

Write-Host "Setup concluído com sucesso!" -ForegroundColor Green