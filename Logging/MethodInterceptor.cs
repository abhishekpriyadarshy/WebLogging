using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Logging
{
    public class MethodInterceptor : IAsyncInterceptor
    {
        private static readonly ThreadLocal<int> CallDepth = new ThreadLocal<int>(() => 0);
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
            CallDepth.Value++; // Increment call depth
            // Handle synchronous methods
            var stopwatch = Stopwatch.StartNew();

            // Get method information
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var className = methodInfo.DeclaringType?.Name; // Get the class name
            var methodName = methodInfo.Name; // Get the method name
            var solutionName = Assembly.GetExecutingAssembly().GetName().Name; // Get the solution name

            // Get custom attribute (if present)
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting synchronous method: {invocation.Method.Name}");
            Console.WriteLine($"Intercepting asynchronous method with result: {methodName}");
            Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage}, Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine($"Request (Method Arguments in JSON): Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            }

            try
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType != typeof(void))
                {
                    var responseJson = JsonSerializer.Serialize(invocation.ReturnValue);
                    Console.WriteLine($"Response (Return Value in JSON): Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                    Console.WriteLine(responseJson);
                }

                Console.WriteLine($"Method {invocation.Method.Name} executed successfully. Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in method {invocation.Method.Name}: {ex.Message}  Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.  Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                // Log method name, class name, solution name, and duration
                Console.WriteLine($"Method {methodName} executed in {stopwatch.ElapsedMilliseconds} ms.");
                Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");

                // Check if this is the last message
                if (CallDepth.Value == 1) // If call depth is 1, this is the last message
                {
                    Console.WriteLine("Summary: This is the last message in the call hierarchy.");
                }

                CallDepth.Value--; // Decrement call depth
            }
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            CallDepth.Value++; // Increment call depth
            // Handle asynchronous methods returning Task
            var stopwatch = Stopwatch.StartNew();

            // Get method information
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var className = methodInfo.DeclaringType?.Name; // Get the class name
            var methodName = methodInfo.Name; // Get the method name
            var solutionName = Assembly.GetExecutingAssembly().GetName().Name; // Get the solution name

            // Get custom attribute (if present)
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting asynchronous method with result: {invocation.Method.Name} Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            Console.WriteLine($"Intercepting asynchronous method with result: {methodName}");
            Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage} Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine($"Request (Method Arguments in JSON): Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            } 

            try
            {
                invocation.Proceed();

                Console.WriteLine($"Method {methodName} executed successfully.");

                Console.WriteLine($"Method {invocation.Method.Name} executed successfully. Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                Console.WriteLine("Response: Task completed.");
                Console.WriteLine($"Method {invocation.Method.Name} executed successfully. Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in async method {invocation.Method.Name}: {ex.Message} Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms. Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                // Log method name, class name, solution name, and duration
                Console.WriteLine($"Method {methodName} executed in {stopwatch.ElapsedMilliseconds} ms.");
                Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");

                // Check if this is the last message
                if (CallDepth.Value == 1) // If call depth is 1, this is the last message
                {
                    Console.WriteLine("Summary: This is the last message in the call hierarchy.");
                    Console.WriteLine($"Method {methodName} executed in {stopwatch.ElapsedMilliseconds} ms.");
                }

                CallDepth.Value--; // Decrement call depth
            }
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            CallDepth.Value++; // Increment call depth
            // Handle asynchronous methods returning Task<TResult>
            var stopwatch = Stopwatch.StartNew();

            // Get method information
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var className = methodInfo.DeclaringType?.Name; // Get the class name
            var methodName = methodInfo.Name; // Get the method name
            var solutionName = Assembly.GetExecutingAssembly().GetName().Name; // Get the solution name

            // Get custom attribute (if present)
            var logMessageAttribute = methodInfo.GetCustomAttribute<LogMessageAttribute>();
            var customMessage = logMessageAttribute?.Message;

            Console.WriteLine($"Intercepting asynchronous method with result: {invocation.Method.Name} Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            Console.WriteLine($"Intercepting asynchronous method with result: {methodName}");
            Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");
            if (!string.IsNullOrEmpty(customMessage))
            {
                Console.WriteLine($"Custom Message: {customMessage} Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
            }

            if (invocation.Arguments.Length > 0)
            {
                Console.WriteLine($"Request (Method Arguments in JSON): Name: {logMessageAttribute?.Application}, Environment: {logMessageAttribute.Environment}");
                var argumentsJson = JsonSerializer.Serialize(invocation.Arguments);
                Console.WriteLine(argumentsJson);
            }

            try
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                var result = task.GetAwaiter().GetResult(); // Await the result

                // Log response (method return value in JSON)
                var responseJson = JsonSerializer.Serialize(result);
                Console.WriteLine("Response (Return Value in JSON):");
                Console.WriteLine(responseJson);

                Console.WriteLine($"Method {methodName} executed successfully.");

                Console.WriteLine($"Method {invocation.Method.Name} executed successfully. Name: {logMessageAttribute?.Application}," +
                    $" Environment: {logMessageAttribute.Environment}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in async method {invocation.Method.Name}: {ex.Message} Name: {logMessageAttribute?.Application}, " +
                    $"Environment: {logMessageAttribute.Environment}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms. Name: {logMessageAttribute?.Application}," +
                    $" Environment: {logMessageAttribute.Environment}");
                    // Log method name, class name, solution name, and duration
                Console.WriteLine($"Method {methodName} executed in {stopwatch.ElapsedMilliseconds} ms.");
                Console.WriteLine($"Class Name: {className}, Solution Name: {solutionName}");

                // Check if this is the last message
                if (CallDepth.Value == 1) // If call depth is 1, this is the last message
                {
                    Console.WriteLine("Summary: This is the last message in the call hierarchy.");
                    Console.WriteLine($"Method {methodName} executed in {stopwatch.ElapsedMilliseconds} ms.");
                }

                CallDepth.Value--; // Decrement call depth
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

        
    }
}
