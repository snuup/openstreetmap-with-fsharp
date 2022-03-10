# Analyze and Process OpenStreetMap data with F#

F# is ~~a~~ the perfect programming language for OSM data analysis and processing. I use it since many years - in combination with C# for low level pointer access to memory. I used Python, R, C++ or SQL for data analysis and if you use any of these you should also try F#. For all reasons - performance, elegance, succinctness and correctness.

For instance, accessing the data inside a memory mapped FlatMap file feels like accessing the data with the convenience of the nicest declarative database languages (is there a nice one?) with the advantage of a modern pragmatic programming language. Calling F# modern is an understatement, its the most innovative language in existence - yes Clojure also has nice tricks, but F# invents the tools that matter and stick - many of the recent additions to major programming languages where catalyzed via F# - as an example you surely have heard about async/await in Javascript, C++, Python, Rust and so on - it origins in F# (https://en.wikipedia.org/wiki/Async/await).

The scripts in this repo demonstrate the way you can work with F# and OSM data. If you do similar things with other languages, Python, Java, Go or C++ and maybe databases - lets compare execution times and code samples.

If you are interested in more about F# and OSM data please star this repo, leave comments in the wiki, discussions. I would be happy to get more geo people into working with F#.


here is an excerpt from [example.fsx](https://github.com/snuup/openstreetmap-with-fsharp/blob/main/example.fsx)

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
