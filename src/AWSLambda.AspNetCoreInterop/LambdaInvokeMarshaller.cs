using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public interface ILambdaInvokeMarshaller
    {

    }

    public class LambdaInvokeMarshaller : ILambdaInvokeMarshaller
    {

    }

    public static class LambdaInvokeMarshallerExtensions
    {        
        public static IServiceProvider Services { get; set; }

        public static async Task RouteLocally(this APIGatewayProxyRequest request, string lambdaName, InvocationType InvocationType, CancellationToken cancellationToken)
        {
        }

        public static async Task RouteLocally(this ApplicationLoadBalancerRequest request, string lambdaName, InvocationType InvocationType, CancellationToken cancellationToken)
        {
        }
    }
}
