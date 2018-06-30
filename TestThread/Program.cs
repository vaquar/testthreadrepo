using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class Program
{
    static Random rand = new Random();
    public static void Main()
    {
        
        try
        {
            //ExecuteTasks();
            ExecuteAnyTasks();
            //Console.ReadKey();
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
            {
                Console.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
            }
        }
    }

    async static Task ExecuteAnyTasks()
    {
        Task<double> otask = null;
        var tasks2 = new List<Task<double>>();

        //otask = Task<double>.Factory.StartNew(() => TrySolution1()).GetAwaiter().GetResult();
        otask = Task.Run(() => TrySolution1().GetAwaiter().GetResult());
        tasks2.Add(otask);
        Console.WriteLine("started task[{0}].", otask.Id);

        otask = Task.Run(() => TrySolution2().GetAwaiter().GetResult());
        tasks2.Add(otask);
        Console.WriteLine("started task[{0}].", otask.Id);

        otask = Task.Run(() => TrySolution3().GetAwaiter().GetResult());
        tasks2.Add(otask);
        Console.WriteLine("started task[{0}].", otask.Id);

        while (tasks2.Count > 0)
        { 
            try
            {
                otask = await Task.WaitAny(tasks2.ToArray());//.ConfigureAwait(false);
                if (otask.Status == TaskStatus.RanToCompletion)
                {
                    //Task.WaitAll(tasks2);
                    double d = otask.Result;
                    Console.WriteLine("task[{0}] completed first with result of {1}.", otask.Id, d);
                    break;
                }
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("caught AE exception: " + ae.Flatten().InnerException.Message);
                Console.WriteLine("Removed task# {0}", otask.Id);
                tasks2.Remove(otask);
            }
        }
    }

    async static Task<double> TrySolution1()
    {
        
        int i = rand.Next(1000000);
        // Simulate work by spinning
        Console.WriteLine("1()  i= "+ i);
        Thread.SpinWait(i);
        throw new ArgumentException();
        return DateTime.Now.Millisecond;
    }
    async static Task<double> TrySolution2()
    {
        int i = rand.Next(1000000);
        // Simulate work by spinning
        Console.WriteLine("2()  i= "+ i);
        Thread.SpinWait(i);
        return DateTime.Now.Millisecond;
    }
    async static Task<double> TrySolution3()
    {
        int i = rand.Next(1000000);
        // Simulate work by spinning
        Console.WriteLine("3()  i= "+ i);
        Thread.SpinWait(i);
        Thread.SpinWait(1000000);
        return DateTime.Now.Millisecond;
    }

    static void ExecuteTasks()
    {
        // Assume this is a user-entered String.
        String path = @"C:\";
        List<Task> tasks = new List<Task>();

        tasks.Add(Task.Run(() => {
            // This should throw an UnauthorizedAccessException.
            return Directory.GetFiles(path, "*.txt",
                                      SearchOption.AllDirectories);
        }));

        tasks.Add(Task.Run(() => {
            if (path == @"C:\")
                throw new ArgumentException("The system root is not a valid path.");
            return new String[] { ".txt", ".dll", ".exe", ".bin", ".dat" };
        }));

        tasks.Add(Task.Run(() => {
            //Console.WriteLine("Sleeping...");
            //Thread.Sleep(3000);
            throw new NotImplementedException("This operation has not been implemented.");
        }));

        tasks.Add(Task.Run(() =>
        {
            var val = 0;
            for (int i=0; i<1000; i++)
            {
                val += i;
            }
            Console.WriteLine("Finished total : " + val);
            return 0;
        }));

        try
        {
            Task.WaitAll(tasks.ToArray());
        }
        catch (AggregateException ae)
        {
            throw ae.Flatten();
        }
    }
}
// The example displays the following output:
//       UnauthorizedAccessException:
//          Access to the path 'C:\Documents and Settings' is denied.
//       ArgumentException:
//          The system root is not a valid path.
//       NotImplementedException:
//          This operation has not been implemented.