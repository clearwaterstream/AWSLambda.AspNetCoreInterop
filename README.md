# AWSLambda.AspNetCoreInterop

You have a fleet of ASP.NET Core apps deployed as AWS Lambda function. You need your apps to talk to each other, either via request-response or async invocation pattern.

This library helps with facilitating inter-lambda communication by leveraging `AmazonLambdaClient` from AWS SDK.

### Current Feature Set
* Support for Lambdas deployed behind an API Gateway
* Support for local inter-Lambda communication (for local development)

### Comming Soon
* Support for Lambdas deployed behing an ALB
