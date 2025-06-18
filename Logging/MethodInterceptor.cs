using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Logging
{
    public class MethodInterceptor : IAsyncInterceptor
    {
        private static readonly ThreadLocal<int> CallDepth = new(() => 0);

        public void Intercept(IInvocation invocation)
        {
            InterceptMethod(invocation, isAsync: false);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            InterceptMethod(invocation, isAsync: false);
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            InterceptMethod(invocation, isAsync: true);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            InterceptMethod(invocation, isAsync: true, isGenericTask: true);
        }

        private void InterceptMethod(IInvocation invocation, bool isAsync, bool isGenericTask = false)
        {
            CallDepth.Value++; // Increment call depth
            var stopwatch = Stopwatch.StartNew();

            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var className = methodInfo.DeclaringType?.Name;
            var methodName = methodInfo.Name;
            var solutionName = Assembly.GetExecutingAssembly().GetName().Name;

            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            LogMethodStart(methodName, className, solutionName, customMessage, invocation.Arguments, logMessageAttribute);

            try
            {
                invocation.Proceed();

                if (isAsync)
                {
                    if (isGenericTask)
                    {
                        HandleAsyncGenericTask(invocation).GetAwaiter().GetResult();
                    }
                    else
                    {
                        HandleAsyncTask(invocation).GetAwaiter().GetResult();
                    }
                }
                else if (invocation.Method.ReturnType != typeof(void))
                {
                    LogResponse(invocation.ReturnValue);
                }

                Console.WriteLine($"Method {methodName} executed successfully.");
            }
            catch (Exception ex)
            {
                LogException(methodName, ex, logMessageAttribute);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                LogMethodEnd(methodName, className, solutionName, stopwatch.ElapsedMilliseconds);

                if (CallDepth.Value == 1) // Last message in the call hierarchy
                {
                    Console.WriteLine("Summary: This is the last message in the call hierarchy.");
                }

                CallDepth.Value--; // Decrement call depth
            }
        }

        private async Task HandleAsyncTask(IInvocation invocation)
        {
            await (Task)invocation.ReturnValue;
            Console.WriteLine("Response: Task completed.");
        }

        private async Task HandleAsyncGenericTask(IInvocation invocation)
        {
            var result = await (dynamic)invocation.ReturnValue;
            LogResponse(result);
        }

        private void LogMethodStart(string methodName, string className, string solutionName, string customMessage, object[] arguments, LogMessageAttribute logMessageAttribute)
        {
            Console.WriteLine($"Intercepting method: {methodName}");
            Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");

            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}, Application: {logMessageAttribute?.Application}, Environment: {logMessageAttribute?.Environment}");
            }

            if (arguments.Length > 0)
            {
                Console.WriteLine("Request (Method Arguments in JSON):");
                Console.WriteLine(JsonSerializer.Serialize(arguments));
            }
        }

        private void LogResponse(object response)
        {
            Console.WriteLine("Response (Return Value in JSON):");
            Console.WriteLine(JsonSerializer.Serialize(response));
        }

        private void LogException(string methodName, Exception ex, LogMessageAttribute logMessageAttribute)
        {
            Console.WriteLine($"Exception in method {methodName}: {ex.Message}, Application: {logMessageAttribute?.Application}, Environment: {logMessageAttribute?.Environment}");
        }

        private void LogMethodEnd(string methodName, string className, string solutionName, long elapsedMilliseconds)
        {
            Console.WriteLine($"Method {methodName} executed in {elapsedMilliseconds} ms.");
            Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");
        }
    }
}
