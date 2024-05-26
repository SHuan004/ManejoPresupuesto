# ManejoPresupuesto

"ManejoPresupuesto" es una aplicación web desarrollada en .NET Core diseñada para ayudar a los usuarios a gestionar sus finanzas personales de manera eficiente.
Permite a los usuarios registrar y monitorear ingresos y gastos, configurar presupuestos mensuales, y obtener una visión clara de su salud financiera 
mediante visualizaciones interactivas y reportes detallados.

## Prerrequisitos

Para ejecutar esta aplicación, necesitarás:
- .NET Core SDK (versión 3.1 o superior)
- Visual Studio o VS Code (recomendado para desarrollo)

## Configuración

### Clonar el repositorio

Obtén una copia local del código fuente clonando el repositorio:

git clone https://github.com/SHuan004/ManejoPresupuesto.git
cd ManejoPresupuesto

### Instalar dependencias


dotnet restore

### Configuración de la base de datos

Configura la cadena de conexión de la base de datos en el archivo appsettings.json y aplica las migraciones necesarias con:

dotnet ef database update

## Ejecución

Compila y ejecuta la aplicación utilizando:

dotnet run

## Contacto

Sebastian Huanca – shuanca128@gmail.com
Link del proyecto: https://github.com/SHuan004/ManejoPresupuesto
