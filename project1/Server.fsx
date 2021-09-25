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
                    port = 1000
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("RemoteFSharp", configuration)
let numberOfActors = Environment.ProcessorCount
let workUnit = 100
let servers = "akka.tcp://RemoteActorSystem@localhost:2000/user/Distributor"


let printActor(name) =
    spawn system "PrintServer"
    <| fun mailbox ->
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                match box message with
                | :? string -> 
                        printfn "printServer!"
                        printfn message
                | _ ->  failwith "unknown message"
            } 
        loop()
    
let echoServer = 
    spawn system "EchoServer"
    <| fun mailbox ->
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                match box message with
                | :? string -> 
                        printfn "super!"
                        sender <! sprintf "Hello %s remote" message
                        return! loop()
                | _ ->  failwith "unknown message"
            } 
        loop()

for n in [1 .. 2] do
    Async.RunSynchronously ((system.ActorSelection("akka.tcp://RemoteActorSystem@24.250.161.160:8777/user/Distributor") <? string(n)), 1000)

Console.Read() |> ignore