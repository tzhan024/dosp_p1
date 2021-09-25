#time "on"

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp


let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }
            remote {
                helios.tcp {
                    port = 2000
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("RemoteActorSystem", configuration)
let numberOfActors = Environment.ProcessorCount
let printActor = "akka.tcp://RemoteFSharp@localhost:1000/user/PrintServer";
let actorList =
    [ 1 .. numberOfActors ] |> List.map (fun id ->
        spawn system ("actor"+string(id))
        <| fun mailbox ->
            let rec loop() =
                actor {
                    let! message = mailbox.Receive()
                    let sender = mailbox.Sender()
                    printfn "%A" message
                    match box message with
                    | :? string -> 
                            printfn "super uint64!"
                            system.ActorSelection(printActor) <! message
                            return! loop()
                    | _ ->  failwith "unknown message"
                } 
            loop())

let mutable indx = 0;
let assignActor = 
    spawn system "Distributor"
    <| fun mailbox ->
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                printfn "Hey %A" message
                match box message with
                | :? string -> 
                        actorList.Item((indx%actorList.Length)) <! message
                        indx <- indx+1
                        return! loop()
                | _ ->  failwith "unknown message"
            }  
        loop()

Console.Read() |> ignore

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
