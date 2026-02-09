# 🏢 Sistema de Gestión de Coworking "WorkSpace 3000"

## Contexto
Un espacio de trabajo compartido desea administrar sus recursos y las reservas. El sistema debe permitir dar de alta espacios (**Oficinas Privadas** y **Escritorios Compartidos**) y gestionar las reservas de los clientes, asegurando que no existan conflictos de horario.

---

## 🛠 Modelos Sugeridos

1. **Espacio (Clase Base/Abstracta):**
   - Propiedades: `Id` (int), `Nombre` (string), `PrecioPorHora` (decimal).
   - Relación: `List<Reserva> Reservas` (Cada espacio tiene su propia lista).

2. **Oficina (Derivada de Espacio):**
   - Propiedades: `CapacidadPersonas` (int), `TieneProyector` (bool).
   - Si tiene proyector su costo es 10% mayor.

3. **Escritorio (Derivada de Espacio):**
   - Propiedades: `EsDePie` (bool), `Ubicacion` (enum: "Ventana", "Pasillo", "Rincón").

4. **Reserva:**
   - Propiedades: `FechaHoraInicio` (DateTime), `DuracionHoras` (int), `NombreCliente` (string).

---

## 📡 Interfaz de la API (Endpoints)

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| **GET** | `/api/espacios` | Lista todos los espacios con sus detalles y reservas. |
| **POST** | `/api/espacios` | Crea un nuevo espacio (Oficina o Escritorio). |
| **PUT** | `/api/espacios/{id}/reservar` | Agrega una **Reserva** a un espacio existente. |
| **GET** | `/api/espacios/disponibles/{fecha}` | Lista espacios que **no** tengan reservas en una fecha específica. |
| **GET** | `/api/espacios/rango` | Filtra reservas que ocurran en una franja horaria (ej: entre las 10:00 y las 14:00). |
| **GET** | `/api/espacios/informe` | Devuelve: Nombre, Tipo y Ganancia Total (Horas * Precio). |
| **DELETE** | `/api/espacios/{id}/reserva` | Elimina una reserva de un espacio por nombre de cliente o fecha. |

---

## 📋 Validaciones y Reglas de Negocio

### 1. Generales y Creación
- El **Nombre** es obligatorio.
- El **PrecioPorHora** debe ser mayor a **$500**.
- **Oficina:** La capacidad mínima debe ser de **2 personas**.
- **Escritorio:** Si `EsDePie` es `true`, la ubicación **no puede ser** "Ventana".

### 2. Reglas de Reserva ("Overkill")
- **Horario Comercial:** Las reservas solo pueden hacerse entre las **09:00 y las 18:00**.
  - *Restricción:* Ninguna reserva puede terminar después de las 18:00.
- **Días Hábiles:** Solo de **Lunes a Viernes** (Prohibido Sábados y Domingos).
- **Duración:** Mínimo **1 hora**, máximo **4 horas**.
- **Solapamiento:** No se puede reservar si el rango horario (`Inicio` hasta `Inicio + Duración`) choca con otra reserva existente en ese mismo espacio.

---

## 💾 Requerimientos Técnicos

- **Persistencia:** Guardar y leer desde un archivo `espacios.json`.
- **Polimorfismo:** El JSON debe guardar correctamente si es Oficina o Escritorio (usar discriminadores de tipo `$type`).
- **LINQ:** Utilizar expresiones Lambda (`.Any()`, `.Where()`) para las validaciones de solapamiento y los filtros de informes.
- **Fechas:** Utilizar `DateTime` para manejar las validaciones de tiempo.
