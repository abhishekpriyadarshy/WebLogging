using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Logging
{
    public class MethodInterceptor : IAsyncInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // Start timing
            var stopwatch = Stopwatch.StartNew();

            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;
            // Get the custom attribute (if present)
            

            // Before method execution
            Console.WriteLine($"Intercepting method: {invocation.Method.Name}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}");
            }
            // Log request (method arguments)
            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine("Request (Method Arguments in JSON):");
                var argumentsJson = System.Text.Json.JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            }
            try
            {
                // Proceed with the original method call
                invocation.Proceed();

                // Handle asynchronous methods
                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    HandleAsyncMethod(invocation).GetAwaiter().GetResult();
                }
                else
                {
                    // Log response (method return value in JSON) for synchronous methods
                    if (invocation.Method.ReturnType != typeof(void))
                    {
                        var responseJson = JsonSerializer.Serialize(invocation.ReturnValue);
                        Console.WriteLine("Response (Return Value in JSON):");
                        Console.WriteLine(responseJson);
                    }
                }

                // After method execution
                Console.WriteLine($"Method {invocation.Method.Name} executed successfully.");
                
                
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Exception in method {invocation.Method.Name}: {ex.Message}");
                throw;
            }
            finally
            {
                // Stop timing
                stopwatch.Stop();

                // Log method name and duration
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            // Handle synchronous methods
            var stopwatch = Stopwatch.StartNew();

            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting synchronous method: {invocation.Method.Name}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine("Request (Method Arguments in JSON):");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            }

            try
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType != typeof(void))
                {
                    var responseJson = JsonSerializer.Serialize(invocation.ReturnValue);
                    Console.WriteLine("Response (Return Value in JSON):");
                    Console.WriteLine(responseJson);
                }

                Console.WriteLine($"Method {invocation.Method.Name} executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in method {invocation.Method.Name}: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public async ValueTask InterceptAsynchronous(IInvocation invocation)
        {
            // Handle asynchronous methods returning Task
            var stopwatch = Stopwatch.StartNew();

            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting asynchronous method: {invocation.Method.Name}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine("Request (Method Arguments in JSON):");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            } 

            try
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                await task;

                Console.WriteLine("Response: Task completed.");
                Console.WriteLine($"Method {invocation.Method.Name} executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in async method {invocation.Method.Name}: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public async ValueTask InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            // Handle asynchronous methods returning Task<TResult>
            var stopwatch = Stopwatch.StartNew();

            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting asynchronous method with result: {invocation.Method.Name}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine("Request (Method Arguments in JSON):");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            }

            try
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                var result = await task;

                var responseJson = JsonSerializer.Serialize(result);
                Console.WriteLine("Response (Return Value in JSON):");
                Console.WriteLine(responseJson);

                Console.WriteLine($"Method {invocation.Method.Name} executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in async method {invocation.Method.Name}: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        private async Task HandleAsyncMethod(IInvocation invocation)
        {
            if (invocation.Method.ReturnType.IsGenericType)
            {
                // Handle Task<T>
                var result = await (dynamic)invocation.ReturnValue;
                var responseJson = JsonSerializer.Serialize(result);
                Console.WriteLine("Response (Return Value in JSON):");
                Console.WriteLine(responseJson);
            }
            else
            {
                // Handle Task (void return type)
                await (Task)invocation.ReturnValue;
                Console.WriteLine("Response: Task completed.");
            }
        }

        void IAsyncInterceptor.InterceptAsynchronous(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        void IAsyncInterceptor.InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
