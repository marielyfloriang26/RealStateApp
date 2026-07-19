# Funcionalidades del Agente: Detalle de propiedades(Agente)

Cuando un usuario autenticado con rol Agente ingrese a la pantalla de detalle de una propiedad registrada por él, el sistema debe mostrar la información general de la propiedad y habilitar funcionalidades adicionales para gestionar la comunicación con clientes y las ofertas recibidas.

Esta pantalla debe permitir que el agente consulte los datos completos de la propiedad, visualice las conversaciones asociadas a dicha propiedad y responda las ofertas realizadas por los clientes.

## Información general de la propiedad
La pantalla de detalle debe mostrar la información principal de la propiedad seleccionada.
De la propiedad se debe visualizar, como mínimo, la siguiente información:

| Campo | Descripción |
| :--- | :--- |
| **Imágenes de la propiedad** | Slider o galería con todas las imágenes registradas para la propiedad. |
| **Código de la propiedad** | Código único de identificación de la propiedad. |
| **Tipo de propiedad** | Tipo o categoría de la propiedad. |
| **Tipo de venta** | Tipo de operación asociada a la propiedad. |
| **Precio** | Valor monetario de la propiedad. |
| **Cantidad de habitaciones** | Número de habitaciones disponibles. |
| **Cantidad de baños** | Número de baños disponibles. |
| **Tamaño de la propiedad** | Tamaño expresado en metros. |
| **Descripción** | Descripción general de la propiedad. |
| **Mejoras** | Listado de mejoras o características adicionales asociadas a la propiedad. |
| **Estado de la propiedad** | Indica si la propiedad se encuentra disponible o vendida. |

## Sección de conversaciones con clientes
La pantalla de detalle de propiedad debe incluir una sección donde el agente pueda visualizar los clientes que han iniciado una conversación sobre esa propiedad.
El agente puede mantener conversaciones con varios clientes para una misma propiedad. Por esta razón, el sistema debe mostrar primero un listado con los clientes que han enviado mensajes relacionados con la propiedad seleccionada.

De cada cliente se debe mostrar la siguiente información:

| Campo | Descripción |
| :--- | :--- |
| **Nombre del cliente** | Nombre y apellido del cliente que inició la conversación. |
| **Último mensaje** | Resumen o vista previa del último mensaje enviado en la conversación. |
| **Fecha del último mensaje** | Fecha y hora del último mensaje registrado. |

Al hacer clic sobre el nombre de un cliente, el sistema debe redirigir al agente a una pantalla donde se muestre la conversación completa entre el agente y ese cliente para la propiedad seleccionada.

### Pantalla de conversación con el cliente
La pantalla de conversación debe mostrar todos los mensajes intercambiados entre el cliente y el agente para la propiedad seleccionada.
Cada mensaje debe mostrar, como mínimo:

| Campo | Descripción |
| :--- | :--- |
| **Remitente** | Indica si el mensaje fue enviado por el cliente o por el agente. |
| **Mensaje** | Contenido del mensaje enviado. |
| **Fecha** | Fecha y hora en que fue enviado el mensaje. |

Además, el agente debe visualizar un formulario para responder al cliente.
El formulario debe contener los siguientes campos:

| Campo | Tipo de dato | Requerido | Descripción |
| :--- | :--- | :--- | :--- |
| **Mensaje** | Texto / string | Sí | Respuesta que el agente desea enviar al cliente. |

Debajo del campo debe existir un botón con el texto `Enviar respuesta`.

#### Validaciones del chat del agente
El formulario de respuesta debe cumplir las siguientes validaciones:
* El agente debe estar autenticado.
* El usuario autenticado debe tener rol **Agente**.
* La propiedad debe existir.
* La propiedad debe pertenecer al agente autenticado.
* El cliente seleccionado debe tener una conversación asociada a esa propiedad.
* El mensaje es requerido.
* El mensaje no debe enviarse vacío.
* El mensaje debe quedar asociado al agente, al cliente y a la propiedad correspondiente.

Si el agente intenta enviar una respuesta vacía, el sistema debe mostrar un mensaje como:
`“Debe escribir un mensaje antes de enviarlo.”`

Si el mensaje se envía correctamente, el sistema debe mostrarlo dentro de la conversación.

## Sección de ofertas recibidas
La pantalla de detalle de propiedad debe incluir una sección donde el agente pueda visualizar los clientes que han realizado ofertas sobre la propiedad seleccionada.
De cada cliente que haya realizado ofertas se debe mostrar la siguiente información:

| Campo | Descripción |
| :--- | :--- |
| **Nombre del cliente** | Nombre y apellido del cliente que realizó una o más ofertas. |
| **Cantidad de ofertas** | Cantidad de ofertas realizadas por ese cliente para la propiedad. |
| **Última oferta** | Monto de la oferta más reciente realizada por el cliente. |
| **Estado de la última oferta** | Estado actual de la oferta más reciente. |

