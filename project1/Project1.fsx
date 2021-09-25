#time "on"
// #r "nuget: Akka.FSharp" 
// #r "nuget: Akka.TestKit" 

open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit
open System
open System.Linq
open System.Security.Cryptography

open System.IO
open System.Security.Cryptography
open System.Text


type Message = { 
    K: int
    W: string
}
type ActorParameter = {
    A: int
    S: int
    isFound: bool
}

// type input = {
//     cmd: int64
//     hs: string
// }

    
// get cmd argument
let cmdArgs = Environment.GetCommandLineArgs()
let n = Array.get cmdArgs 1 |> int

// // get hash string
// let sha = 
//     let prefix = "tinghui.zhang;"

//     //get random string
//     let randomStr = 
//         let chars = "ABCDEFGHIJKLMNOPQRSTUVWUXYZabcdefghijklmnopqrstuvwxyz0123456789"
//         let charsLen = chars.Length
//         let random = System.Random()

//         fun len -> 
//             let randomChars = [|for i in 0..len -> chars.[random.Next(charsLen)]|]
//             new System.String(randomChars)

//     //combine them
//     let string = prefix + (randomStr 10)
//     printfn "%s" string

//     //generate hash code with SHA256
//     let hash = SHA256Managed.Create()
//     let hs = hash.ComputeHash(Encoding.UTF8.GetBytes(string))

//     let ByteToHex bytes = 
//         bytes 
//         |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
//         |> String.concat System.String.Empty
//     ByteToHex(hs)


let countZeros (hashString) = 
    let mutable (i : int) = 0
    let hashChar = hashString.ToString().ToCharArray()
    while hashChar.[i] = '0' do
        i <- i + 1
    i
// countZeros "00000ooooo"

let mutable conti = true

//creating actor model
let sys = ActorSystem.Create("Bitcoin")

type Worker(name) =
    inherit Actor()

    override x.OnReceive message = 
        match box message with
        | :? Message as msg ->
            // let prefix = "tinghui.zhang;"

            // //get random string
            // let randomStr = 
            //     let chars = "ABCDEFGHIJKLMNOPQRSTUVWUXYZabcdefghijklmnopqrstuvwxyz0123456789"
            //     let charsLen = chars.Length
            //     let random = System.Random()

            //     fun len -> 
            //         let randomChars = [|for i in 0..len -> chars.[random.Next(charsLen)]|]
            //         new System.String(randomChars)

            // //combine them
            // let string = prefix + (randomStr 10)

            let sha = 
                //generate hash code with SHA256
                let hash = SHA256Managed.Create()
                let hs = hash.ComputeHash(Encoding.UTF8.GetBytes(msg.W))

                let ByteToHex bytes = 
                    bytes 
                    |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
                    |> String.concat System.String.Empty
                ByteToHex(hs)

            if (countZeros sha) >= msg.K then 
                printfn "%s  ->  %s\n" msg.W sha
                conti <- false
                // let ap : ActorParameter = {
                //     A = 100
                //     S = 10000000
                //     isFound = true
                // } 
                // getDispatcher <! ap
                // printfn "%s  " 

        | _ -> failwith "unknown message"


// let workers = 
//     [1 .. 100]
//     |> List.map(fun id -> let properties = [| string(id) :> obj |]
//                           sys.ActorOf(Props(typedefof<Worker>, properties)))
// let rand = Random(1234567890)

type Dispatcher(name) =
    inherit Actor()

    override x.OnReceive message =
        match box message with
        | :? ActorParameter as ap ->
            if ap.isFound  then
                printfn "founded"
            else
                let workers = 
                    [1 .. 1000]
                    |> List.map(fun id -> let properties = [| string(id) :> obj |]
                                          sys.ActorOf(Props(typedefof<Worker>, properties)))
                let rand = Random(1234)

                // for i = 1 to ap.S do//cannot process to msg.B, can only process to a number before msg.B, and i cannot find the reason
                // while conti do
                for i = 1 to 100000 do
                    let prefix = "tinghui.zhang;"

                    //get random string
                    let randomStr = 
                        let chars = "ABCDEFGHIJKLMNOPQRSTUVWUXYZabcdefghijklmnopqrstuvwxyz0123456789"
                        let charsLen = chars.Length
                        let random = System.Random()

                        fun len -> 
                            let randomChars = [|for i in 0..len -> chars.[random.Next(charsLen)]|]
                            new System.String(randomChars)

                    //combine them
                    let string = prefix + (randomStr 10)
                    let myMessage = {Message.K = n; Message.W = string}
                    workers.Item(i % 1000) <! myMessage
               
        | _ ->  failwith "unknown message"


// let getDispatcher = sys.ActorOf(Props(typedefof<Dispatcher>, [| string("1") :> obj |]))
// let ap : ActorParameter = {
//     A = 100
//     S = 10000000
//     isFound = false
// } 
// getDispatcher <! ap

[<EntryPoint>]
let main argv =
    let timer = System.Diagnostics.Stopwatch.StartNew()
    let getDispatcher = sys.ActorOf(Props(typedefof<Dispatcher>, [| string("1") :> obj |]))
    while conti do
        let ap : ActorParameter = {
            A = 100
            S = 10000000
            isFound = false
        } 
        getDispatcher <! ap
        // printfn "%d" n
        // printfn "%s" sha
        // printfn "%d" (countZeros sha)
    timer.Stop()
    printfn "%f ms" timer.Elapsed.TotalMilliseconds
    0 // return an integer exit code


// add loop to main and terminate conditionally
//send message in loop
