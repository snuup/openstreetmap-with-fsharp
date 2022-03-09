#load "fm.fsx"

open FlatMap
open Fm

#time

let fm = FMView.OpenRead "~/osm/planet.fm"

fm.GetNode 1

fm.GetRel 10777888

for i = 1 to 1000000 do
    fm.GetRel 10777888 |> ignore

let marburg = fm.FindRelByName "Marburg"

marburg.Members |> plo

let ways =
    marburg.Members
    |> Seq.map (fun m -> fm.GetWay m.Id)
    |> Seq.toList

type Way with
    member w.First with get() = (w.Nodes |> Seq.head).Id
    member w.Last  with get() = (w.Nodes |> Seq.last).Id

ways
|> List.map (fun w -> w.Id, w.First, w.Last)
|> plo

let boundary = stitchboundary ways

let getnodes (w: Way, forward) =
    if forward then w.Nodes else w.Nodes |> Seq.rev
    |> Seq.toList

boundary
|> List.collect getnodes
|> List.map (fun n -> n.LonLat)
|> plo

type LonLat with
    member ll.RawId with get() =
        let x : int64 = ll.LonRaw
        let y : int64 = ll.LatRaw
        y + (x <<< 32)

type Way with
    member w.FirstRaw with get() = (w.Nodes |> Seq.head).LonLat.RawId
    member w.LastRaw  with get() = (w.Nodes |> Seq.last).LonLat.RawId

ways
|> List.map (fun w -> w.Id, w.FirstRaw, w.LastRaw)
|> plo



let countries =
    fm.FilterRelsByTag "admin_level" "2"
    |> Seq.toList

countries.Length

countries
|> Seq.map (fun r -> r.Name)
|> plo

let ns =
    countries
    |> Seq.collect (fun r -> r.Members)
    |> Seq.filter (fun m -> m.Type = MemberType.Way)
    |> Seq.map (fun m -> fm.GetWay m.Id)
    |> Seq.collect (fun w -> w.Nodes |> Seq.map (fun n -> n.Id))
    |> Seq.toList

ns.Length

ns |> List.distinct |> List.length
