#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"
#r "System.Windows.Forms.dll"
#r "System.Windows.Forms.DataVisualization.dll"

#r "./packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "./packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

[<Literal>]
let sampleJson = """{"metadata":{"skip":0,"top":1000,"count":0,"filter":"","format":"json","orderby":{},"select":null,"entityname":"DisasterDeclarationsSummaries","url":"/api/open/v1/DisasterDeclarationsSummaries"},"DisasterDeclarationsSummaries":[{"disasterNumber":3,"ihProgramDeclared":false,"iaProgramDeclared":true,"paProgramDeclared":true,"hmProgramDeclared":true,"state":"LA","declarationDate":"1953-05-29T00:00:00.000Z","disasterType":"DR","incidentType":"Flood","title":"FLOOD","incidentBeginDate":"1953-05-29T00:00:00.000Z","incidentEndDate":"1953-05-29T00:00:00.000Z","declaredCountyArea":"","hash":"4e055303be6988a32e8ca8e11d09dfac","lastRefresh":"2015-08-04T01:43:17.917Z","disasterCloseOutDate":"1960-02-01T00:00:00.000Z","id":"5465a00f49d3d6a93c732b98"}]}"""

type DisasterData = JsonProvider<sampleJson>

let data = DisasterData.Load("http://www.fema.gov/api/open/v1/DisasterDeclarationsSummaries?$filter=state eq 'IN'&$orderby=declarationDate desc")

let disastersByType = 
    data.DisasterDeclarationsSummaries 
    |> Seq.groupBy (fun d -> d.IncidentType) 
    |> Seq.map (fun (k, g) -> (k, Seq.length g))
    |> Seq.sortBy (fun (k, n) -> n)

disastersByType 
|> Seq.fold (fun acc (k, n) -> acc + n) 0
|> printfn "Total Disasters: %i"

disastersByType
|> Chart.Column
|> Chart.WithTitle(Text="Indiana Disasters by Type",InsideArea=false)
|> Chart.WithXAxis(LabelStyle=ChartTypes.LabelStyle(Angle = -45, Interval = 1.0))
|> Chart.Show