# AWSLambda.LocalInvoke

You have ASP.NET Core apps running locally via Kestrel. When deployed, the apps are wrapped in a Lambda. You use `AmazonLambdaClient` to `Invoke()` or `InvokeAsync()` another lambda. All works great, but how do you debug locally?

This library allows for local invocation of your lambdas using Named Pipes. You need to run the Lambdas locally, as well as run a router that will route the calls to the appropriate lambda instance on your machine.
