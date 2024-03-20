using IntelliTect.TestTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    PingProcess Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new();
    }
    [TestMethod]
    public void Start_PingProcess_Success()
    {
        Process process = Process.Start("ping", "-c 4 localhost");
        process.WaitForExit();
        Assert.AreEqual<int>(0, process.ExitCode);
    }

    
 
    /*

     [TestMethod]
     public void Run_GoogleDotCom_Success()
     {
         int exitCode = Sut.Run("google.com").ExitCode;
         Assert.AreEqual<int>(0, exitCode);
     }
    */

    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        (int exitCode, string? stdOutput) = Sut.Run("badaddress");
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.AreEqual<string?>(
            "Ping request could not find host badaddress. Please check the name and try again.".Trim(),
            stdOutput,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(2, exitCode);
    }

    [TestMethod]
    public void Run_CaptureStdOutput_Success()
    {
        PingResult result = Sut.Run("-c 4 localhost");
        AssertValidPingOutput(result);
    }
    
    [TestMethod]
    public void RunTaskAsync_Success()
    {
        // Do NOT use async/await in this test.
        Task<PingResult> pingResult = Sut.RunTaskAsync("-c 4 localhost");
        AssertValidPingOutput(pingResult.Result);
        }

        [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        // Do NOT use async/await in this test.
        // PingResult result = default;
        // Test Sut.RunAsync("localhost");
        //  AssertValidPingOutput(result);
        Task<PingResult> pingResult = Sut.RunAsync("-c 4 localhost");
        AssertValidPingOutput(pingResult.Result);

    }

    [TestMethod]
//#pragma warning disable CS1998 // Remove this
    async public Task RunAsync_UsingTpl_Success()
    {
        // DO use async/await in this test.
        //PingResult result = default;

        // Test Sut.RunAsync("localhost");
        //AssertValidPingOutput(result);
        PingResult result = await Sut.RunAsync("-c 4 localhost");
        AssertValidPingOutput(result);
    }
//#pragma warning restore CS1998 // Remove this


    [TestMethod]
    [ExpectedException(typeof(AggregateException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        CancellationToken cancelToken = cancelSource.Token;
        cancelSource.Cancel();
        Task<PingResult> result = Sut.RunAsync("-c 4 localhost", cancelToken);
        result.Wait();



    }

    [TestMethod]
    [ExpectedException(typeof(TaskCanceledException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        // Use exception.Flatten()
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        CancellationToken cancelToken = cancelSource.Token;

        try
        {
            Task<PingResult> result = Sut.RunAsync("-c 4 localhost", cancelToken);
            cancelSource.Cancel();
            result.Wait();
        }
        catch (AggregateException ex)
        {
            ex.Flatten().Handle(inner =>
            {
                if (inner is TaskCanceledException)
                {
                    throw inner;  //Rethrow TaskCanceledException to satisfy expected exception
                }

                return false;  //Indicates we have not handled other exceptions, so they are still propagated.
            });

            // If no TaskCanceledException was found (though one should be), rethrow the original AggregateException.
            // However, in this structure, we should never hit this point with TaskCanceledException present.
            throw;
        }
    }
 
    

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        // Pseudo Code - don't trust it!!!
        string[] hostNames = new string[] { "-c 4 localhost", "-c 4 localhost", "-c 4 localhost", "-c 4 localhost" };
        //int expectedLineCount = PingOutputLikeExpression.Split(Environment.NewLine).Length*hostNames.Length;
       // PingResult result = await Sut.RunAsync(hostNames);
        PingResult result = await PingProcess.RunAsync(hostNames);

        // Assert.IsNotNull(result, "PingResult should not be null.");
        // int? lineCount = result.StdOutput?.Split(Environment.NewLine).Length;
        // Assert.AreEqual<int?>(expectedLineCount, lineCount);
        AssertValidPingOutput(result);
    } 
    
    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        CancellationToken cancelToken = cancelSource.Token;
        PingResult result = await Sut.RunLongRunningAsync("-c 4 localhost");
        AssertValidPingOutput(result);
    } 

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, short.MaxValue);
        System.Text.StringBuilder stringBuilder = new();
        numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
        int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;
        Assert.AreNotEqual(lineCount, numbers.Count()+1);
    }

    readonly string PingOutputLikeExpression = @"
Pinging * with 32 bytes of data:
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*

Ping statistics for ::1:
    Packets: Sent = *, Received = *, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = *, Maximum = *, Average = *".Trim();
    private void AssertValidPingOutput(int exitCode, string? stdOutput)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression)??false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
