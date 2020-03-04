# Amazon Lambda ASP.NET Core App Mesh - Catalog

### Catalog Tool Installation

Catalog Tool keeps track of all the running Lambda ASP.NET Core Applications on your local machine that choose to register to recieve incoming `InvokeRequest` requests.

```
dotnet tool install -g AWSLambda.AspNetCoreAppMesh.Catalog
```

Run the Catalog Tool

```
dotnet lambda-app-mesh --urls http://localhost:5050
```

The `--urls` param is optional. The tool will listen on port 5000 and 5001 by default. This is a default port for Kestrel applications so you may want to change it for the Catalog tool.

Once the Catalog Tool is running, your ASP.NET Core applications will be able to register themselves with the catalog. Ensure the catalog url is resolvable and reachable by your applications.

### Getting the List of Registered Functions

In a browser, navigate to the URL the Catalog Tool is listening on, such as http://localhost:5050. You will see a list similar to this:

```text
===============================================================================
                  AWS Lambda ASP.NET Core App Mesh - Catalog
===============================================================================

 --------------------------------------------------------------------- 
 | Function Name      | Listening On                                 |
 --------------------------------------------------------------------- 
 | MyAspNetCoreLambda | http://localhost:63778/lambda-invoke-handler |
 --------------------------------------------------------------------- 

 Count: 1
```
