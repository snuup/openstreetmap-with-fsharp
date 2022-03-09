#r "fm.dll" // sorry, not open sourced yet

open FlatMap

// print utilities
let po x    = printfn "%O" x
let spo x   = sprintf "%O" x
let plo xs  = Seq.iter po xs
fsi.AddPrinter (fun (n: Node) -> spo n)
fsi.AddPrinter (fun (w: Way) -> $"way#{w.Id}")
fsi.AddPrinter (fun (m: Member) -> spo m)

// Element is the super class of Node, Way and Relation
type Element with

    static member GetTag key (e: Element) : string option =
        e.Tags
        |> Seq.tryFind (fun t -> t.Key = key)
        |> Option.map (fun t -> t.Val)

    static member HasTag key (e: Element) : bool =
        e.Tags
        |> Seq.exists (fun t -> t.Key = key)

    member e.Name with get() =
        e.Tags
        |> Seq.tryFind (fun t -> t.Key = "name")
        |> Option.map (fun t -> t.Val)
        |> Option.defaultValue $"unnamed {e.Id}"

    member e.TagCount with get() =
        e.TagsRaw |> Seq.length

    static member GetTagFilter (getsid: string -> int32) key value =
        let skey = getsid key
        let sval = getsid value
        (fun (e: Element) -> e.TagsRaw |> Seq.exists (fun t -> t.Key = skey && t.Val = sval))

    member e.Item with get(key) =
        e.Tags
        |> Seq.tryFind (fun t -> t.Key = key)
        |> Option.map (fun t -> t.Val)


// add some simple data retrieval methods,
// they all iterate the respective collection
// which can be done much faster with an improved
// access library
type FMView with

    member fm.FindRelByTag key value =
        let skey = fm.StringTable.GetSid key
        let sval = fm.StringTable.GetSid value
        fm.AllRels
        |> Seq.find (fun r -> r.TagsRaw |> Seq.exists (fun t -> t.Key = skey && t.Val = sval))

    member fm.FilterRelsByTag key value =
        let skey = fm.StringTable.GetSid key
        let sval = fm.StringTable.GetSid value
        fm.AllRels
        |> Seq.filter (fun r -> r.TagsRaw |> Seq.exists (fun t -> t.Key = skey && t.Val = sval))

    member fm.FindRelByName name =
        fm.FindRelByTag "name" name

    member fm.FilterRelsHavingTag key =
        let skey = fm.StringTable.GetSid key
        fm.AllRels
        |> Seq.filter (fun r -> r.TagsRaw |> Seq.exists (fun t -> t.Key = skey))

    member fm.FilterWaysByTag key value =
        let skey = fm.StringTable.GetSid key
        let sval = fm.StringTable.GetSid value
        fm.AllWays
        |> Seq.filter (fun w -> w.TagsRaw |> Seq.exists (fun t -> t.Key = skey && t.Val = sval))

    member fm.FilterNodesByTag key value =
        let skey = fm.StringTable.GetSid key
        let sval = fm.StringTable.GetSid value
        fm.AllNodes
        |> Seq.filter (fun n -> n.TagsRaw |> Seq.exists (fun t -> t.Key = skey && t.Val = sval))


// a simple stitching algorithm
// it assumes the ways are already in the right order,
// which is the case for the Marburg relation but not for others
let stitchboundary (ways: Way list) =

    let mutable boundary = [(ways.Head, true)]

    let add w forward = boundary <- (w, forward) :: boundary

    for w in ways.Tail do
        let (lw, forward) = boundary.Head
        let p = if forward then lw.Last else lw.First
        if w.First = p then add w true
        elif w.Last = p then add w false
        else failwith "stitching failed"

    boundary <- boundary |> List.rev
    boundary
