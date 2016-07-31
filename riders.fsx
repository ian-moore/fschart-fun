#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"
#r "System.Windows.Forms.dll"
#r "System.Windows.Forms.DataVisualization.dll"

#r "./packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "./packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

[<Literal>]
let sampleJson = """[{
  "date" : 978336000,
  "daytype" : "U",
  "station_id" : "40010",
  "stationname" : "Austin-Forest Park",
  "rides" : "290"
}, {
  "date" : 978336000,
  "daytype" : "U",
  "station_id" : "40020",
  "stationname" : "Harlem-Lake",
  "rides" : "633"
}]"""

type RiderData = JsonProvider<sampleJson>

let queryRiders station = 
    let apiFormat = "https://data.cityofchicago.org/resource/5neh-572f.json?$order=date desc&$limit=100&stationname={{STATION}}"
    RiderData.Load(apiFormat.Replace("{{STATION}}", station))

let stations = [|"Howard"; "95th/Dan Ryan"; "35th/Archer"|]

let dateTimeFromUnixTime ticks =
    let epoch = new System.DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc)
    epoch.AddSeconds((float ticks)).ToLocalTime()

let makeDataPoint (r:RiderData.Root) =
    (dateTimeFromUnixTime r.Date, r.Rides)

let data = 
    stations
    |> Array.Parallel.map (fun s -> queryRiders s)
    |> Array.concat
    |> Array.groupBy (fun r -> r.Stationname)
    |> Array.map (fun (k, g) -> (k, g |> Array.map makeDataPoint))

data
|> Array.map (fun (k, g) -> Chart.Line(g,Name=k))
|> Chart.Combine
|> Chart.WithTitle(Text="CTA Riders by Station",InsideArea=false)
|> Chart.WithLegend(Enabled=true)
|> Chart.Show


