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
    bool isLinux { get; set; } 
    string pingArgs { get; set; } = "";
    string pingOutputLikeExpression { get; set; } = "";



    [TestInitialize]
    public void TestInitialize()
    {
        isLinux = Environment.OSVersion.Platform is PlatformID.Unix;
        (string arg, string exp) = isLinux ? ("-c", PingOutputLikeExpression) : ("-n", PingOutputLikeExpressionWindows);
        pingArgs = arg;
        pingOutputLikeExpression = exp;
        Sut = new();
    }
    [TestMethod]
    public void Start_PingProcess_Success()
    {
        Process process = Process.Start("ping", $"{pingArgs} 4 localhost");
        process.WaitForExit();
        Assert.AreEqual<int>(0, process.ExitCode);
    }

     [TestMethod]
     public void Run_GoogleDotCom_Success()
     {
        int expectedCode = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is null ? 1 : 0;
        int exitCode = Sut.Run("google.com").ExitCode;
         Assert.AreEqual<int>(expectedCode, exitCode);
     }
    [TestMethod]
    public void Run_LocalHost_Success()
    {
        int exitCode = Sut.Run("localhost").ExitCode;
        Assert.AreEqual<int>(0, exitCode);
    }

    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        //adding these checks so it can work on both windows and unix 
        int expectedExitCode = isLinux ? 2 : 1;
        string expectedOutput = isLinux ? "ping: badaddress: Name or service not known"
            : "Ping request could not find host badaddress. Please check the name and try again.";
        (int exitCode, string? stdOutput, string? stdError) = Sut.Run("badaddress");
        string actualOutput = isLinux ? stdError! : stdOutput!;
        Assert.IsFalse(string.IsNullOrWhiteSpace(isLinux ? stdError : stdOutput));
        actualOutput = WildcardPattern.NormalizeLineEndings(actualOutput!.Trim());
        Assert.AreEqual<string?>(
            actualOutput,
            expectedOutput,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(expectedExitCode, exitCode);
    }

    [TestMethod]
    public void Run_CaptureStdOutput_Success()
    {
        PingResult pingTask = Sut.Run("localhost");
        AssertValidPingOutput(pingTask);
    }
    
    [TestMethod]
    public void RunTaskAsync_Success()
    {
        // Do NOT use async/await in this test.
        Task<PingResult> pingTask = Sut.RunTaskAsync("localhost");
       // pingTask.Start();
        AssertValidPingOutput(pingTask.Result);
        }

        [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        // Do NOT use async/await in this test.
        // PingResult result = default;
        // Test Sut.RunAsync("localhost");
        //  AssertValidPingOutput(result);
        PingResult pingTask = Sut.RunAsync("localhost").Result;
        AssertValidPingOutput(pingTask);

    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        // DO use async/await in this test.
        //PingResult result = default;

        // Test Sut.RunAsync("localhost");
        //AssertValidPingOutput(result);
        PingResult pingTask = await Sut.RunAsync("localhost");
        AssertValidPingOutput(pingTask);
    }

    
    [TestMethod]
    [ExpectedException(typeof(AggregateException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        CancellationTokenSource cancelSource = new CancellationTokenSource();
       // CancellationToken cancelToken = cancelSource.Token;
        cancelSource.Cancel();
        Task<PingResult> pingTask = Sut.RunAsync("localhost", cancelSource.Token);
        pingTask.Wait();

    }

    [TestMethod]
    [ExpectedException(typeof(TaskCanceledException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        // Use exception.Flatten()
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        cancelSource.Cancel();

        Task<PingResult> pingTask = Sut.RunAsync("localhost", cancelSource.Token);
        try
        {
            cancelSource.Cancel();
            pingTask.Wait();
        }
        catch (AggregateException ex)
        {
            throw ex.Flatten().InnerException!;
           
        }
    }
    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        // Pseudo Code - don't trust it!!!
        string[] hostNames = new string[] { "localhost", "localhost", "localhost", "localhost" };
        int expectedLineCount = PingOutputLikeExpression.Split(Environment.NewLine).Length * hostNames.Length;
        PingResult result = await Sut.RunAsync(hostNames);
        int lineCount = result.StdOutput!.Split(Environment.NewLine).Length;
        Assert.AreEqual(expectedLineCount, lineCount);
    }

        [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        ProcessStartInfo startInfo = new("ping", $"{pingArgs} 4 localhost");

        int exitCode = await Sut.RunLongRunningAsync(startInfo, null, null, default);

        Assert.AreEqual(0, exitCode);
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
    //for unix 
    readonly string PingOutputLikeExpression = @"
PING * * bytes*
64 bytes from * (*): icmp_seq=* ttl=* time=* ms
64 bytes from * (*): icmp_seq=* ttl=* time=* ms
64 bytes from * (*): icmp_seq=* ttl=* time=* ms
64 bytes from * (*): icmp_seq=* ttl=* time=* ms
--- * ping statistics ---
* packets transmitted, * received, *% packet loss, time *ms
rtt min/avg/max/mdev = */*/*/* ms
".Trim();
    readonly string PingOutputLikeExpressionWindows = @"
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
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression) ?? false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
