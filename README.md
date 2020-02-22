# AWSLambda.AspNetCoreInterop

You have a fleet of serverless ASP.NET Core apps deployed as AWS Lambda functions. You need your apps to talk to each other, either via request-response or [async invocation pattern](https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html).

This library helps with facilitating inter-Lambda communication by leveraging `AmazonLambdaClient`'s `Invoke()` and `InvokeAsync()` found in the [AWS SDK](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Lambda/MLambdaInvokeInvokeRequest.html).

### Current Feature Set
* Support for Lambdas deployed behind an API Gateway
* Support for local inter-Lambda communication (for local development)

### Comming Soon
* Support for Lambdas deployed behind an ALB
