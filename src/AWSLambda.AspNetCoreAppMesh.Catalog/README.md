# Amazon Lambda ASP.NET Core App Mesh - Catalog

### Catalog installation

Catalog keeps track of all the running ASP.NET Core Applications on your local machine that choose to register to recieve incoming `InvokeRequest` requests.

```
dotnet tool install -g AWSLambda.AspNetCoreAppMesh.Catalog
```

Run the catalog

```
dotnet lambda-app-mesh --urls http://localhost:5050
```

The `--urls` param is optional. The tool will listen on port 5000 and 5001 by default.

Once the catalog is running, your ASP.NET Core application will be able to register themselves with the catalog. Ensure the catalog url is resolvable and reachable by your applications.
