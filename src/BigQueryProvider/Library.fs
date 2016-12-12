module BigQueryProvider

open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection
open FSharp.Data
open System
open Newtonsoft.Json

type ProjectReference = {
        ProjectId: string
    }

type Project = {
    Kind: string
    Id: string
    NumericId: string
    ProjectReference: ProjectReference
    FriendlyName: string
}

type ProjectResponse = {
    Kind: string
    [<JsonProperty("etag")>]ETag: string
    Projects: Project list
    TotalItems: int
}



// {
//  "kind": "bigquery#projectList",
//  "etag": "\"wWvNncJfeAdSHVaIWRpICxBS7AM/8EXTgQqP7ykbmT0zT3OKpGhJffE\"",
//  "projects": [
//   {
//    "kind": "bigquery#project",
//    "id": "uc-prox-development",
//    "numericId": "653283520984",
//    "projectReference": {
//     "projectId": "uc-prox-development"
//    },
//    "friendlyName": "uc-prox-development"
//   },
//   {
//    "kind": "bigquery#project",
//    "id": "uc-prox-production",
//    "numericId": "96045981614",
//    "projectReference": {
//     "projectId": "uc-prox-production"
//    },
//    "friendlyName": "uc-prox-production"
//   },
//   {
//    "kind": "bigquery#project",
//    "id": "uc-prox-sandbox",
//    "numericId": "458145531920",
//    "projectReference": {
//     "projectId": "uc-prox-sandbox"
//    },
//    "friendlyName": "uc-prox-sandbox"
//   }
//  ],
//  "totalItems": 3
// }

module Json = 
  open Newtonsoft.Json
  let parseJson<'a> str = JsonConvert.DeserializeObject<'a>(str)

module Auth = 
  open Json
  open Newtonsoft.Json
  open System.IO

  type AuthFile = 
    {
      [<JsonProperty("client_id")>]ClientId: string
      [<JsonProperty("client_secret")>]ClientSecret: string
      [<JsonProperty("refresh_token")>]RefreshToken: string
      [<JsonProperty("type")>]Type: string
    }

  type AuthToken = 
    {
      [<JsonProperty("access_token")>]AccessToken: string
      [<JsonProperty("token_type")>]TokenType: string
      [<JsonProperty("expires_in")>]ExpiresIn: string
      [<JsonProperty("id_token")>]IdToken: string    
    }

  let getWellknownFileContent() = 
    let relativePath = ".config/gcloud/application_default_credentials.json"
    let homePath = Environment.GetEnvironmentVariable("HOME")
    let path = sprintf "%s/%s" homePath relativePath
    File.ReadAllText(path)

  let refreshToken authFile = 
    let authUrl = "https://www.googleapis.com/oauth2/v4/token"
    let requestBody = 
      [
        "grant_type", "refresh_token"
        "client_id", authFile.ClientId
        "client_secret", authFile.ClientSecret
        "refresh_token", authFile.RefreshToken
      ]
    let response = Http.RequestString(authUrl, body = HttpRequestBody.FormValues requestBody)
    response

  let authenticate() = 
    getWellknownFileContent()
    |> parseJson<AuthFile>
    |> refreshToken
    |> parseJson<AuthToken>
    |> (fun i -> i.AccessToken)


module BigQueryData = 
    let getProjects authToken = 
        let url = sprintf "https://www.googleapis.com/bigquery/v2/projects/"
        let response = 
            Http.RequestString(url, 
                headers = 
                    [
                        "Authorization", (sprintf "Bearer %s" authToken)
                    ])
        let tables = Json.parseJson<ProjectResponse>(response)
        tables


module BigQueryProviderRuntime = 
  open BigQueryData
  open Auth
  let getProject projectId = 
    Auth.authenticate()
    |> BigQueryData.getProjects
    |> (fun p -> p.Projects)
    |> List.find (fun p -> p.Id = projectId)

[<TypeProvider>]
type BigQueryProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces ()

    let ns = "BigQueryProvider"
    let asm = Assembly.GetExecutingAssembly()

    let createProjectProperty (proj:Project) =
      let id = proj.Id
      ProvidedProperty(id, typeof<Project>, IsStatic = true,
                                        GetterCode = fun args -> <@@ BigQueryProviderRuntime.getProject id @@>)

    let createTypes () =
        let authToken = Auth.authenticate()
        let projectResponse = BigQueryData.getProjects authToken

        let projects = ProvidedTypeDefinition(asm, ns, "Projects", Some typeof<obj>)

        projectResponse.Projects
        |> List.map createProjectProperty
        |> List.iter projects.AddMember



        // let ctor = ProvidedConstructor([], InvokeCode = fun args -> <@@ "My internal state" :> obj @@>)
        // myType.AddMember(ctor)

        // let ctor2 = ProvidedConstructor(
        //                 [ProvidedParameter("InnerState", typeof<string>)],
        //                 InvokeCode = fun args -> <@@ (%%(args.[0]):string) :> obj @@>)
        // myType.AddMember(ctor2)

        // let innerState = ProvidedProperty("InnerState", typeof<string>,
        //                     GetterCode = fun args -> <@@ (%%(args.[0]) :> obj) :?> string @@>)
        // myType.AddMember(innerState)

        [projects]

    do
        this.AddNamespace(ns, createTypes())
[<assembly:TypeProviderAssembly>]
do ()