# Analyze and Process OpenStreetMap data with F#

<br/>
<br/>

![OpenStreeMap Logo](/images/osm-logo.png)
and
![FSharp Logo](/images/fsharp-logo.png)

<br/>
<br/>

F# is ~~a~~ the perfect programming language for OSM data analysis and processing. I use it since many years - in combination with C# for low level pointer access to memory. I used Python, R, C++ or SQL for data analysis and if you use any of these you should also try F#. For all reasons - performance, elegance, succinctness and correctness.

For instance, accessing the data inside a memory mapped FlatMap file feels like accessing the data in a declarative way with the advantage of a modern pragmatic programming language and the speed of raw memory access. Besides being modern, F# is probably the most innovative language in existence, F# invents the tools that matter and stick - many fundamental additions to major programming languages where catalyzed via F# - as an example you surely have heard about async/await in Javascript, C++, Python, Rust and so on - they all got it from F# (https://en.wikipedia.org/wiki/Async/await).

The scripts in this repo demonstrate how you might work with F# and OSM data. If you do similar things with other languages, Python, Java, Go or C++ and maybe databases - lets compare execution times and code samples.

If you are interested in more about F# and OSM data please star this repo, leave comments in the wiki or start discussions. I will be happy to get more geo and openstreetmap people into working with F#.

Here is an excerpt from [example.fsx](https://github.com/snuup/openstreetmap-with-fsharp/blob/main/example.fsx)

```fsharp
let marburg = fm.FindRelByName "Marburg"

marburg.Members // list members

let ways = // get the ways of marburg
    marburg.Members
    |> Seq.map (fun m -> fm.GetWay m.Id)
    |> Seq.toList

type Way with // adorn type Way so we easily get first and last node id
    member w.First with get() = (w.Nodes |> Seq.head).Id
    member w.Last  with get() = (w.Nodes |> Seq.last).Id

ways
|> List.map (fun w -> w.Id, w.First, w.Last) // list way-id with first and last node-id
```
