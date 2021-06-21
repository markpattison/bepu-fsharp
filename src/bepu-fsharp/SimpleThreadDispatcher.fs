module BepuFSharp.SimpleThreadDispatcher

open System
open System.Diagnostics
open System.Threading
open BepuUtilities
open BepuUtilities.Memory

[<Struct>]
type Worker =
    {
        Thread: Thread
        Signal: AutoResetEvent
    }

type SimpleThreadDispatcher(threadCount) =

    let ref workerIndex = 0
    let mutable completedWorkerCounter = 0

    member this.WorkerLoop (untypedSignal: obj) =
        let signal = untypedSignal :?> AutoResetEvent

        while (not disposed) do
            signal.WaitOne() |> ignore
            if not disposed then this.DispatchThread(Interlocked.Increment(workerIndex) - 1)
        ()

    let workers = Array.init (threadCount - 1) (fun _ ->
        let thread = new Thread(WorkerLoop)
        let signal = new AutoResetEvent(false)
        thread.IsBackground <- true
        thread.Start(signal)
        { Thread = thread; Signal = signal })

    let finished = new AutoResetEvent(false)

    let bufferPools = Array.init threadCount (fun _ -> new BufferPool())

    [<VolatileField>]
    let mutable disposed = false

    [<VolatileField>]
    let mutable workerBody = Unchecked.defaultof<Action<int>>

    interface IDisposable with
        member this.Dispose() = ()
    
    interface IThreadDispatcher with
        member this.ThreadCount = threadCount

        member this.DispatchWorkers(workBody) = ()

        member this.GetThreadMemoryPool(workerIndex) = new BufferPool(0)