# Amazon Lambda ASP.NET Core App Mesh

You have a fleet of serverless ASP.NET Core apps configured as AWS Lambda functions. While `AmazonLambdaClient.InvokeAsync()` found in the [AWSSDK.Lambda](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Lambda/MLambdaInvokeInvokeRequest.html) is a one way to achieve [inter-Lambda communication](https://docs.aws.amazon.com/lambda/latest/dg/lambda-invocation.html), the method does not work for invocation of Lambdas running on your local machine. This library aims to fill this functionality gap by marshalling your requests using Kestrel when your ASP.NET Core lambdas are being debugged locally.

![Overview](doc/Lambda%20App%20Mesh%20Overview.png)

### Roadmap
* Catalog clients: auto-discover the catalog tool url (assess feasibility)
* Support for Lambdas deployed behind an ALB

### Example

Say you have an existing Invoke code such as this:

```csharp
var invokeReq = new InvokeRequest();
invokeReq.FunctionName = "MyLambdaFunction";
invokeReq.InvocationType = InvocationType.RequestResponse;
// ... other params

var apiGatewayReq = new APIGatewayProxyRequest()
{
    HttpMethod = "GET",
    Path = "/home/index"
};

invokeReq.Payload = JsonSerializer.Serialize(apiGatewayReq);

var lambdaClient = new AmazonLambdaClient(); // region, creds

var resp = await lambdaClient.InvokeAsync(invokeReq); // When running in AWS environment

// --- OR, WHEN DEBUGGING LOCALLY ---
// This will route the request to MyLambdaFunction running on your local machine

resp = await invokeReq.RouteAPIGatewayProxyRequestLocally();
```
## Getting Started

### Catalog Tool Installation

Catalog Tool keeps track of all the running Lambda ASP.NET Core Applications on your local machine that register to recieve incoming `InvokeRequest` requests.

```
dotnet tool install -g AWSLambda.AspNetCoreAppMesh.Catalog

/* or to update */

dotnet tool update -g AWSLambda.AspNetCoreAppMesh.Catalog
```

Run the Catalog Tool

```
dotnet lambda-app-mesh --urls http://localhost:5050
```

The `--urls` param is optional. The tool will listen on port 5000 and 5001 by default.

Once the Catalog Tool is running, your ASP.NET Core applications will be able to register themselves with the catalog. Ensure the catalog url is resolvable and reachable by your applications.

See full [Catalog Tool Documentation](src/AWSLambda.AspNetCoreAppMesh.Catalog)

### Configuring Your ASP.NET Core Application to route `InvokeRequest` objects locally

Add `AWSLambda.AspNetCoreAppMesh` NuGet package to your application.

In Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddAWSLambdaAppMeshClient(opts =>
	{
		opts.LambdaName = "MyAspNetCoreLambda"; // name of your Lambda function
		opts.CatalogUrl = "http://localhost:5050"; // URL the Catalog Tool (dotnet lambda-app-mesh) is listening on
	});
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseAWSLambdaAppMeshClient();
}
```

Now, you can call `RouteAPIGatewayProxyRequestLocally()` on your `InvokeRequest` requests. In order for `InvokeRequest` to be processed, the receiver ASP.NET Core Lambda must be running on your machine, and must have registered with the Catalog tool.

### Configuring Your ASP.NET Core Application to receive incoming `InvokeRequest` requests

_It is assumed that your ASP.NET Core Lambda applications are configured to run using Kestrel when debugging locally. If you are running the app using IIS or IIS Express then the incoming requests will fail to process properly._

**Ensure the Catalog Tool (dotnet lambda-app-mesh) is running prior to launching your ASP.NET Core apps**. Otherwise, you'll get an exception when trying to register with the catalog.

In Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{	
	services.AddAPIGatewayProxyFunctionEntryPoint<LambdaEntryPoint>(); // your APIGatewayProxyFunction entry point
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.HandleIncomingAWSLambdaInvokeRequests(env);
}
```
