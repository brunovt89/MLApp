# MLApp - Portal Mobile del Trabajador (Access Salud & ART)

Aplicación móvil PWA y Backend REST en **.NET 10.0** para la autogestión de Medicina Laboral y seguimiento integral de siniestros de ART.

> **Proyecto Final - Inteligencia Artificial para Programadores**  
> **Universidad Tecnológica Nacional (UTN BA - Centro de e-Learning)**  

---

## Enlaces del Proyecto
- **Repositorio GitHub:** [https://github.com/brunovt89/MLApp.git](https://github.com/brunovt89/MLApp.git)
- **Aplicación en Producción:** [https://ejemplo.com/MLApp/mobile/](https://ejemplo.com/MLApp/mobile/)
- **Swagger API (Producción):** [https://ejemplo.com/MLApp/](https://ejemplo.com/MLApp/)

---

## Arquitectura y Stack Tecnológico

| Capa | Tecnología | Justificación Técnica |
| :--- | :--- | :--- |
| **Frontend Mobile** | HTML5, Tailwind CSS, Lucide Icons, Vanilla JS (PWA) | Máxima velocidad de carga, sin dependencias pesadas de frameworks, soporte offline nativo vía Service Worker e instalable como App nativa en Android/iOS. |
| **Backend API** | ASP.NET Core Web API en **.NET 10.0** | Alto rendimiento, compatibilidad nativa con IIS en Windows Server, inyección de dependencias y Swagger OpenAPI interactivo. |
| **Base de Datos** | Microsoft SQL Server | Motor transaccional relacional robusto con Stored Procedures y consultas optimizadas para trazabilidad médica y de siniestros. |
| **Seguridad** | Encabezado X-Api-Key + Gatekeeper de Testing | Protección de endpoints mediante token de seguridad precompartido y control de acceso restringido para entorno de pruebas. |
| **IA Local** | Ollama (llama3.2:3b / phi4-mini) | Procesamiento local confidencial para resúmenes de evolución médica y asistencia sin fuga de datos sensibles a la nube. |

---

## Funcionalidades Principales de la Aplicación

1. **Autogestión por DNI y Credencial Digital:**
   - Acceso seguro mediante documento de identidad.
   - Generación dinámica de Credencial Digital con código QR oficial para validación en centros médicos y farmacias.
2. **Seguimiento Dinámico de Siniestros (Circuito Clínico):**
   - La pantalla principal prioriza el siniestro activo (en curso).
   - Cálculo dinámico de etapas: Denuncia -> Atención Médica Inicial (IAMI) -> Evolución y Kinesiología (IET/FPA) -> Alta Médica Definitiva (AM).
   - Visualización de siniestros en acordeón interactivo con listado cronológico de formularios clínicos (lo más reciente arriba).
3. **Agenda Inteligente de Turnos:**
   - Filtro por pestañas: **Próximos** (futuros) vs. **Historial** (pasados).
   - Botón directo para **Agendar en Google Calendar** para turnos pendientes.
   - Clasificación visual de asistencia: *Programado*, *Concurrió* (verde) o *No Asistió* (rojo).
4. **Línea de Urgencias 24hs:**
   - Botón de marcado directo ante accidentes laborales con la ART.

---

## Estructura del Repositorio

`	ext
├── MLApp.Api/                # Backend ASP.NET Core .NET 10.0
│   ├── Controllers/          # Auth, Turno, Siniestro, Examen
│   ├── Services/             # Capa de datos Dapper / ADO.NET
│   ├── Models/               # DTOs y entidades del dominio
│   ├── appsettings.template.json # Plantilla de configuración segura
│   └── Program.cs            # Configuración de pipeline, CORS y Swagger
├── ml-app-pwa/               # Frontend Mobile PWA
│   ├── index.html            # Interfaz móvil responsiva y lógica JS
│   ├── manifest.json         # Manifiesto PWA (instalable en celular)
│   ├── sw.js                 # Service Worker para caché y rendimiento
│   ├── auth-config.template.json # Plantilla de clave de testing
│   ├── icon-192.png          # Icono de app para Android/iOS
│   ├── icon-512.png          # Icono de alta resolución
│   └── logoaccess.png        # Logo transparente adaptable
└── .gitignore                # Reglas estrictas de exclusión de secretos
`

---

## Ejecución Local

### Prerrequisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- Python 3.x (o cualquier servidor HTTP estático)
- SQL Server con base de datos configurada

### 1. Levantar la API (.NET 10.0)
`ash
cd MLApp.Api
# Crear appsettings.json copiando appsettings.template.json y completando la cadena de conexión
dotnet run --no-launch-profile
# La API quedará disponible en http://localhost:5000
`

### 2. Levantar la PWA Mobile
`ash
cd ml-app-pwa
# Crear auth-config.json copiando auth-config.template.json
python -m http.server 3000
# Abrir en el navegador: http://localhost:3000
`

---

## Seguridad y Buenas Prácticas
- **Gestión de Secretos:** Las cadenas de conexión, contraseñas de testing y API Keys están estrictamente excluidas de Git mediante .gitignore y purgadas del historial de commits.
- **Cumplimiento Legal:** El sistema protege los datos médicos del trabajador conforme a la Ley 25.326 de Protección de Datos Personales (Argentina) y normativas de la Superintendencia de Riesgos del Trabajo (SRT).
