using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment;

public record struct PingResult(int ExitCode, string? StdOutput, string? StdError);

public class PingProcess
{
    private ProcessStartInfo StartInfo { get; } = new("ping");

    public PingResult Run(string hostNameOrAddress)
    {
        string pingArgs = Environment.OSVersion.Platform is PlatformID.Unix ? "-c" : "-n";

        StartInfo.Arguments = $"{pingArgs} 4 {hostNameOrAddress}";

        StringBuilder? stdOutput = null;
        StringBuilder? stdError = null;
        void updateOutput(string? line) =>
            (stdOutput ??= new StringBuilder()).AppendLine(line);
        void updateError(string? line) =>
            (stdError ??= new StringBuilder()).AppendLine(line);
        Process process = RunProcessInternal(StartInfo, updateOutput, updateError, default);
        return new PingResult(process.ExitCode, stdOutput?.ToString(), stdError?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        //return new Task<PingResult>(()=> Run(hostNameOrAddress));
        Task<PingResult> task = Task.Run(() => Run(hostNameOrAddress));
       // task.Wait();
        return task;
    }


    async public Task<PingResult> RunAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        //cancellationToken.ThrowIfCancellationRequested();
        Task<PingResult> task = Task.Run(() => Run(hostNameOrAddress), cancellationToken);
        return await task;
    }

    async public Task<PingResult> RunAsync(IEnumerable<string> hostNameOrAddresses, CancellationToken cancellationToken = default)
    {
        StringBuilder? stringBuilder = new();
        int count = 0;
        ParallelQuery<Task<int>>? all = hostNameOrAddresses.AsParallel().Select(async item =>
        {
           // cancellationToken.ThrowIfCancellationRequested();
            Task<PingResult> task = Task.Run(() => Run(item));
            // ...
            lock (stringBuilder)
            {
                stringBuilder.AppendLine(task.Result.StdOutput?.Trim());
                count ++;
            }
            cancellationToken.ThrowIfCancellationRequested();
            await task.WaitAsync(default(CancellationToken));
            return task.Result.ExitCode;
        });
        cancellationToken.ThrowIfCancellationRequested();
        await Task.WhenAll(all);
        return new PingResult(count, stringBuilder?.ToString().Trim(), default);
    }

   /* async public Task<PingResult> RunLongRunningAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        //Task task = null!;
        // await task;
        // throw new NotImplementedException();
        StartInfo.Arguments = hostNameOrAddress;
        void updateOutput(string? output)
        {
            stringBuilder ??= new StringBuilder();
            stringBuilder.AppendLine(output);
        }
        int exitCode = await RunLongRunningAsync(StartInfo, updateOutput, default, cancellationToken);
        PingResult result = new PingResult(exitCode, stringBuilder?.ToString());
        return result;


    }
   */
     public Task<int> RunLongRunningAsync(ProcessStartInfo startInfo, Action<string?>? progressOutput, Action<string?>? progressError, CancellationToken token)
    {
        ProcessStartInfo processStartInfo = UpdateProcessStartInfo(startInfo);

        return Task.Factory.StartNew(() =>
        {
            using (Process running = RunProcessInternal(processStartInfo, progressOutput, progressError, token))
            {
                running.WaitForExit();
                return running.ExitCode;
            }
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    private Process RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        var process = new Process
        {
            StartInfo = UpdateProcessStartInfo(startInfo)
        };
        return RunProcessInternal(process, progressOutput, progressError, token);
    }

    private Process RunProcessInternal(
        Process process,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += OutputHandler;
        process.ErrorDataReceived += ErrorHandler;

        try
        {
            if (!process.Start())
            {
                return process;
            }

            token.Register(obj =>
            {
                if (obj is Process p && !p.HasExited)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Win32Exception ex)
                    {
                        throw new InvalidOperationException($"Error cancelling process{Environment.NewLine}{ex}");
                    }
                }
            }, process);


            if (process.StartInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }
            if (process.StartInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }

            if (process.HasExited)
            {
                return process;
            }
            process.WaitForExit();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'{Environment.NewLine}{e}");
        }
        finally
        {
            if (process.StartInfo.RedirectStandardError)
            {
                process.CancelErrorRead();
            }
            if (process.StartInfo.RedirectStandardOutput)
            {
                process.CancelOutputRead();
            }
            process.OutputDataReceived -= OutputHandler;
            process.ErrorDataReceived -= ErrorHandler;

            if (!process.HasExited)
            {
                process.Kill();
            }

        }
        return process;

        void OutputHandler(object s, DataReceivedEventArgs e)
        {
            progressOutput?.Invoke(e.Data);
        }

        void ErrorHandler(object s, DataReceivedEventArgs e)
        {
            progressError?.Invoke(e.Data);
        }
    }

    private static ProcessStartInfo UpdateProcessStartInfo(ProcessStartInfo startInfo)
    {
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;

        return startInfo;
    }
}