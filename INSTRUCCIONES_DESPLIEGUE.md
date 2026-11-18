# Instrucciones de Despliegue - ContaFacil para Windows

## 📦 Aplicación Publicada

Tu aplicación **ContaFacil** ha sido publicada exitosamente para Windows x64.

**Ubicación:** `g:\Wilson\Proyectos\NET CORE\Sistema Contable\conta_facil\ContaFacil\publish\win-x64`

## 🚀 Características de la Publicación

- ✅ **Self-contained**: Incluye el runtime de .NET 8.0 (no requiere instalación de .NET)
- ✅ **Single File**: Todo empaquetado en un único ejecutable
- ✅ **Optimizado**: Compilado en modo Release para máximo rendimiento
- ✅ **Tamaño**: ~190 MB (incluye todas las dependencias)

## 📋 Requisitos del Sistema

- **Sistema Operativo**: Windows 10/11 (64-bit)
- **Base de Datos**: MySQL/MariaDB (debe estar instalado y configurado)
- **Puertos**: Puerto 5000 (HTTP) o el configurado en appsettings.json

## 🔧 Pasos para Desplegar

### 1. Copiar los Archivos

Copia toda la carpeta `win-x64` a la ubicación deseada en el servidor o PC de producción:

```
C:\ContaFacil\
├── ContaFacil.exe
├── appsettings.json
├── appsettings.Development.json
├── web.config
├── wwwroot\
└── Controllers\
```

### 2. Configurar la Base de Datos

Edita el archivo `appsettings.json` y actualiza la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=TU_SERVIDOR;database=TU_BASE_DATOS;user=TU_USUARIO;password=TU_CONTRASEÑA"
  }
}
```

### 3. Configurar el Puerto y URLs

En `appsettings.json`, ajusta las URLs si es necesario:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

### 4. Ejecutar la Aplicación

#### Opción A: Ejecución Manual
Doble clic en `ContaFacil.exe` o desde CMD:

```cmd
cd C:\ContaFacil
ContaFacil.exe
```

#### Opción B: Como Servicio de Windows (Recomendado para Producción)

1. Instalar NSSM (Non-Sucking Service Manager):
   - Descargar de: https://nssm.cc/download
   - Extraer nssm.exe

2. Instalar el servicio:
```cmd
nssm install ContaFacil "C:\ContaFacil\ContaFacil.exe"
nssm set ContaFacil AppDirectory "C:\ContaFacil"
nssm set ContaFacil DisplayName "ContaFacil Sistema Contable"
nssm set ContaFacil Description "Sistema de Contabilidad ContaFacil"
nssm set ContaFacil Start SERVICE_AUTO_START
```

3. Iniciar el servicio:
```cmd
nssm start ContaFacil
```

### 5. Verificar la Instalación

Abre un navegador y accede a:
```
http://localhost:5000
```

## 🔒 Configuración de Seguridad

### Para Producción:

1. **HTTPS**: Configura un certificado SSL
2. **Firewall**: Abre solo los puertos necesarios
3. **Credenciales**: Cambia las contraseñas por defecto en appsettings.json
4. **Permisos**: Ejecuta el servicio con una cuenta de usuario limitada

## 🛠️ Comandos Útiles del Servicio

```cmd
# Ver estado
nssm status ContaFacil

# Detener servicio
nssm stop ContaFacil

# Reiniciar servicio
nssm restart ContaFacil

# Desinstalar servicio
nssm remove ContaFacil confirm
```

## 📝 Logs y Diagnóstico

Los logs de la aplicación se encuentran en:
- Logs de aplicación: Configurados en `appsettings.json`
- Logs del servicio: Visor de eventos de Windows

## 🔄 Actualización

Para actualizar a una nueva versión:

1. Detener el servicio: `nssm stop ContaFacil`
2. Reemplazar `ContaFacil.exe` con la nueva versión
3. Iniciar el servicio: `nssm start ContaFacil`

## ⚠️ Notas Importantes

- **Backup**: Realiza copias de seguridad de la base de datos regularmente
- **appsettings.json**: NO elimines este archivo, contiene la configuración crítica
- **wwwroot**: Contiene archivos estáticos (CSS, JS, imágenes)
- **Primera ejecución**: Puede tardar unos segundos en iniciar

## 🆘 Solución de Problemas

### La aplicación no inicia:
- Verifica que el puerto no esté en uso
- Revisa la cadena de conexión a la base de datos
- Verifica los permisos de la carpeta

### Error de conexión a BD:
- Verifica que MySQL esté corriendo
- Confirma usuario y contraseña en appsettings.json
- Verifica que la base de datos exista

### Puerto en uso:
- Cambia el puerto en appsettings.json
- O detén la aplicación que usa el puerto 5000

## 📞 Soporte

Para más información, contacta al administrador del sistema.

---
**Versión**: .NET 8.0
**Fecha de Publicación**: Noviembre 2024
**Plataforma**: Windows x64
