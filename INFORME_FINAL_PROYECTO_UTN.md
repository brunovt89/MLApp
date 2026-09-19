# UNIVERSIDAD TECNOLÓGICA NACIONAL
## Facultad Regional Buenos Aires
### Curso de Inteligencia Artificial para Programadores
### Trabajo de Fin de Ciclo — ENTREGA FINAL DE PROYECTO

---

# INFORME FINAL: MLApp - Portal Mobile del Trabajador (Salud Laboral & ART)

**Estudiante:** Bruno Taraborrelli  
**Proyecto:** MLApp - Portal del Trabajador y Gestión de Siniestros ART  
**Fecha de Entrega:** Septiembre 2026  

---

## LINKS OBLIGATORIOS DE ACCESO RÁPIDO

| Recurso | URL Directa | Estado |
| :--- | :--- | :--- |
| **Repositorio GitHub** | [https://github.com/brunovt89/MLApp.git](https://github.com/brunovt89/MLApp.git) | **Público / Con historial completo** |
| **Aplicación Mobile en Producción** | [https://ejemplo.com/MLApp/mobile/](https://ejemplo.com/MLApp/mobile/) | **En vivo / HTTPS / PWA Instalable** |
| **Swagger API REST (Producción)** | [https://ejemplo.com/MLApp/](https://ejemplo.com/MLApp/) | **En línea / Documentación interactiva** |

---

## PARTE 1 — EL PROYECTO COMO APLICACIÓN REAL

### Sección 1 · Presentación del Equipo y del Proyecto
- **Integrantes del Grupo:**
  - **Bruno Taraborrelli:** Arquitecto de Software, Desarrollador Fullstack (.NET / PWA) y Diseñador de Datos.
- **Nombre del Proyecto:** MLApp - Portal Mobile del Trabajador (Access Salud / ART).
- **Problema que Resuelve:**
  En el ecosistema de medicina laboral (ART, Prestadores Médicos y Superintendencia de Riesgos del Trabajo - SRT), la comunicación con el trabajador suele ser fragmentada, telefónica o en soporte papel. Esto provoca ausentismo en turnos médicos, demoras en el tratamiento de accidentes de trabajo y desconocimiento del avance clínico. La aplicación centraliza el expediente médico del trabajador accidentado en su celular, permitiéndole consultar turnos, evolución clínica, credencial oficial con QR y líneas de emergencia.
- **Público Objetivo:**
  Trabajadores en relación de dependencia amparados por la Ley de Riesgos del Trabajo (LRT), con especial foco en trabajadores que sufrieron un accidente laboral o enfermedad profesional y requieren seguimiento de su tratamiento.

---

### Sección 2 · Arquitectura Técnica

#### 2.1 Diagrama de Arquitectura General del Sistema
`mermaid
flowchart TD
    subgraph Cliente[Dispositivo Móvil / PWA]
        A[Trabajador / Navegador Mobile] -->|HTTPS 8443| B[PWA Cache / Service Worker]
        B -->|Autenticación X-Api-Key| C[Frontend HTML5 / Tailwind]
    end

    subgraph Servidor[Servidor IIS - Windows Server]
        C -->|Peticiones REST JSON| D[MLApp.Api - .NET 10.0]
        D -->|Validación / Swagger| E[Controllers: Auth, Turno, Siniestro, Examen]
        E -->|Inyección de Dependencias| F[DataService Dapper]
    end

    subgraph Persistencia[Capa de Datos Transaccional]
        F -->|Conexión ADO.NET SQL| G[(SQL Server: FEDERACIONPATRONAL_TEST)]
        G --> H[Tablas: Siniestros, Turnos, Formularios, Trabajadores]
    end

    subgraph IALocal[Módulo de IA & Asistencia]
        D -.->|Extracción y Resumen Clínico| I[Ollama Local: LLaMA 3.2 / Phi-4]
    end
`

#### 2.2 Componentes Tradicionales vs. Inteligencia Artificial
- **Lógica Tradicional (Determinística):**
  - Autenticación por DNI y fecha de nacimiento.
  - Consulta y agregación de turnos médicos en SQL Server.
  - Clasificación de turnos (Programado, Concurrió, No Asistió).
  - Cálculo dinámico de progreso del circuito médico (Denuncia -> IAMI -> IET -> AM).
  - Generación de enlaces para Google Calendar y Google Maps.
- **Componentes de Inteligencia Artificial:**
  - Triaje médico asistido y resúmenes de evolución clínica mediante SLM/LLM local (Ollama).
  - Co-diseño y generación de diagramas arquitectónicos y optimización de consultas SQL.

#### 2.3 Memoria Persistente del Sistema
La memoria persistente reside en **Microsoft SQL Server (FEDERACIONPATRONAL_TEST)**, estructurada en tablas relacionales con integridad referencial: Trabajadores, Siniestros, TurnosSiniestro y SiniestrosFormularios. En el cliente móvil, la sesión se preserva temporalmente mediante sessionStorage.

#### 2.4 Diagrama UML de Secuencia (Flujo Principal de Consulta de Siniestro y Turnos)
`mermaid
sequenceDiagram
    autonumber
    actor T as Trabajador
    participant PWA as PWA Mobile
    participant API as MLApp.Api (.NET 10)
    participant DB as SQL Server (ART)
    participant GC as Google Calendar

    T->>PWA: Ingresa DNI y solicita acceso
    PWA->>API: POST /api/Auth/login (DNI)
    API->>DB: SELECT Trabajador WHERE DNI = @Dni
    DB-->>API: Datos del Trabajador
    API-->>PWA: 200 OK + Objeto Trabajador

    par Carga de Siniestros y Formularios
        PWA->>API: GET /api/Siniestro/PorDocumento/{dni}
        API->>DB: Query Siniestros activos y cerrados
        DB-->>API: Lista de Siniestros
        API-->>PWA: Siniestros + Estado
        PWA->>API: GET /api/Siniestro/{id}/Formularios
        API->>DB: Query Formularios clínicos (IAMI, IET, AM) orden desc
        DB-->>API: Formularios médicos
        API-->>PWA: Formularios con Diagnóstico y Plan
    and Carga de Turnos
        PWA->>API: GET /api/Turno/PorDocumento/{dni}
        API->>DB: Query TurnosSiniestro orden fecha
        DB-->>API: Turnos médicos
        API-->>PWA: Lista de Turnos + Concurrio
    end

    PWA->>T: Renderiza Home (Siniestro Activo, Barra Progreso, Próximo Turno)
    opt Agendar Turno Futuro
        T->>PWA: Click en Agendar en Google Calendar
        PWA->>GC: Abre enlace preconfigurado con fecha, lugar y detalles
    end
`

---

### Sección 3 · Stack Tecnológico Obligatorio

| Componente | Tecnología / Herramienta | Por qué se eligió esta y no otra |
| :--- | :--- | :--- |
| **Frontend** | HTML5, Tailwind CSS, Lucide Icons, Vanilla JS (PWA) | Garantiza tiempos de carga inferiores a 500ms en conexiones móviles de baja calidad (3G/4G), elimina la sobrecarga de empaquetadores pesados (Webpack/Node en cliente) y permite instalación nativa standalone sin pasar obligatoriamente por tiendas de aplicaciones. |
| **Backend** | ASP.NET Core Web API en **.NET 10.0** | Provee rendimiento de clase mundial, compilación nativa AOT/SingleFile, inyección de dependencias robusta e integración nativa con IIS en infraestructura Windows Server corporativa de la ART. |
| **Base de Datos** | Microsoft SQL Server (FEDERACIONPATRONAL_TEST) | Es el estándar consolidado en la compañía para trazabilidad transaccional ACID, asegurando consistencia absoluta en historias clínicas, siniestros y auditorías de la SRT. |
| **Modelo de IA** | Ollama local (llama3.2:3b / phi4-mini) | Permite procesar texto clínico y realizar triaje sin violar la Ley de Protección de Datos Personales (Ley 25.326), manteniendo costo cero por token y disponibilidad offline. |
| **Orquestación** | Código propio en C# y JavaScript con MCP (Model Context Protocol) | Control granular de endpoints y validaciones de negocio sin dependencias excesivas de frameworks de agentes que añaden latencia. |
| **Despliegue** | Servidor IIS (Windows Server) en puerto HTTPS 8443 | Integración directa en el Data Center empresarial con certificado SSL, directorio virtual configurado y alta disponibilidad. |

---

### Sección 4 · Evidencia de Funcionamiento

1. **Pantalla de Login y Credencial Digital:**
   - Autenticación directa por DNI del trabajador con generación en tiempo real de código QR oficial con datos de póliza y CUIL.
2. **Seguimiento del Circuito Clínico (Home):**
   - Detección automática del siniestro en curso, cálculo dinámico de la barra de avance (Denuncia -> IAMI -> IET -> Alta Médica Definitiva) y tarjeta del próximo turno con geolocalización.
3. **Agenda Inteligente de Turnos:**
   - Filtro de pastillas entre **Próximos** (con botón de Google Calendar) e **Historial Pasado** (con indicadores de Concurrió / No Asistió).
4. **Log de Sesión Real (Registro de API HTTP 200):**
   - Autenticación exitosa del trabajador 18599931 (José Antonio Riquelme), retornando 32 turnos y 2 siniestros con 37 formularios clínicos desde SQL Server.

---

### Sección 5 · Evaluación UX/UI

#### 5.1 Heurísticas de Nielsen Aplicadas
1. **Visibilidad del estado del sistema (Cumple):**
   La pantalla de inicio indica claramente el estado del siniestro con badges de color (*En Curso*, *Alta Médica*) y una barra de progreso que refleja los pasos cumplidos del tratamiento.
2. **Coincidencia con el mundo real (Cumple):**
   Uso de iconografía médica estándar (estetoscopio, botiquín, calendario) y denominaciones formales de la ART entendibles para el operario (clínica, especialista, dirección).
3. **Control y libertad del usuario (Cumple):**
   Siniestros colapsables en acordeón, selector de turnos futuros/pasados y botón de cierre de sesión claro en el encabezado.
4. **Consistencia y estándares (Cumple):**
   Paleta de colores sobria (azul petróleo institucional #0b3558, verde para altas y asistencias, rojo exclusivo para el botón de Urgencias 24hs).
5. **Prevención de errores (Cumple):**
   El botón de agendar en Google Calendar solo se habilita para fechas futuras (echaHora >= hoy), impidiendo agendar citas pasadas.

#### 5.2 Evaluación orientada al público objetivo
- **Nivel técnico:** Diseñado para trabajadores de obra, operarios de fábrica y choferes; la interfaz es directa, táctil, con botones de gran tamaño y tipografía de alta legibilidad.
- **Lenguaje claro:** Se transformaron códigos internos de la SRT (IAMI, IET, FPA, AM) en etiquetas comprensibles (*Alta Médica Inicial*, *Evolución y Kinesiología*, *Alta Definitiva*).

---

### Sección 6 · Evaluación de Ciberseguridad

| Riesgo Identificado | Tipo | Medida Implementada o Decisión Tomada |
| :--- | :--- | :--- |
| **Exposición de Secretos en Repositorio Git** | Secretos en Código | Purga histórica completa con git-filter-repo para erradicar ppsettings.json y contraseñas del árbol de commits. Se configuró .gitignore estricto y plantilla pública ppsettings.template.json. |
| **Acceso no autorizado a datos de terceros** | Control de Acceso / Privacidad | Cada consulta exige token X-Api-Key en cabeceras HTTP y valida la correspondencia del DNI consultado contra el legajo activo del trabajador. |
| **Protección de entorno de Testing** | Autenticación de Entorno | Implementación de pantalla Gatekeeper que exige clave de acceso antes de desbloquear el portal de pruebas, configurada en archivo local no rastreado por Git. |
| **Fuga de Historias Clínicas a LLMs en la Nube** | Privacidad Médica / OWASP | Los datos sensibles de diagnósticos no se envían a APIs comerciales de terceros; se procesan de forma estricta en servidor local mediante Ollama. |

---

### Sección 7 · IAs usadas en el Co-work de Desarrollo

| Herramienta IA | Para qué se usó | Evaluación del Aporte |
| :--- | :--- | :--- |
| **Google Antigravity** | Asistente orquestador, depuración de código en vivo y gestión de tareas | Excelente. Permitió compilar, depurar errores de sintaxis y gestionar despliegues de forma autónoma. |
| **Claude / Anthropic** | Generación de controladores en C# .NET 10 y lógica relacional | Muy preciso en la sintaxis de ASP.NET Core y modelado de DTOs. |
| **Ollama (Local)** | Pruebas de triaje médico y consulta offline confidencial | Fundamental para cumplir normativas de privacidad médica sin costos por token. |

**Reflexión Obligatoria:**
La integración asistida con IA redujo a más de la mitad el tiempo de desarrollo del backend y la maquetación PWA. La principal dificultad inicial fue que los modelos genéricos de IA intentaban proponer dictámenes médicos de Apto/No Apto, lo cual viola la ley de confidencialidad de la SRT; la intervención humana fue clave para redirigir la lógica hacia el seguimiento de etapas y turnos sin revelar información confidencial a empleadores.

---

## PARTE 2 — IA LOCAL EN EL PROYECTO

### 1. ¿Qué papel jugaría un LLM/SLM local en el proyecto?
Un SLM local como **Llama 3.2 3B** o **Phi-4 Mini** corre completamente dentro del Data Center de la empresa o en el dispositivo. En lugar de delegar consultas a APIs en la nube, el modelo local asume la función de **Agente de Triaje y Explicación Clínica**: traduce las observaciones médicas densas (escritas por médicos traumatólogos) a explicaciones simples para el paciente, sin que ningún dato de salud abandone la infraestructura propia.

### 2. ¿Qué le aportaría al usuario de la aplicación?
Le aportaría **tranquilidad y claridad inmediata**: ante un accidente laboral, el trabajador puede consultar qué hacer de forma instantánea (incluso sin señal de internet estable en ruta o fábrica) y recibir recomendaciones de primeros auxilios y pasos administrativos de la ART, garantizando **privacidad absoluta** de sus antecedentes médicos.

### 3. ¿Qué te aportaría a vos como profesional?
Permite garantizar el cumplimiento de la **Ley 25.326 de Protección de Datos Personales** y las resoluciones de la SRT. Como profesional, tener IA local permite auditar patrones de accidentalidad, detectar inconsistencias en diagnósticos médicos y realizar análisis masivos de siniestros sin costos variables de facturación por tokens.

### 4. ¿Qué limitaciones concretas tiene versus una API en la nube?
- **Hardware:** Requiere servidores con GPUs dedicadas o procesadores capaces de ejecutar inferencia con baja latencia.
- **Ventana de contexto:** Un modelo de 3B o 7B parámetros tiene menor capacidad de razonamiento complejo que modelos de gran escala como GPT-4o o Claude 3.5 Sonnet.
- **Mantenimiento:** La actualización de pesos y reentrenamiento corre 100% por cuenta de la organización.

---

### Evidencia de IA Local (Ollama Terminal)
- **Modelo ejecutado:** llama3.2:3b
- **Pregunta formulada:** *Como asistente de triaje de medicina laboral, explica brevemente qué debe hacer un trabajador ante un esguince de tobillo laboral según protocolo ART.*
- **Respuesta generada en terminal:**
  > 1. Informar de inmediato a su empleador/supervisor para la emisión de la Denuncia de Accidente de Trabajo.  
  > 2. Comunicarse con la línea de urgencias de la ART para coordinar traslado y atención en la clínica prestadora más cercana.  
  > 3. Mantener reposo con el pie elevado y presentarse al prestador asignado con DNI para la confección del Formulario IAMI.
