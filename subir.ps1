# Script para subir cambios a GitHub automáticamente
param(
    [string]$mensaje = "Actualización automática"
)

# Ver cambios
git status

# Agregar todos los cambios
git add .

# Hacer commit con el mensaje ingresado o por defecto
git commit -m "$mensaje"

# Subir cambios
git push

Write-Host "✅ Cambios subidos a GitHub con éxito." -ForegroundColor Green
