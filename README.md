# aws-lambda-aspdotnetcore-appmesh

You have a fleet of serverless ASP.NET Core apps configured as AWS Lambda functions. While `AmazonLambdaClient`'s `Invoke()` and `InvokeAsync()` found in the [AWS SDK](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Lambda/MLambdaInvokeInvokeRequest.html) are a great way to achieve [inter-Lambda communication](https://docs.aws.amazon.com/lambda/latest/dg/lambda-invocation.html), the methods do not work for invocation of Lambdas running on your local machine. This library aims to fill this functionality gap by marshalling your requests using IISExpress.

### Roadmap
* Support for Lambdas deployed behind an ALB
* NET 3.1 support (waiting for AWS)
