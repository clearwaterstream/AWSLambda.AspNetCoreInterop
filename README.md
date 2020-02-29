# Amazon Lambda ASP.NET Core App Mesh

You have a fleet of serverless ASP.NET Core apps configured as AWS Lambda functions. While `AmazonLambdaClient.InvokeAsync()` found in the [AWSSDK.Lambda](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Lambda/MLambdaInvokeInvokeRequest.html) is a great way to achieve [inter-Lambda communication](https://docs.aws.amazon.com/lambda/latest/dg/lambda-invocation.html), the method does not work for invocation of Lambdas running on your local machine. This library aims to fill this functionality gap by marshalling your requests using IISExpress or IIS when your ASP.NET Core lambdas are being debugged locally.

### Roadmap
* Validate the solution on OSX + Visual Studio for Mac
* Support for Lambdas deployed behind an ALB
* NET 3.1 support (dependant on AWS)

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

invokeReq.Payload = JsonConvert.SerializeObject(apiGatewayReq);

var lambdaClient = new AmazonLambdaClient(); // region, creds

var resp = await lambdaClient.InvokeAsync(invokeReq); // When running in AWS environment

// --- OR, WHEN DEBUGGING LOCALLY ---
// This will route the request to MyLambdaFunction running on your local machine

resp = await invokeReq.RouteAPIGatewayProxyRequestLocally();
```
## Getting Started

### Catalog installation

Catalog tool keeps track of all the running ASP.NET Core Applications on your local machine that register to recieve incoming `InvokeRequest` requests.

```
dotnet tool install -g AWSLambda.AspNetCoreAppMesh.Catalog
```

Run the catalog

```
lambda-mesh-cat --urls http://localhost:5050
```

The `--urls` param is optional. The tool will listen on port 5000 and 5001 by default.

Once the catalog is running, your ASP.NET Core application will be able to register themselves with the catalog. Ensure the catalog url is resolvable and reachable by your applications.

See full [Catalog Tool Documentation](https://github.com/clearwaterstream/aws-lambda-aspdotnetcore-appmesh/tree/master/src/AWSLambda.AspNetCoreAppMesh.Catalog)

### Configuring Your ASP.NET Core Application to route `InvokeRequest` objects locally

In Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddAWSLambdaAppMeshClient(opts =>
	{
		opts.LambdaName = "MyAspNetCoreLambda"; // name of your Lambda function
		opts.CatalogUrl = "http://localhost:5050"; // URL the catalog tool (lambda-mesh-cat) is listening on
	});
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseAWSLambdaAppMeshClient();
}
```

Now, you can call `RouteAPIGatewayProxyRequestLocally()` on your `InvokeRequest` requests. In order for `InvokeRequest` to be processed, the receiver ASP.NET Core Lambda must be running on your machine, and must have registered with the Catalog tool.

### Configuring Your ASP.NET Core Application to receive incoming `InvokeRequest` requests

**Ensure Catalog tool (lambda-mesh-cat) is running prior to to launching your ASP.NET Core apps**. Otherwise, you'll get an exception when trying to register with the catalog.

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
