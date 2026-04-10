Write-Host "Subindo container PostgreSQL..." -ForegroundColor Yellow
docker compose up -d

Write-Host "Containers ativos:" -ForegroundColor Cyan
docker ps
