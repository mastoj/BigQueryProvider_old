#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "bin/Release/BigQueryProvider.dll"
//#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
// #r "packages/Google.Api.Gax/lib/net45/Google.Api.Gax.dll"
// #r "packages/Google.Apis.Auth/lib/net45/Google.Apis.Auth.dll"
// #r "packages/Google.Apis.Auth/lib/net45/Google.Apis.Auth.PlatformServices.dll"


//http://blog.mavnn.co.uk/type-providers-from-the-ground-up/

open System
open FSharp.Data
// open Google.Api.Gax
// open Google.Apis.Auth.OAuth2
open Newtonsoft.Json
open System.IO

open BigQueryProvider

let project = BigQueryProvider.Projects.``uc-prox-development``
printfn "%A" project
// printfn "%s" (MyType().InnerState)
// printfn "%s" (MyType("YOLO").InnerState)



// let projectId = "uc-prox-production"
// // let bigQueryClient = BigqueryClient.Create(projectId)
// // let dataset = bigQueryClient.GetDataset("venue_visits_import")

// // let table = dataset.GetTable("gimbal_visits_airport_2016_11_10_visits_00000")

// // let rows = table.ListRows()

// // rows
// // |> Seq.take 10
// // |> Seq.iter (fun i -> let item = i.RawRow.F.Item 0 in printfn "%A, %A" (item.V) (i.Schema.Fields.ToString()))

// [<Literal>]
// let dataSetSample = """
// {
//   "kind": "bigquery#datasetList",
//   "etag": "\"wWvNncJfeAdSHVaIWRpICxBS7AM/CMmBj3t9m1m047bVlGrpPo-H59k\"",
//   "datasets": [
//     {
//       "kind": "bigquery#dataset",
//       "id": "uc-prox-production:audience_analytics",
//       "datasetReference": {
//         "datasetId": "audience_analytics",
//         "projectId": "uc-prox-production"
//       }
//     },
//     {
//       "kind": "bigquery#dataset",
//       "id": "uc-prox-production:convertro",
//       "datasetReference": {
//         "datasetId": "convertro",
//         "projectId": "uc-prox-production"
//       }
//     }]}"""

// type Dataset = JsonProvider<dataSetSample>

// let getDatasets project = 
//     let url = sprintf "https://www.googleapis.com/bigquery/v2/projects/%s/datasets" project
//     let response = 
//         Http.RequestString(url, 
//             headers = 
//                 [
//                     "Authorization", "Bearer ya29.Ci-uA5stRKgNigQ5OryhXTkF731TXAhZOZ_60kq5vQZq5mJp8RhNdufuT-QYBEA2JQ"
//                 ])
//     Dataset.Parse(response)

// [<Literal>]
// let tablesSample = """
// {
//   "kind": "bigquery#tableList",
//   "etag": "\"wWvNncJfeAdSHVaIWRpICxBS7AM/uwhpiBsMw719A6_Z1-CZIo9-DAM\"",
//   "nextPageToken": "gimbal_visits_auto_2016_11_24_visits_00005",
//   "tables": [
//     {
//       "kind": "bigquery#table",
//       "id": "uc-prox-production:venue_visits_import.areametrics_beacon_proc",
//       "tableReference": {
//         "projectId": "uc-prox-production",
//         "datasetId": "venue_visits_import",
//         "tableId": "areametrics_beacon_proc"
//       },
//       "type": "TABLE"
//     },
//     {
//       "kind": "bigquery#table",
//       "id": "uc-prox-production:venue_visits_import.areametrics_beacon_raw",
//       "tableReference": {
//         "projectId": "uc-prox-production",
//         "datasetId": "venue_visits_import",
//         "tableId": "areametrics_beacon_raw"
//       },
//       "type": "TABLE"
//     }]}"""

// type Tables = JsonProvider<tablesSample>
// let getTables authToken project dataset = 
//     let url = sprintf "https://www.googleapis.com/bigquery/v2/projects/%s/datasets/%s/tables" project dataset
//     let response = 
//         Http.RequestString(url, 
//             headers = 
//                 [
//                     "Authorization", (sprintf "Bearer %s" authToken)
//                 ])
//     let tables = Tables.Parse(response)
//     tables

// module Auth = 
//   open Newtonsoft.Json
//   open System.IO
//   let parseJson<'a> str = Newtonsoft.Json.JsonConvert.DeserializeObject<'a>(str)

//   type AuthFile = 
//     {
//       [<JsonProperty("client_id")>]ClientId: string
//       [<JsonProperty("client_secret")>]ClientSecret: string
//       [<JsonProperty("refresh_token")>]RefreshToken: string
//       [<JsonProperty("type")>]Type: string
//     }

//   type AuthToken = 
//     {
//       [<JsonProperty("access_token")>]AccessToken: string
//       [<JsonProperty("token_type")>]TokenType: string
//       [<JsonProperty("expires_in")>]ExpiresIn: string
//       [<JsonProperty("id_token")>]IdToken: string    
//     }

//   let getWellknownFileContent() = 
//     let relativePath = ".config/gcloud/application_default_credentials.json"
//     let homePath = Environment.GetEnvironmentVariable("HOME")
//     let path = sprintf "%s/%s" homePath relativePath
//     File.ReadAllText(path)

//   let refreshToken authFile = 
//     let authUrl = "https://www.googleapis.com/oauth2/v4/token"
//     let requestBody = 
//       [
//         "grant_type", "refresh_token"
//         "client_id", authFile.ClientId
//         "client_secret", authFile.ClientSecret
//         "refresh_token", authFile.RefreshToken
//       ]
//     let response = Http.RequestString(authUrl, body = HttpRequestBody.FormValues requestBody)
//     response

// open Auth
// let authenticate() = 
//   getWellknownFileContent()
//   |> parseJson<AuthFile>
//   |> refreshToken
//   |> parseJson<AuthToken>
//   |> (fun i -> i.AccessToken)

// let projectName = "uc-prox-production"
// //let productionDatasets = getDatasets projectName
// // productionDatasets.Datasets
// // |> Seq.iter (fun i -> printfn "%s" i.DatasetReference.DatasetId)


// let authToken = authenticate()
// printfn "authToken: %A" authToken

// let someTables = getTables authToken projectName "venue_visits_import"

// someTables.Tables
// |> Seq.iter (fun i -> printfn "%s" i.TableReference.TableId)

// // let scopes = 
// //     [

// //     ]
// // let credentialProvider = Google.Api.Gax.ScopedCredentialProvider(scopes)
// // let googleCredentials = GoogleCredential.GetApplicationDefaultAsync() |> Async.AwaitTask |> Async.RunSynchronously
// // let creds = credentialProvider.GetCredentials(googleCredentials)


