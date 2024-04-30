# .NET Web Scraping
La aplicación utiliza Selenium para interactuar con sitios web y JSON Web Token (JWT) para la autenticación de usuarios.

## Autenticación con JWT
El proceso de autenticación se realiza mediante la presentación de un usuario de prueba en la ruta "/auth/{usuario}/{contraseña}".


    Usuario: testUser
    Contraseña: testPassword

##Uso del Webscraper
Al enviar una solicitud GET a la ruta /auth/{usuario}/{contraseña} con las credenciales del usuario de prueba, se obtiene un token de autenticación JWT.
El token JWT obtenido se debe incluir en el encabezado Authorization de las solicitudes posteriores a la ruta /api/WebScraper/{busqueda}.

La estructura del encabezado Authorization debe ser:

    Key: Authorization
    Value: Bearer {Token}

Luego, el metodo de busqueda empezara una session de chromium y trabajara recolectando al informacion de las tres paginas.
Se devolvera una respuesta en formato Json, una coleccion de 3 objetos cada uno representando los resultados de cada pagina y con un elemento de cuenta del total.
