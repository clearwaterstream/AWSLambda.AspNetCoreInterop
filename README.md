# AWSLambda.AspNetCoreInterop

You have a fleet of serverless ASP.NET Core apps configured as AWS Lambda functions. While debugging on your local machine, you need your apps to talk to each other, either via request-response or [async invocation pattern](https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html).

While `AmazonLambdaClient`'s `Invoke()` and `InvokeAsync()` found in the [AWS SDK](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Lambda/MLambdaInvokeInvokeRequest.html) are a great way to achieve inter-Lambda communication, the methods do not work for invocation of Lambdas running on your local machine. This library aims to fill this functionality gap.

### Current Feature Set
* Support for Lambdas deployed behind an API Gateway
* Support for local inter-Lambda communication (for local development)

### Comming Soon
* Support for Lambdas deployed behind an ALB
