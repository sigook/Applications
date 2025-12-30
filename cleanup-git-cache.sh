#!/bin/bash
# Script para limpiar archivos trackeados que ahora están en .gitignore
# ⚠️ IMPORTANTE: Haz commit de todos tus cambios antes de ejecutar este script

echo "============================================"
echo "Limpieza de Git Cache"
echo "============================================"
echo ""
echo "⚠️  Este script va a:"
echo "   1. Remover del índice de Git todos los archivos que están en .gitignore"
echo "   2. Mantener los archivos localmente (no los borra)"
echo "   3. Preparar un commit para remover estos archivos del repositorio"
echo ""
read -p "¿Continuar? (y/n): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Cancelado."
    exit 1
fi

echo ""
echo "📋 Paso 1: Verificando estado de Git..."
if ! git diff-index --quiet HEAD --; then
    echo "❌ ERROR: Tienes cambios sin commitear."
    echo "Por favor, haz commit o stash de tus cambios primero:"
    echo "  git add ."
    echo "  git commit -m 'Save work before cleanup'"
    echo ""
    echo "O usa stash:"
    echo "  git stash"
    exit 1
fi

echo "✅ No hay cambios pendientes"
echo ""

echo "📋 Paso 2: Removiendo archivos del índice de Git..."
echo "Esto puede tomar un momento..."
echo ""

# Remover todos los archivos del índice (cached) pero mantenerlos localmente
git rm -r --cached .

echo ""
echo "📋 Paso 3: Re-agregando archivos respetando .gitignore..."
git add .

echo ""
echo "📋 Paso 4: Mostrando archivos que serán removidos..."
echo ""

# Mostrar estadísticas
DELETED=$(git status --short | grep "^D " | wc -l)
MODIFIED=$(git status --short | grep "^M " | wc -l)

echo "============================================"
echo "Resumen de cambios:"
echo "============================================"
echo "Archivos que serán eliminados del repo: $DELETED"
echo "Archivos modificados: $MODIFIED"
echo ""

if [ $DELETED -eq 0 ]; then
    echo "✅ No hay archivos para limpiar. El repositorio ya está limpio!"
    git reset
    exit 0
fi

echo "Ejemplos de archivos que se eliminarán del repositorio:"
git status --short | grep "^D " | head -20
echo ""
echo "============================================"
echo ""
echo "⚠️  Estos archivos se eliminarán del repositorio pero"
echo "    se mantendrán en tu directorio local."
echo ""
read -p "¿Crear commit con estos cambios? (y/n): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Cancelado. Deshaciendo cambios..."
    git reset
    exit 1
fi

echo ""
echo "📋 Paso 5: Creando commit..."
git commit -m "chore: remove tracked files that are now in .gitignore

Remove build artifacts, IDE files, and other files that should not be tracked:
- Visual Studio files (.vs/, bin/, obj/)
- Node.js files (node_modules/, dist/, wwwroot/)
- IDE configuration files
- OS-specific files
- Build artifacts

These files are now properly ignored via .gitignore"

echo ""
echo "============================================"
echo "✅ Limpieza completada!"
echo "============================================"
echo ""
echo "Próximos pasos:"
echo "1. Revisa el commit con: git show"
echo "2. Si todo se ve bien, haz push: git push origin dev"
echo "3. Otros desarrolladores deberán hacer: git pull"
echo ""
echo "Nota: Los archivos siguen existiendo localmente,"
echo "      solo se removieron del repositorio Git."
echo "============================================"