Al hacer clic sobre el nombre de un cliente, el sistema debe redirigir al agente a una pantalla donde se listen todas las ofertas realizadas por ese cliente para la propiedad seleccionada.

### Listado de ofertas de un cliente
En la pantalla de ofertas del cliente seleccionado, el sistema debe mostrar todas las ofertas que dicho cliente ha realizado sobre la propiedad.
De cada oferta se debe visualizar la siguiente información:

| Campo | Descripción |
| :--- | :--- |
| **Fecha de la oferta** | Fecha y hora en que el cliente realizó la oferta. |
| **Monto ofertado** | Valor monetario ofrecido por el cliente. |
| **Estado de la oferta** | Estado actual de la oferta. |
| **Acción** | Opción para aceptar o rechazar la oferta, sólo cuando la oferta esté en estado pendiente. |

#### Estados de una oferta
Las ofertas deben manejar los siguientes estados:

| Estado | Descripción |
| :--- | :--- |
| **Pendiente** | Estado inicial de una oferta cuando aún no ha sido respondida por el agente. |
| **Rechazada** | Estado asignado cuando el agente rechaza la oferta. |
| **Aceptada** | Estado asignado cuando el agente acepta la oferta. |

#### Responder una oferta
Cuando una oferta se encuentre en estado **pendiente**, el sistema debe permitir al agente responder mediante las acciones `Aceptar` o `Rechazar`.
Si el agente selecciona la opción `Rechazar`, el sistema debe cambiar el estado de la oferta a **Rechazada**.
Si el agente selecciona la opción `Aceptar`, el sistema debe realizar las siguientes acciones:
* Cambiar el estado de la oferta seleccionada a **Aceptada**.
* Cambiar el estado de todas las demás ofertas pendientes de esa propiedad a **Rechazada**, sin importar si pertenecen al mismo cliente o a otros clientes.
* Cambiar el estado de la propiedad a **Vendida**.
* Impedir que cualquier cliente pueda realizar nuevas ofertas sobre esa propiedad.

Esta operación debe ejecutarse de forma completa para evitar que la propiedad quede vendida con más de una oferta aceptada.

#### Validaciones para responder ofertas
El sistema debe cumplir las siguientes validaciones antes de permitir que el agente responda una oferta:
* El agente debe estar autenticado.
* El usuario autenticado debe tener rol **Agente**.
* La propiedad debe existir.
* La propiedad debe pertenecer al agente autenticado.
* La oferta debe existir.
* La oferta debe pertenecer a la propiedad seleccionada.
* La oferta debe encontrarse en estado **pendiente**.
* La propiedad debe estar en estado **Disponible** para poder aceptar una oferta.
* No debe existir otra oferta aceptada para la misma propiedad.

Si el agente intenta responder una oferta que ya fue aceptada o rechazada, el sistema debe mostrar un mensaje como:
`“Esta oferta ya fue respondida.”`

Si el agente intenta aceptar una oferta de una propiedad vendida, el sistema debe mostrar un mensaje como:
`“No se puede aceptar una oferta para una propiedad que ya fue vendida.”`

Si la oferta se rechaza correctamente, el sistema debe mostrar un mensaje como:
`“La oferta fue rechazada correctamente.”`

Si la oferta se acepta correctamente, el sistema debe mostrar un mensaje como:
`“La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.”`

## Reglas adicionales del detalle de propiedad del agente
El detalle de propiedad del agente debe cumplir las siguientes reglas:
* Solo usuarios autenticados con rol **Agente** deben acceder a esta pantalla.
* El agente solo debe acceder al detalle de propiedades registradas por él.
* El agente no debe poder gestionar conversaciones ni ofertas de propiedades pertenecientes a otros agentes.
* El agente debe poder visualizar las conversaciones asociadas a cada propiedad.
* El agente debe poder responder mensajes enviados por clientes.
* El agente debe poder visualizar los clientes que han realizado ofertas sobre la propiedad.
* El agente debe poder consultar el historial completo de ofertas realizadas por cada cliente.
* Solo las ofertas en estado pendiente deben permitir acciones de aceptación o rechazo.
* Una propiedad solo puede tener una oferta aceptada.
* Al aceptar una oferta, la propiedad debe cambiar automáticamente a estado **Vendida**.
* Al aceptar una oferta, todas las demás ofertas pendientes de la misma propiedad deben cambiar automáticamente a estado **Rechazada**.
* Una vez que la propiedad esté vendida, no se deben permitir nuevas ofertas.
* Las ofertas rechazadas y aceptadas deben permanecer visibles como historial.
