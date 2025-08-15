# Script interactivo para subir cambios a GitHub con fecha y hora
Write-Host "==== SUBIDA A GITHUB ====" -ForegroundColor Cyan

# Pedir mensaje al usuario
$mensaje = Read-Host "Escribe el mensaje del commit"

# Obtener fecha y hora actual
$fechaHora = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Combinar mensaje con fecha y hora
$commitMsg = "$mensaje (Subido el $fechaHora)"

# Ver cambios
git status

# Agregar todos los cambios
git add .

# Hacer commit con mensaje y fecha
git commit -m "$commitMsg"

# Subir cambios
git push

Write-Host "✅ Cambios subidos a GitHub con éxito el $fechaHora" -ForegroundColor Green
