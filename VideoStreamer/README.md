How to configure the authentication

1. Create a client and add a client scope with Audience mapper type
2. Use the client id as the audience in AddJwtBearer function
3. Use the created scope in swagger settings.

How to get the auth token

1. http://localhost:8080/realms/streamer/protocol/openid-connect/auth?response_type=code&client_id=streamer_client Use
this url to get the code. Use that code in postman to get the auth token